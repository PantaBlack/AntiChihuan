using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoMasChihuan.Models.ViewModels;

public class TransaccionListViewModel
{
    public List<Transaccion> Transacciones { get; set; } = new();
    public int? Mes { get; set; }
    public int? Anio { get; set; }
    public TipoTransaccion? Tipo { get; set; }
    public int? CategoriaId { get; set; }
    public bool? SoloHormiga { get; set; }

    public decimal TotalIngresosFiltrados { get; set; }
    public decimal TotalGastosFiltrados { get; set; }
    public decimal BalanceFiltrado => TotalIngresosFiltrados - TotalGastosFiltrados;

    public SelectList? CategoriasSelectList { get; set; }
}

public class TransaccionFormViewModel
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public TipoTransaccion Tipo { get; set; } = TipoTransaccion.Gasto;
    public bool EsGastoHormiga { get; set; }
    public string? Notas { get; set; }
    public int CategoriaId { get; set; }
    public SelectList? CategoriasSelectList { get; set; }
}
