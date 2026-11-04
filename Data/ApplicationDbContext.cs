using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Models;

namespace NoMasChihuan.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transaccion> Transacciones => Set<Transaccion>();
    public DbSet<Presupuesto> Presupuestos => Set<Presupuesto>();
    public DbSet<RecomendacionIA> RecomendacionesIA => Set<RecomendacionIA>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Precisión para campos monetarios
        builder.Entity<Transaccion>()
            .Property(t => t.Monto)
            .HasPrecision(18, 2);

        builder.Entity<Presupuesto>()
            .Property(p => p.MontoLimite)
            .HasPrecision(18, 2);

        // Relaciones de Transaccion
        builder.Entity<Transaccion>()
            .HasOne(t => t.Usuario)
            .WithMany(u => u.Transacciones)
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Transaccion>()
            .HasOne(t => t.Categoria)
            .WithMany(c => c.Transacciones)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relaciones de Presupuesto
        builder.Entity<Presupuesto>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Presupuestos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Presupuesto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Presupuestos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relaciones de RecomendacionIA
        builder.Entity<RecomendacionIA>()
            .HasOne(r => r.Usuario)
            .WithMany(u => u.RecomendacionesIA)
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relaciones de Categoria (Restrict para evitar ciclos de cascada en SQL Server)
        builder.Entity<Categoria>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.CategoriasPersonalizadas)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Categorías Predeterminadas
        builder.Entity<Categoria>().HasData(
            // Ingresos
            new Categoria { Id = 1, Nombre = "Salario / Sueldo", Tipo = TipoTransaccion.Ingreso, Icono = "bi-cash-coin", ColorHex = "#198754", EsPredeterminada = true },
            new Categoria { Id = 2, Nombre = "Ventas / Negocio", Tipo = TipoTransaccion.Ingreso, Icono = "bi-shop", ColorHex = "#20c997", EsPredeterminada = true },
            new Categoria { Id = 3, Nombre = "Freelance / Proyectos", Tipo = TipoTransaccion.Ingreso, Icono = "bi-laptop", ColorHex = "#0dcaf0", EsPredeterminada = true },
            new Categoria { Id = 4, Nombre = "Inversiones", Tipo = TipoTransaccion.Ingreso, Icono = "bi-graph-up-arrow", ColorHex = "#17a2b8", EsPredeterminada = true },
            new Categoria { Id = 5, Nombre = "Otros Ingresos", Tipo = TipoTransaccion.Ingreso, Icono = "bi-plus-circle", ColorHex = "#6c757d", EsPredeterminada = true },

            // Gastos
            new Categoria { Id = 6, Nombre = "Alimentación y Supermercado", Tipo = TipoTransaccion.Gasto, Icono = "bi-cart4", ColorHex = "#fd7e14", EsPredeterminada = true },
            new Categoria { Id = 7, Nombre = "Vivienda y Alquiler", Tipo = TipoTransaccion.Gasto, Icono = "bi-house-door", ColorHex = "#dc3545", EsPredeterminada = true },
            new Categoria { Id = 8, Nombre = "Servicios (Luz, Agua, Internet)", Tipo = TipoTransaccion.Gasto, Icono = "bi-lightning-charge", ColorHex = "#ffc107", EsPredeterminada = true },
            new Categoria { Id = 9, Nombre = "Transporte y Gasolina", Tipo = TipoTransaccion.Gasto, Icono = "bi-car-front", ColorHex = "#6f42c1", EsPredeterminada = true },
            new Categoria { Id = 10, Nombre = "Salud y Medicinas", Tipo = TipoTransaccion.Gasto, Icono = "bi-heart-pulse", ColorHex = "#e83e8c", EsPredeterminada = true },
            new Categoria { Id = 11, Nombre = "Educación y Cursos", Tipo = TipoTransaccion.Gasto, Icono = "bi-book", ColorHex = "#0d6efd", EsPredeterminada = true },
            new Categoria { Id = 12, Nombre = "Entretenimiento y Salidas", Tipo = TipoTransaccion.Gasto, Icono = "bi-controller", ColorHex = "#d63384", EsPredeterminada = true },
            new Categoria { Id = 13, Nombre = "Gastos Hormiga (Snacks, Cafés, Antojos)", Tipo = TipoTransaccion.Gasto, Icono = "bi-cup-hot", ColorHex = "#b02a37", EsPredeterminada = true },
            new Categoria { Id = 14, Nombre = "Compras Personales y Ropa", Tipo = TipoTransaccion.Gasto, Icono = "bi-bag-check", ColorHex = "#0b5ed7", EsPredeterminada = true },
            new Categoria { Id = 15, Nombre = "Otros Gastos", Tipo = TipoTransaccion.Gasto, Icono = "bi-dash-circle", ColorHex = "#6c757d", EsPredeterminada = true }
        );
    }
}
