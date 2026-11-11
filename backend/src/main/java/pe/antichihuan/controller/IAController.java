package pe.antichihuan.controller;

import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.dto.DashboardDto.IAConsejoDto;
import pe.antichihuan.model.RecomendacionIA;
import pe.antichihuan.repository.RecomendacionIARepository;
import pe.antichihuan.service.GeminiAIService;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/ia")
public class IAController {

    private final GeminiAIService geminiAIService;
    private final RecomendacionIARepository recomendacionIARepository;

    public IAController(GeminiAIService geminiAIService, RecomendacionIARepository recomendacionIARepository) {
        this.geminiAIService = geminiAIService;
        this.recomendacionIARepository = recomendacionIARepository;
    }

    @GetMapping("/historial")
    public List<RecomendacionIA> obtenerHistorial(Authentication auth) {
        Long usuarioId = (Long) auth.getPrincipal();
        return recomendacionIARepository.findTop10ByUsuarioIdOrderByFechaGeneracionDesc(usuarioId);
    }

    @PostMapping("/generar-diagnostico")
    public Map<String, Object> generarDiagnostico(Authentication auth) {
        Long usuarioId = (Long) auth.getPrincipal();
        IAConsejoDto diagnostico = geminiAIService.generarDiagnosticoFinanciero(usuarioId);
        List<RecomendacionIA> historial = recomendacionIARepository.findTop10ByUsuarioIdOrderByFechaGeneracionDesc(usuarioId);
        return Map.of(
                "diagnostico", diagnostico,
                "historial", historial
        );
    }
}
