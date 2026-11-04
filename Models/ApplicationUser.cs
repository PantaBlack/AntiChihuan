using Microsoft.AspNetCore.Identity;

namespace NoMasChihuan.Models;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Relaciones
    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
    public virtual ICollection<Categoria> CategoriasPersonalizadas { get; set; } = new List<Categoria>();
    public virtual ICollection<RecomendacionIA> RecomendacionesIA { get; set; } = new List<RecomendacionIA>();
}
