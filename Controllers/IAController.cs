using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Data;
using NoMasChihuan.Models;
using NoMasChihuan.Models.ViewModels;
using NoMasChihuan.Services;

namespace NoMasChihuan.Controllers;

[Authorize]
public class IAController : Controller
{
    private readonly IAIService _aiService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IAController(
        IAIService aiService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _aiService = aiService;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var now = DateTime.Today;
        var transaccionesMes = await _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == user.Id && t.Fecha.Month == now.Month && t.Fecha.Year == now.Year)
            .ToListAsync();

        var totalIngresos = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto);
        var totalGastos = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto);
        var totalHormiga = transaccionesMes
            .Where(t => t.Tipo == TipoTransaccion.Gasto && (t.EsGastoHormiga || (t.Categoria != null && t.Categoria.Nombre.Contains("Hormiga"))))
            .Sum(t => t.Monto);

        var historial = await _context.RecomendacionesIA
            .Where(r => r.UsuarioId == user.Id)
            .OrderByDescending(r => r.FechaGeneracion)
            .Take(10)
            .ToListAsync();

        var viewModel = new IAPanelViewModel
        {
            TotalIngresosMes = totalIngresos,
            TotalGastosMes = totalGastos,
            TotalGastosHormiga = totalHormiga,
            HistorialRecomendaciones = historial,
            HayDatosSuficientes = transaccionesMes.Any()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerarDiagnostico()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var diagnostico = await _aiService.GenerarDiagnosticoFinancieroAsync(user.Id);

        var now = DateTime.Today;
        var transaccionesMes = await _context.Transacciones
            .Include(t => t.Categoria)
            .Where(t => t.UsuarioId == user.Id && t.Fecha.Month == now.Month && t.Fecha.Year == now.Year)
            .ToListAsync();

        var historial = await _context.RecomendacionesIA
            .Where(r => r.UsuarioId == user.Id)
            .OrderByDescending(r => r.FechaGeneracion)
            .Take(10)
            .ToListAsync();

        var viewModel = new IAPanelViewModel
        {
            UltimoDiagnostico = diagnostico,
            TotalIngresosMes = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto),
            TotalGastosMes = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto),
            TotalGastosHormiga = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Gasto && (t.EsGastoHormiga || (t.Categoria != null && t.Categoria.Nombre.Contains("Hormiga")))).Sum(t => t.Monto),
            HistorialRecomendaciones = historial,
            HayDatosSuficientes = transaccionesMes.Any()
        };

        TempData["SuccessMessage"] = "¡Diagnóstico inteligente generado por el Asistente AntiChihuan!";
        return View("Index", viewModel);
    }
}
