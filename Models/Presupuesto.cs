using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoMasChihuan.Models;

public class Presupuesto
{
    public int Id { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
    public int Mes { get; set; } = DateTime.Today.Month;

    [Required]
    [Range(2020, 2100, ErrorMessage = "Año fuera de rango.")]
    public int Anio { get; set; } = DateTime.Today.Year;

    [Required(ErrorMessage = "El monto presupuestado es obligatorio.")]
    [Range(0.01, 9999999.99, ErrorMessage = "El límite debe ser mayor a 0.")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Monto Límite")]
    public decimal MontoLimite { get; set; }

    // Opcional: Si se asigna Categoría, el presupuesto es específico para dicha categoría. Si es null, es límite general mensual.
    [Display(Name = "Categoría (opcional)")]
    public int? CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria? Categoria { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [ForeignKey(nameof(UsuarioId))]
    public virtual ApplicationUser? Usuario { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
