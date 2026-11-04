using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoMasChihuan.Models;

public enum NivelPrioridad
{
    Baja = 1,
    Media = 2,
    Alta = 3,
    Critica = 4 // ¡Alerta Chihuán!
}

public class RecomendacionIA
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public string Mensaje { get; set; } = string.Empty;

    [StringLength(50)]
    public string CategoriaAnalizada { get; set; } = "General";

    public NivelPrioridad Prioridad { get; set; } = NivelPrioridad.Media;

    public bool Leida { get; set; } = false;

    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [ForeignKey(nameof(UsuarioId))]
    public virtual ApplicationUser? Usuario { get; set; }
}
