using NoMasChihuan.Models.ViewModels;

namespace NoMasChihuan.Services;

public interface IAIService
{
    Task<IAConsejoViewModel> GenerarDiagnosticoFinancieroAsync(string userId);
}
