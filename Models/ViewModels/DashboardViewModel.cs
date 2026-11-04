namespace NoMasChihuan.Models.ViewModels;

public enum EstadoFinanciero
{
    Excelente = 1, // Verde: Ahorro >= 25%
    Aceptable = 2, // Verde claro: Ahorro 10% - 25%
    Alerta = 3,    // Amarillo: Ahorro 0% - 10% o >85% de presupuesto consumido
    Chihuan = 4    // Rojo: Gastos superan ingresos o presupuesto reventado
}

public class CategoriaGastoItem
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public double Porcentaje { get; set; }
    public string ColorHex { get; set; } = "#0d6efd";
    public string Icono { get; set; } = "bi-tag";
}

public class MesHistoricoItem
{
    public string MesNombre { get; set; } = string.Empty;
    public decimal Ingresos { get; set; }
    public decimal Gastos { get; set; }
    public decimal Balance => Ingresos - Gastos;
}

public class DashboardViewModel
{
    public string UsuarioNombre { get; set; } = string.Empty;
    public int MesActual { get; set; }
    public int AnioActual { get; set; }
    public string NombreMesActual { get; set; } = string.Empty;

    public decimal TotalIngresosMes { get; set; }
    public decimal TotalGastosMes { get; set; }
    public decimal BalanceMes => TotalIngresosMes - TotalGastosMes;

    public decimal PresupuestoMes { get; set; }
    public double PorcentajePresupuestoConsumido { get; set; }
    public decimal PresupuestoDisponible => PresupuestoMes > 0 ? (PresupuestoMes - TotalGastosMes) : 0;

    public decimal TotalGastosHormigaMes { get; set; }
    public double PorcentajeGastosHormiga => TotalGastosMes > 0 ? (double)(TotalGastosHormigaMes / TotalGastosMes * 100) : 0;

    public EstadoFinanciero EstadoSemaforo { get; set; }
    public string MensajeSemaforo { get; set; } = string.Empty;

    public List<Transaccion> UltimasTransacciones { get; set; } = new();
    public List<CategoriaGastoItem> GastosPorCategoria { get; set; } = new();
    public List<MesHistoricoItem> HistoricoUltimosMeses { get; set; } = new();
    public List<RecomendacionIA> AlertasIA { get; set; } = new();
}
