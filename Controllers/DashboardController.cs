using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NoMasChihuan.Models;
using NoMasChihuan.Services;

namespace NoMasChihuan.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IFinanceService _financeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IFinanceService financeService,
        UserManager<ApplicationUser> userManager)
    {
        _financeService = financeService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? mes = null, int? anio = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var dashboardData = await _financeService.GetDashboardDataAsync(user.Id, mes, anio);
        return View(dashboardData);
    }
}
