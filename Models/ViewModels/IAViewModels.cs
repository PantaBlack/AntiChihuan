namespace NoMasChihuan.Models.ViewModels;

public class IAConsejoViewModel
{
    public string AnalisisGeneral { get; set; } = string.Empty;
    public string NivelRiesgoChihuan { get; set; } = "Bajo"; // Bajo, Moderado, Alto, Crítico
    public List<string> GastosInnecesariosDetectados { get; set; } = new();
    public List<string> RecomendacionesAhorro { get; set; } = new();
    public List<string> AlertasPresupuesto { get; set; } = new();
    public decimal EstimadoAhorroMensualSugerido { get; set; }
    public DateTime FechaAnalisis { get; set; } = DateTime.UtcNow;
}

public class IAPanelViewModel
{
    public IAConsejoViewModel? UltimoDiagnostico { get; set; }
    public List<RecomendacionIA> HistorialRecomendaciones { get; set; } = new();
    public decimal TotalGastosHormiga { get; set; }
    public decimal TotalGastosMes { get; set; }
    public decimal TotalIngresosMes { get; set; }
    public bool HayDatosSuficientes { get; set; } = true;
}
