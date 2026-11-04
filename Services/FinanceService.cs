using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Data;
using NoMasChihuan.Models;
using NoMasChihuan.Models.ViewModels;

namespace NoMasChihuan.Services;

public class FinanceService : IFinanceService
{
    private readonly ApplicationDbContext _context;

    public FinanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(string userId, int? mes = null, int? anio = null)
    {
        var now = DateTime.Today;
        var selectedMes = mes ?? now.Month;
        var selectedAnio = anio ?? now.Year;

        var cultureEs = new CultureInfo("es-PE");
        var nombreMes = cultureEs.DateTimeFormat.GetMonthName(selectedMes);
        nombreMes = char.ToUpper(nombreMes[0]) + nombreMes[1..];

        // 1. Obtener usuario
        var user = await _context.Users.FindAsync(userId);
        var nombreUsuario = user?.NombreCompleto ?? "Usuario";

        // 2. Transacciones del mes
        var transaccionesMes = await _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == userId && t.Fecha.Month == selectedMes && t.Fecha.Year == selectedAnio)
            .ToListAsync();

        var totalIngresos = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Ingreso)
            .Sum(t => t.Monto);

        var totalGastos = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto)
            .Sum(t => t.Monto);

        var totalGastosHormiga = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto && (t.EsGastoHormiga || (t.Categoria != null && t.Categoria.Nombre.Contains("Hormiga"))))
            .Sum(t => t.Monto);

        // 3. Presupuesto del mes (si existe presupuesto general o suma de presupuestos por categoría)
        var presupuestosMes = await _context.Presupuestos
            .Where(p => p.UsuarioId == userId && p.Mes == selectedMes && p.Anio == selectedAnio)
            .ToListAsync();

        decimal presupuestoTotal = 0;
        var presupuestoGeneral = presupuestosMes.FirstOrDefault(p => p.CategoriaId == null);
        if (presupuestoGeneral != null)
        {
            presupuestoTotal = presupuestoGeneral.MontoLimite;
        }
        else if (presupuestosMes.Any())
        {
            presupuestoTotal = presupuestosMes.Sum(p => p.MontoLimite);
        }

        double pctConsumido = presupuestoTotal > 0 ? (double)(totalGastos / presupuestoTotal * 100) : 0;

        // 4. Semáforo Financiero
        var estado = EstadoFinanciero.Excelente;
        string mensaje = "Tus finanzas van por excelente camino. ¡Sigue así!";

        if (totalGastos > totalIngresos && totalIngresos > 0)
        {
            estado = EstadoFinanciero.Chihuan;
            mensaje = "¡Alerta Roja Chihuán! Estás gastando más de lo que ganas este mes.";
        }
        else if (presupuestoTotal > 0 && totalGastos > presupuestoTotal)
        {
            estado = EstadoFinanciero.Chihuan;
            mensaje = "¡Excediste tu presupuesto mensual! Corta gastos no esenciales de inmediato.";
        }
        else if (presupuestoTotal > 0 && pctConsumido >= 85)
        {
            estado = EstadoFinanciero.Alerta;
            mensaje = $"¡Cuidado! Ya consumiste el {pctConsumido:F1}% de tu presupuesto mensual.";
        }
        else if (totalIngresos > 0 && ((totalIngresos - totalGastos) / totalIngresos) < 0.10m)
        {
            estado = EstadoFinanciero.Alerta;
            mensaje = "Margen de ahorro muy bajo (menos del 10%). Evita gastos innecesarios.";
        }
        else if (totalIngresos > 0 && ((totalIngresos - totalGastos) / totalIngresos) < 0.20m)
        {
            estado = EstadoFinanciero.Aceptable;
            mensaje = "Finanzas estables, pero puedes optimizar tus gastos hormiga para ahorrar más.";
        }

        // 5. Gastos agrupados por categoría (para gráfico de dona)
        var gastosPorCat = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto && t.Categoria != null)
            .GroupBy(t => t.Categoria!)
            .Select(g => new CategoriaGastoItem
            {
                Categoria = g.Key.Nombre,
                Monto = g.Sum(x => x.Monto),
                ColorHex = g.Key.ColorHex,
                Icono = g.Key.Icono,
                Porcentaje = totalGastos > 0 ? (double)(g.Sum(x => x.Monto) / totalGastos * 100) : 0
            })
            .OrderByDescending(x => x.Monto)
            .ToList();

        // 6. Histórico últimos 6 meses (para gráfico de barras)
        var historico = new List<MesHistoricoItem>();
        for (int i = 5; i >= 0; i--)
        {
            var dt = now.AddMonths(-i);
            var m = dt.Month;
            var a = dt.Year;
            var mNombre = cultureEs.DateTimeFormat.GetAbbreviatedMonthName(m) + " " + a;

            var tHist = await _context.Transacciones
                .Where(t => t.UsuarioId == userId && t.Fecha.Month == m && t.Fecha.Year == a)
                .ToListAsync();

            var ing = tHist.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto);
            var gas = tHist.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto);

            historico.Add(new MesHistoricoItem
            {
                MesNombre = mNombre,
                Ingresos = ing,
                Gastos = gas
            });
        }

        // 7. Últimas 8 transacciones
        var ultimas = await _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == userId)
            .OrderByDescending(t => t.Fecha)
            .ThenByDescending(t => t.Id)
            .Take(8)
            .ToListAsync();

        // 8. Alertas IA recientes
        var alertas = await _context.RecomendacionesIA
            .Where(r => r.UsuarioId == userId)
            .OrderByDescending(r => r.FechaGeneracion)
            .Take(4)
            .ToListAsync();

        return new DashboardViewModel
        {
            UsuarioNombre = nombreUsuario,
            MesActual = selectedMes,
            AnioActual = selectedAnio,
            NombreMesActual = nombreMes,
            TotalIngresosMes = totalIngresos,
            TotalGastosMes = totalGastos,
            PresupuestoMes = presupuestoTotal,
            PorcentajePresupuestoConsumido = pctConsumido,
            TotalGastosHormigaMes = totalGastosHormiga,
            EstadoSemaforo = estado,
            MensajeSemaforo = mensaje,
            UltimasTransacciones = ultimas,
            GastosPorCategoria = gastosPorCat,
            HistoricoUltimosMeses = historico,
            AlertasIA = alertas
        };
    }

    public async Task<PresupuestosIndexViewModel> GetPresupuestosOverviewAsync(string userId, int? mes = null, int? anio = null)
    {
        var now = DateTime.Today;
        var selectedMes = mes ?? now.Month;
        var selectedAnio = anio ?? now.Year;

        var presupuestos = await _context.Presupuestos
            .Include(p => p.Categoria)
            .Where(p => p.UsuarioId == userId && p.Mes == selectedMes && p.Anio == selectedAnio)
            .ToListAsync();

        var transaccionesMes = await _context.Transacciones
            .Where(t => t.UsuarioId == userId && t.Tipo == TipoTransaccion.Gasto && t.Fecha.Month == selectedMes && t.Fecha.Year == selectedAnio)
            .ToListAsync();

        var totalGastadoMes = transaccionesMes.Sum(t => t.Monto);

        var items = new List<PresupuestoItemViewModel>();

        foreach (var p in presupuestos)
        {
            decimal gastado = 0;
            if (p.CategoriaId.HasValue)
            {
                gastado = transaccionesMes
                    .Where(t => t.CategoriaId == p.CategoriaId.Value)
                    .Sum(t => t.Monto);
            }
            else
            {
                gastado = totalGastadoMes;
            }

            items.Add(new PresupuestoItemViewModel
            {
                Id = p.Id,
                Mes = p.Mes,
                Anio = p.Anio,
                CategoriaId = p.CategoriaId,
                NombreCategoria = p.Categoria != null ? p.Categoria.Nombre : "Presupuesto General",
                IconoCategoria = p.Categoria?.Icono ?? "bi-wallet2",
                ColorHex = p.Categoria?.ColorHex ?? "#0d6efd",
                MontoLimite = p.MontoLimite,
                MontoGastado = gastado
            });
        }

        var totalPresupuesto = presupuestos.FirstOrDefault(p => p.CategoriaId == null)?.MontoLimite 
            ?? presupuestos.Sum(p => p.MontoLimite);

        return new PresupuestosIndexViewModel
        {
            Mes = selectedMes,
            Anio = selectedAnio,
            PresupuestoTotal = totalPresupuesto,
            GastoTotalMes = totalGastadoMes,
            Items = items
        };
    }

    public async Task<List<Categoria>> GetCategoriasDisponiblesAsync(string userId, TipoTransaccion? tipo = null)
    {
        var query = _context.Categorias
            .Where(c => c.EsPredeterminada || c.UsuarioId == userId);

        if (tipo.HasValue)
        {
            query = query.Where(c => c.Tipo == tipo.Value);
        }

        return await query.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task<TransaccionListViewModel> GetTransaccionesFiltradasAsync(
        string userId, int? mes, int? anio, TipoTransaccion? tipo, int? categoriaId, bool? soloHormiga)
    {
        var query = _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == userId);

        if (mes.HasValue && mes.Value > 0)
        {
            query = query.Where(t => t.Fecha.Month == mes.Value);
        }

        if (anio.HasValue && anio.Value > 0)
        {
            query = query.Where(t => t.Fecha.Year == anio.Value);
        }

        if (tipo.HasValue)
        {
            query = query.Where(t => t.Tipo == tipo.Value);
        }

        if (categoriaId.HasValue && categoriaId.Value > 0)
        {
            query = query.Where(t => t.CategoriaId == categoriaId.Value);
        }

        if (soloHormiga.HasValue && soloHormiga.Value)
        {
            query = query.Where(t => t.EsGastoHormiga);
        }

        var transacciones = await query
            .OrderByDescending(t => t.Fecha)
            .ThenByDescending(t => t.Id)
            .ToListAsync();

        var categorias = await GetCategoriasDisponiblesAsync(userId);

        var totalIngresos = transacciones.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto);
        var totalGastos = transacciones.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto);

        return new TransaccionListViewModel
        {
            Transacciones = transacciones,
            Mes = mes,
            Anio = anio,
            Tipo = tipo,
            CategoriaId = categoriaId,
            SoloHormiga = soloHormiga,
            TotalIngresosFiltrados = totalIngresos,
            TotalGastosFiltrados = totalGastos,
            CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", categoriaId)
        };
    }
}
