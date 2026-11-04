using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Data;
using NoMasChihuan.Models;

namespace NoMasChihuan.Controllers;

[Authorize]
public class CategoriasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CategoriasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var categorias = await _context.Categorias
            .Where(c => c.EsPredeterminada || c.UsuarioId == user.Id)
            .OrderBy(c => c.Tipo)
            .ThenBy(c => c.Nombre)
            .ToListAsync();

        return View(categorias);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Categoria model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(model.Nombre))
        {
            TempData["ErrorMessage"] = "El nombre de la categoría es requerido.";
            return RedirectToAction(nameof(Index));
        }

        model.UsuarioId = user.Id;
        model.EsPredeterminada = false;
        if (string.IsNullOrWhiteSpace(model.ColorHex)) model.ColorHex = "#6c757d";
        if (string.IsNullOrWhiteSpace(model.Icono)) model.Icono = "bi-tag";

        _context.Categorias.Add(model);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Categoría personalizada creada con éxito.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var categoria = await _context.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == user.Id && !c.EsPredeterminada);

        if (categoria == null)
        {
            TempData["ErrorMessage"] = "No puedes eliminar esta categoría.";
            return RedirectToAction(nameof(Index));
        }

        var tieneUso = await _context.Transacciones.AnyAsync(t => t.CategoriaId == id);
        if (tieneUso)
        {
            TempData["ErrorMessage"] = "No puedes eliminar una categoría que contiene transacciones asociadas.";
            return RedirectToAction(nameof(Index));
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Categoría eliminada.";
        return RedirectToAction(nameof(Index));
    }
}
