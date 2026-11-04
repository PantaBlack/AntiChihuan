using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoMasChihuan.Models;

public class Transaccion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(150, ErrorMessage = "La descripción no puede superar los 150 caracteres.")]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, 9999999.99, ErrorMessage = "El monto debe ser mayor a 0.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Tipo")]
    public TipoTransaccion Tipo { get; set; } = TipoTransaccion.Gasto;

    [Display(Name = "¿Es gasto hormiga?")]
    public bool EsGastoHormiga { get; set; } = false;

    [StringLength(300)]
    [Display(Name = "Notas adicionales")]
    public string? Notas { get; set; }

    // Clave foránea de Categoría
    [Required(ErrorMessage = "Debe seleccionar una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [ForeignKey(nameof(CategoriaId))]
    public virtual Categoria? Categoria { get; set; }

    // Clave foránea de Usuario
    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [ForeignKey(nameof(UsuarioId))]
    public virtual ApplicationUser? Usuario { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
