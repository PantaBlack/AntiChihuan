using System.Text;
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
public class TransaccionesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFinanceService _financeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public TransaccionesController(
        ApplicationDbContext context,
        IFinanceService financeService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _financeService = financeService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? mes, int? anio, TipoTransaccion? tipo, int? categoriaId, bool? soloHormiga)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var viewModel = await _financeService.GetTransaccionesFiltradasAsync(user.Id, mes, anio, tipo, categoriaId, soloHormiga);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create(TipoTransaccion? tipo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var defaultTipo = tipo ?? TipoTransaccion.Gasto;
        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, defaultTipo);

        var model = new TransaccionFormViewModel
        {
            Tipo = defaultTipo,
            Fecha = DateTime.Today,
            CategoriasSelectList = new SelectList(categorias, "Id", "Nombre")
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransaccionFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (ModelState.IsValid)
        {
            var transaccion = new Transaccion
            {
                Descripcion = model.Descripcion,
                Monto = model.Monto,
                Fecha = model.Fecha,
                Tipo = model.Tipo,
                EsGastoHormiga = model.EsGastoHormiga,
                Notas = model.Notas,
                CategoriaId = model.CategoriaId,
                UsuarioId = user.Id,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡Transacción registrada exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, model.Tipo);
        model.CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", model.CategoriaId);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var transaccion = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == user.Id);
        if (transaccion == null) return NotFound();

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, transaccion.Tipo);

        var model = new TransaccionFormViewModel
        {
            Id = transaccion.Id,
            Descripcion = transaccion.Descripcion,
            Monto = transaccion.Monto,
            Fecha = transaccion.Fecha,
            Tipo = transaccion.Tipo,
            EsGastoHormiga = transaccion.EsGastoHormiga,
            Notas = transaccion.Notas,
            CategoriaId = transaccion.CategoriaId,
            CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", transaccion.CategoriaId)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TransaccionFormViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        if (id != model.Id) return BadRequest();

        if (ModelState.IsValid)
        {
            var transaccion = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == user.Id);
            if (transaccion == null) return NotFound();

            transaccion.Descripcion = model.Descripcion;
            transaccion.Monto = model.Monto;
            transaccion.Fecha = model.Fecha;
            transaccion.Tipo = model.Tipo;
            transaccion.EsGastoHormiga = model.EsGastoHormiga;
            transaccion.Notas = model.Notas;
            transaccion.CategoriaId = model.CategoriaId;

            _context.Update(transaccion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Transacción actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, model.Tipo);
        model.CategoriasSelectList = new SelectList(categorias, "Id", "Nombre", model.CategoriaId);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var transaccion = await _context.Transacciones.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == user.Id);
        if (transaccion != null)
        {
            _context.Transacciones.Remove(transaccion);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Transacción eliminada.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetCategoriasPorTipo(TipoTransaccion tipo)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var categorias = await _financeService.GetCategoriasDisponiblesAsync(user.Id, tipo);
        return Json(categorias.Select(c => new { id = c.Id, nombre = c.Nombre }));
    }

    [HttpGet]
    public async Task<IActionResult> ExportarCsv()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var transacciones = await _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == user.Id)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("ID,Fecha,Tipo,Categoría,Descripción,Monto,EsGastoHormiga,Notas");

        foreach (var t in transacciones)
        {
            sb.AppendLine($"{t.Id},{t.Fecha:yyyy-MM-dd},{t.Tipo},\"{t.Categoria?.Nombre}\",\"{t.Descripcion.Replace("\"", "\"\"")}\",{t.Monto},{t.EsGastoHormiga},\"{t.Notas?.Replace("\"", "\"\"")}\"");
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv", $"Transacciones_AntiChihuan_{DateTime.Now:yyyyMMdd_HHmm}.csv");
    }
}
