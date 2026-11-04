using NoMasChihuan.Models;
using NoMasChihuan.Models.ViewModels;

namespace NoMasChihuan.Services;

public interface IFinanceService
{
    Task<DashboardViewModel> GetDashboardDataAsync(string userId, int? mes = null, int? anio = null);
    Task<PresupuestosIndexViewModel> GetPresupuestosOverviewAsync(string userId, int? mes = null, int? anio = null);
    Task<List<Categoria>> GetCategoriasDisponiblesAsync(string userId, TipoTransaccion? tipo = null);
    Task<TransaccionListViewModel> GetTransaccionesFiltradasAsync(string userId, int? mes, int? anio, TipoTransaccion? tipo, int? categoriaId, bool? soloHormiga);
}
