using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoMasChihuan.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(60, ErrorMessage = "El nombre no puede exceder 60 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public TipoTransaccion Tipo { get; set; } = TipoTransaccion.Gasto;

    [StringLength(50)]
    public string Icono { get; set; } = "bi-tag"; // Bootstrap Icons class

    [StringLength(20)]
    public string ColorHex { get; set; } = "#0d6efd";

    public bool EsPredeterminada { get; set; } = false;

    // Si es null, es una categoría del sistema para todos los usuarios
    public string? UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual ApplicationUser? Usuario { get; set; }

    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
}
