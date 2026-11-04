using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Data;
using NoMasChihuan.Models;
using NoMasChihuan.Models.ViewModels;
using NoMasChihuan.Services;

namespace NoMasChihuan.Controllers;

[Authorize]
public class PresupuestosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFinanceService _financeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PresupuestosController(
        ApplicationDbContext context,
        IFinanceService financeService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _financeService = financeService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? mes, int? anio)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var viewModel = await _financeService.GetPresupuestosOverviewAsync(user.Id, mes, anio);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, TipoTransaccion.Gasto);

        var model = new PresupuestoFormViewModel
        {
            Mes = DateTime.Today.Month,
            Anio = DateTime.Today.Year,
            CategoriasSelectList = new SelectList(categorias, "Id", "Nombre")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PresupuestoFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        // Validar si ya existe presupuesto para el mismo mes, año y categoría
        var existe = await _context.Presupuestos.AnyAsync(p => 
            p.UsuarioId == user.Id && 
            p.Mes == model.Mes && 
            p.Anio == model.Anio && 
            p.CategoriaId == model.CategoriaId);

        if (existe)
        {
            ModelState.AddModelError(string.Empty, "Ya existe un presupuesto asignado para ese periodo y categoría.");
        }

        if (ModelState.IsValid)
        {
            var presupuesto = new Presupuesto
            {
                UsuarioId = user.Id,
                Mes = model.Mes,
                Anio = model.Anio,
                MontoLimite = model.MontoLimite,
                CategoriaId = model.CategoriaId,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Presupuestos.Add(presupuesto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Presupuesto configurado exitosamente.";
            return RedirectToAction(nameof(Index), new { mes = model.Mes, anio = model.Anio });
        }

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, TipoTransaccion.Gasto);
        model.CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", model.CategoriaId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var presupuesto = await _context.Presupuestos.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == user.Id);
        if (presupuesto == null) return NotFound();

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, TipoTransaccion.Gasto);

        var model = new PresupuestoFormViewModel
        {
            Id = presupuesto.Id,
            Mes = presupuesto.Mes,
            Anio = presupuesto.Anio,
            MontoLimite = presupuesto.MontoLimite,
            CategoriaId = presupuesto.CategoriaId,
            CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", presupuesto.CategoriaId)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PresupuestoFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (id != model.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            var presupuesto = await _context.Presupuestos.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == user.Id);
            if (presupuesto == null) return NotFound();

            presupuesto.Mes = model.Mes;
            presupuesto.Anio = model.Anio;
            presupuesto.MontoLimite = model.MontoLimite;
            presupuesto.CategoriaId = model.CategoriaId;

            _context.Update(presupuesto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Presupuesto actualizado.";
            return RedirectToAction(nameof(Index), new { mes = model.Mes, anio = model.Anio });
        }

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, TipoTransaccion.Gasto);
        model.CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", model.CategoriaId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var presupuesto = await _context.Presupuestos.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == user.Id);
        if (presupuesto != null)
        {
            var mes = presupuesto.Mes;
            var anio = presupuesto.Anio;

            _context.Presupuestos.Remove(presupuesto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Presupuesto eliminado.";
            return RedirectToAction(nameof(Index), new { mes, anio });
        }

        return RedirectToAction(nameof(Index));
    }
}
