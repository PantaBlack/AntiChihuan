using Microsoft.AspNetCore.Mvc.Rendering;

namespace NoMasChihuan.Models.ViewModels;

public class PresupuestoItemViewModel
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Anio { get; set; }
    public int? CategoriaId { get; set; }
    public string NombreCategoria { get; set; } = "Presupuesto General";
    public string IconoCategoria { get; set; } = "bi-wallet2";
    public string ColorHex { get; set; } = "#0d6efd";
    public decimal MontoLimite { get; set; }
    public decimal MontoGastado { get; set; }
    public decimal MontoDisponible => MontoLimite - MontoGastado;
    public double PorcentajeConsumido => MontoLimite > 0 ? (double)(MontoGastado / MontoLimite * 100) : 0;
    public bool Excedido => MontoGastado > MontoLimite;
}

public class PresupuestosIndexViewModel
{
    public int Mes { get; set; }
    public int Anio { get; set; }
    public decimal PresupuestoTotal { get; set; }
    public decimal GastoTotalMes { get; set; }
    public double PorcentajeTotalConsumido => PresupuestoTotal > 0 ? (double)(GastoTotalMes / PresupuestoTotal * 100) : 0;
    public List<PresupuestoItemViewModel> Items { get; set; } = new();
}

public class PresupuestoFormViewModel
{
    public int Id { get; set; }
    public int Mes { get; set; } = DateTime.Today.Month;
    public int Anio { get; set; } = DateTime.Today.Year;
    public decimal MontoLimite { get; set; }
    public int? CategoriaId { get; set; }
    public SelectList? CategoriasSelectList { get; set; }
}
