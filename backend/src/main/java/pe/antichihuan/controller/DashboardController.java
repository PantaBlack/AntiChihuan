package pe.antichihuan.controller;

import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.dto.DashboardDto;
import pe.antichihuan.service.FinanceService;

@RestController
@RequestMapping("/api/dashboard")
public class DashboardController {

    private final FinanceService financeService;

    public DashboardController(FinanceService financeService) {
        this.financeService = financeService;
    }

    @GetMapping
    public DashboardDto getDashboard(
            Authentication auth,
            @RequestParam(required = false) Integer mes,
            @RequestParam(required = false) Integer anio) {
        Long usuarioId = (Long) auth.getPrincipal();
        return financeService.getDashboardData(usuarioId, mes, anio);
    }
}
