using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Data;
using NoMasChihuan.Models;
using NoMasChihuan.Models.ViewModels;

namespace NoMasChihuan.Services;

public class GeminiAIService : IAIService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeminiAIService> _logger;

    public GeminiAIService(
        ApplicationDbContext context,
        IConfiguration config,
        HttpClient httpClient,
        ILogger<GeminiAIService> logger)
    {
        _context = context;
        _config = config;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IAConsejoViewModel> GenerarDiagnosticoFinancieroAsync(string userId)
    {
        var now = DateTime.Today;
        var mesActual = now.Month;
        var anioActual = now.Year;

        // 1. Cargar datos del usuario
        var user = await _context.Users.FindAsync(userId);
        var transacciones = await _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == userId && t.Fecha >= now.AddMonths(-2))
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();

        var presupuestos = await _context.Presupuestos
            .Include(p => p.Categoria)
            .Where(p => p.UsuarioId == userId && p.Mes == mesActual && p.Anio == anioActual)
            .ToListAsync();

        var transaccionesMes = transacciones
            .Where(t => t.Fecha.Month == mesActual && t.Fecha.Year == anioActual)
            .ToList();

        var totalIngresosMes = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Ingreso)
            .Sum(t => t.Monto);

        var totalGastosMes = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto)
            .Sum(t => t.Monto);

        var gastosHormiga = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto && (t.EsGastoHormiga || (t.Categoria != null && t.Categoria.Nombre.Contains("Hormiga"))))
            .Sum(t => t.Monto);

        var gastosPorCat = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto && t.Categoria != null)
            .GroupBy(t => t.Categoria!.Nombre)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(x => x.Monto) })
            .OrderByDescending(x => x.Total)
            .ToList();

        var apiKey = _config["Gemini:ApiKey"];
        IAConsejoViewModel resultado;

        // 2. Intentar llamar a Gemini si hay API Key configurada
        if (!string.IsNullOrWhiteSpace(apiKey) && apiKey != "TU_GEMINI_API_KEY_AQUI")
        {
            resultado = await LlamarGeminiApiAsync(apiKey, user?.NombreCompleto ?? "Usuario", totalIngresosMes, totalGastosMes, gastosHormiga, gastosPorCat, presupuestos, transaccionesMes);
        }
        else
        {
            // Motor Heurístico Inteligente AntiChihuan
            resultado = GenerarDiagnosticoHeuristico(totalIngresosMes, totalGastosMes, gastosHormiga, gastosPorCat, presupuestos);
        }

        // 3. Guardar recomendación en la Base de Datos para el historial del usuario
        try
        {
            var nivelPrioridad = resultado.NivelRiesgoChihuan switch
            {
                "Crítico" or "Critica" => NivelPrioridad.Critica,
                "Alto" or "Alta" => NivelPrioridad.Alta,
                "Moderado" or "Medio" => NivelPrioridad.Media,
                _ => NivelPrioridad.Baja
            };

            var primerRecomendacion = resultado.RecomendacionesAhorro.FirstOrDefault() 
                ?? resultado.AlertasPresupuesto.FirstOrDefault() 
                ?? "Continúa registrando tus movimientos.";

            var nuevaRecomendacion = new RecomendacionIA
            {
                UsuarioId = userId,
                Titulo = $"Diagnóstico IA: Riesgo {resultado.NivelRiesgoChihuan}",
                Mensaje = resultado.AnalisisGeneral + " Consejo clave: " + primerRecomendacion,
                Prioridad = nivelPrioridad,
                CategoriaAnalizada = gastosPorCat.FirstOrDefault()?.Categoria ?? "General",
                FechaGeneracion = DateTime.UtcNow,
                Leida = false
            };

            _context.RecomendacionesIA.Add(nuevaRecomendacion);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error guardando recomendación en base de datos.");
        }

        return resultado;
    }

    private async Task<IAConsejoViewModel> LlamarGeminiApiAsync(
        string apiKey,
        string nombreUsuario,
        decimal ingresos,
        decimal gastos,
        decimal gastosHormiga,
        dynamic gastosPorCat,
        List<Presupuesto> presupuestos,
        List<Transaccion> transacciones)
    {
        try
        {
            var modelo = _config["Gemini:Model"] ?? "gemini-2.5-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelo}:generateContent?key={apiKey}";

            var catString = string.Join(", ", ((IEnumerable<dynamic>)gastosPorCat).Select(c => $"{c.Categoria}: S/ {c.Total:F2}"));
            var ultimosGastos = string.Join("; ", transacciones.Where(t => t.Tipo == TipoTransaccion.Gasto).Take(10).Select(t => $"{t.Descripcion}: S/{t.Monto} ({t.Categoria?.Nombre ?? "General"})"));

            var prompt = $@"
Eres el Agente de Inteligencia Artificial Financiera del sistema 'AntiChihuan'. Tu objetivo es aconsejar a personas para que no se queden sin dinero ('chihuán'), optimicen sus gastos, eliminen gastos hormiga y aumenten su capacidad de ahorro.

Datos del usuario {nombreUsuario} este mes:
- Ingresos totales del mes: S/ {ingresos:F2}
- Gastos totales del mes: S/ {gastos:F2}
- Gastos Hormiga identificados: S/ {gastosHormiga:F2}
- Distribución de gastos por categoría: {catString}
- Muestra de gastos recientes: {ultimosGastos}

Devuelve ÚNICAMENTE un objeto JSON válido (sin texto Markdown adicional alrededor, o solo un bloque ```json ... ```) con la siguiente estructura exacta:
{{
  ""analisisGeneral"": ""string con un resumen conciso y directo del estado de sus finanzas"",
  ""nivelRiesgoChihuan"": ""Bajo"" | ""Moderado"" | ""Alto"" | ""Crítico"",
  ""gastosInnecesariosDetectados"": [""detalle de gasto o fuga detectada"", ...],
  ""recomendacionesAhorro"": [""consejo práctico 1"", ""consejo práctico 2"", ...],
  ""alertasPresupuesto"": [""alerta relevante"", ...],
  ""estimadoAhorroMensualSugerido"": número decimal sugerido de ahorro mensual
}}
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.4,
                    responseMimeType = "application/json"
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            if (response.IsSuccessStatusCode)
            {
                var jsonDoc = await response.Content.ReadFromJsonAsync<JsonElement>();
                var rawText = jsonDoc.GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                if (!string.IsNullOrWhiteSpace(rawText))
                {
                    // Limpieza en caso venga envuelto en backticks
                    var cleanJson = rawText.Trim();
                    if (cleanJson.StartsWith("```json"))
                        cleanJson = cleanJson[7..];
                    if (cleanJson.StartsWith("```"))
                        cleanJson = cleanJson[3..];
                    if (cleanJson.EndsWith("```"))
                        cleanJson = cleanJson[..^3];

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parseado = JsonSerializer.Deserialize<IAConsejoViewModel>(cleanJson.Trim(), options);
                    if (parseado != null)
                    {
                        parseado.FechaAnalisis = DateTime.UtcNow;
                        return parseado;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falló la consulta con Gemini API, recurriendo a análisis heurístico.");
        }

        return GenerarDiagnosticoHeuristico(ingresos, gastos, gastosHormiga, gastosPorCat, presupuestos);
    }

    private IAConsejoViewModel GenerarDiagnosticoHeuristico(
        decimal ingresos,
        decimal gastos,
        decimal gastosHormiga,
        dynamic gastosPorCat,
        List<Presupuesto> presupuestos)
    {
        var resultado = new IAConsejoViewModel
        {
            FechaAnalisis = DateTime.UtcNow
        };

        var balance = ingresos - gastos;
        var pctGasto = ingresos > 0 ? (gastos / ingresos * 100) : 100;
        var pctHormiga = gastos > 0 ? (gastosHormiga / gastos * 100) : 0;

        if (gastos > ingresos && ingresos > 0)
        {
            resultado.NivelRiesgoChihuan = "Crítico";
            resultado.AnalisisGeneral = $"¡Atención urgente! Has gastado S/ {gastos:F2} habiendo ingresado S/ {ingresos:F2}, dejando un déficit de S/ {Math.Abs(balance):F2}. Estás en riesgo extremo de endeudamiento o quedarte sin liquidez.";
            resultado.AlertasPresupuesto.Add("Tus egresos superan en un " + (pctGasto - 100).ToString("F1") + "% a tus ingresos.");
            resultado.RecomendacionesAhorro.Add("Congela compras no esenciales (ropa, salidas, suscripciones de ocio) de inmediato.");
            resultado.RecomendacionesAhorro.Add("Aplica la regla de emergencia de supervivencia: solo comida básica, transporte y servicios vitales.");
        }
        else if (pctGasto >= 90)
        {
            resultado.NivelRiesgoChihuan = "Alto";
            resultado.AnalisisGeneral = $"Tus gastos representan el {pctGasto:F1}% de tus ingresos. Tu colchón financiero es de apenas S/ {balance:F2}, cualquier imprevisto te dejará en números rojos.";
            resultado.AlertasPresupuesto.Add("Margen de ahorro prácticamente nulo (menos del 10%).");
            resultado.RecomendacionesAhorro.Add("Establece un presupuesto estricto por categoría semanal para evitar quedarte corto a fin de mes.");
        }
        else if (pctGasto >= 75)
        {
            resultado.NivelRiesgoChihuan = "Moderado";
            resultado.AnalisisGeneral = $"Tienes una situación estable pero con margen de mejora. Estás gastando el {pctGasto:F1}% de tus ingresos, lo que te permite ahorrar S/ {balance:F2}.";
            resultado.RecomendacionesAhorro.Add("Apunta a ahorrar al menos el 20% de tus ingresos cada mes (regla 50/30/20).");
        }
        else
        {
            resultado.NivelRiesgoChihuan = "Bajo";
            resultado.AnalisisGeneral = $"¡Excelente disciplina financiera! Estás gastando de manera prudente ({pctGasto:F1}% de tus ingresos) y tienes un superávit mensual de S/ {balance:F2}.";
            resultado.RecomendacionesAhorro.Add("Destina tus ahorros a un fondo de emergencia en una cuenta de alto rendimiento o depósito a plazo fijo.");
        }

        // Detección de Gastos Hormiga
        if (gastosHormiga > 0)
        {
            if (pctHormiga >= 15)
            {
                resultado.GastosInnecesariosDetectados.Add($"Gastos Hormiga elevados: S/ {gastosHormiga:F2} ({pctHormiga:F1}% de tus gastos). Antojos, deliveries y cafés diarios están drenando tu dinero.");
                resultado.RecomendacionesAhorro.Add($"Si reduces tus gastos hormiga a la mitad, podrías ahorrar S/ {(gastosHormiga / 2):F2} adicionales cada mes.");
            }
            else
            {
                resultado.GastosInnecesariosDetectados.Add($"Gastos Hormiga registrados: S/ {gastosHormiga:F2}. Mantenlos bajo control.");
            }
        }
        else
        {
            resultado.RecomendacionesAhorro.Add("Recuerda marcar las compras pequeñas (golosinas, propinas, taxis innecesarios) como 'Gasto Hormiga' para detectarlas a tiempo.");
        }

        // Revisión de Presupuestos
        foreach (var p in presupuestos)
        {
            if (p.Categoria != null)
            {
                resultado.AlertasPresupuesto.Add($"Tienes un límite presupuestario asignado para '{p.Categoria.Nombre}' de S/ {p.MontoLimite:F2}.");
            }
        }

        resultado.EstimadoAhorroMensualSugerido = Math.Max(50, ingresos * 0.15m);

        return resultado;
    }
}
