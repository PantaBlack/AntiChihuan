package pe.antichihuan.service;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestClient;
import pe.antichihuan.dto.DashboardDto;
import pe.antichihuan.dto.DashboardDto.IAConsejoDto;
import pe.antichihuan.model.*;
import pe.antichihuan.repository.RecomendacionIARepository;
import pe.antichihuan.repository.UsuarioRepository;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Service
public class GeminiAIService {

    private static final Logger log = LoggerFactory.getLogger(GeminiAIService.class);

    private final FinanceService financeService;
    private final UsuarioRepository usuarioRepository;
    private final RecomendacionIARepository recomendacionIARepository;
    private final RestClient restClient;
    private final ObjectMapper objectMapper;

    @Value("${app.gemini.api-key:}")
    private String geminiApiKey;

    @Value("${app.gemini.model:gemini-2.5-flash}")
    private String geminiModel;

    public GeminiAIService(
            FinanceService financeService,
            UsuarioRepository usuarioRepository,
            RecomendacionIARepository recomendacionIARepository,
            RestClient restClient,
            ObjectMapper objectMapper) {
        this.financeService = financeService;
        this.usuarioRepository = usuarioRepository;
        this.recomendacionIARepository = recomendacionIARepository;
        this.restClient = restClient;
        this.objectMapper = objectMapper;
    }

    public IAConsejoDto generarDiagnosticoFinanciero(Long usuarioId) {
        LocalDate hoy = LocalDate.now();
        DashboardDto resumen = financeService.getDashboardData(usuarioId, hoy.getMonthValue(), hoy.getYear());
        Usuario user = usuarioRepository.findById(usuarioId).orElseThrow();

        IAConsejoDto resultado;
        if (geminiApiKey != null && !geminiApiKey.isBlank() && !geminiApiKey.equals("TU_API_KEY_AQUI")) {
            resultado = llamarGeminiApi(resumen);
        } else {
            resultado = generarDiagnosticoHeuristico(resumen);
        }

        // Persistir recomendación en la base de datos para historial del usuario
        try {
            NivelPrioridad prioridad = switch (resultado.nivelRiesgoChihuan) {
                case "Crítico", "Critica" -> NivelPrioridad.CRITICA;
                case "Alto", "Alta" -> NivelPrioridad.ALTA;
                case "Moderado", "Medio" -> NivelPrioridad.MEDIA;
                default -> NivelPrioridad.BAJA;
            };

            String primerConsejo = resultado.recomendacionesAhorro.isEmpty()
                    ? "Continúa registrando tus movimientos."
                    : resultado.recomendacionesAhorro.get(0);

            RecomendacionIA rec = new RecomendacionIA();
            rec.setUsuario(user);
            rec.setTitulo("Diagnóstico IA: Riesgo " + resultado.nivelRiesgoChihuan);
            rec.setMensaje(resultado.analisisGeneral + " Consejo clave: " + primerConsejo);
            rec.setPrioridad(prioridad);
            rec.setCategoriaAnalizada(resumen.gastosPorCategoria.isEmpty()
                    ? "General"
                    : resumen.gastosPorCategoria.get(0).categoria);
            rec.setFechaGeneracion(LocalDateTime.now());
            recomendacionIARepository.save(rec);
        } catch (Exception ex) {
            log.error("Error guardando recomendación IA en base de datos", ex);
        }

        return resultado;
    }

    private IAConsejoDto llamarGeminiApi(DashboardDto resumen) {
        try {
            String url = String.format(
                    "https://generativelanguage.googleapis.com/v1beta/models/%s:generateContent?key=%s",
                    geminiModel, geminiApiKey);

            String catString = resumen.gastosPorCategoria.stream()
                    .map(c -> c.categoria + ": S/ " + c.monto)
                    .collect(Collectors.joining(", "));

            String prompt = String.format("""
                    Eres el Agente de Inteligencia Artificial Financiera del sistema 'AntiChihuan'.
                    Tu objetivo es aconsejar a personas para que no se queden sin dinero ('chihuán'),
                    optimicen sus gastos, eliminen gastos hormiga y aumenten su capacidad de ahorro.

                    Datos del usuario %s este mes:
                    - Ingresos totales: S/ %.2f
                    - Gastos totales: S/ %.2f
                    - Gastos Hormiga identificados: S/ %.2f
                    - Distribución por categoría: %s

                    Devuelve ÚNICAMENTE un objeto JSON válido con la estructura exacta:
                    {
                      "analisisGeneral": "resumen conciso del estado de sus finanzas",
                      "nivelRiesgoChihuan": "Bajo" | "Moderado" | "Alto" | "Crítico",
                      "gastosInnecesariosDetectados": ["detalle 1"],
                      "recomendacionesAhorro": ["consejo 1", "consejo 2"],
                      "alertasPresupuesto": ["alerta 1"],
                      "estimadoAhorroMensualSugerido": 500.00
                    }
                    """,
                    resumen.usuarioNombre,
                    resumen.totalIngresosMes,
                    resumen.totalGastosMes,
                    resumen.totalGastosHormigaMes,
                    catString);

            Map<String, Object> body = Map.of(
                    "contents", List.of(Map.of("parts", List.of(Map.of("text", prompt)))),
                    "generationConfig", Map.of("temperature", 0.4, "responseMimeType", "application/json")
            );

            String responseJson = restClient.post()
                    .uri(url)
                    .contentType(MediaType.APPLICATION_JSON)
                    .body(body)
                    .retrieve()
                    .body(String.class);

            JsonNode root = objectMapper.readTree(responseJson);
            String rawText = root.path("candidates").get(0)
                    .path("content").path("parts").get(0)
                    .path("text").asText();

            IAConsejoDto dto = objectMapper.readValue(rawText.trim(), IAConsejoDto.class);
            dto.fechaAnalisis = LocalDateTime.now();
            return dto;
        } catch (Exception ex) {
            log.warn("Falló consulta con Google Gemini API, recurriendo al Motor Heurístico local.", ex);
            return generarDiagnosticoHeuristico(resumen);
        }
    }

    private IAConsejoDto generarDiagnosticoHeuristico(DashboardDto resumen) {
        IAConsejoDto res = new IAConsejoDto();
        BigDecimal ingresos = resumen.totalIngresosMes;
        BigDecimal gastos = resumen.totalGastosMes;
        BigDecimal hormiga = resumen.totalGastosHormigaMes;
        BigDecimal balance = ingresos.subtract(gastos);

        double pctGasto = ingresos.compareTo(BigDecimal.ZERO) > 0
                ? gastos.multiply(BigDecimal.valueOf(100)).divide(ingresos, 1, RoundingMode.HALF_UP).doubleValue()
                : 100.0;

        if (gastos.compareTo(ingresos) > 0 && ingresos.compareTo(BigDecimal.ZERO) > 0) {
            res.nivelRiesgoChihuan = "Crítico";
            res.analisisGeneral = String.format(
                    "¡Atención urgente! Has gastado S/ %.2f habiendo ingresado S/ %.2f, dejando un déficit de S/ %.2f.",
                    gastos, ingresos, balance.abs());
            res.alertasPresupuesto.add(String.format("Tus egresos superan en un %.1f%% a tus ingresos.", pctGasto - 100));
            res.recomendacionesAhorro.add("Congela compras no esenciales (ropa, salidas, suscripciones de ocio) de inmediato.");
            res.recomendacionesAhorro.add("Aplica la regla de emergencia: prioriza únicamente alimentación, transporte y servicios vitales.");
        } else if (pctGasto >= 90.0) {
            res.nivelRiesgoChihuan = "Alto";
            res.analisisGeneral = String.format(
                    "Tus gastos representan el %.1f%% de tus ingresos. Tu colchón financiero es de apenas S/ %.2f.",
                    pctGasto, balance);
            res.alertasPresupuesto.add("Margen de ahorro menor al 10%.");
            res.recomendacionesAhorro.add("Establece un límite semanal por categoría para evitar quedarte corto a fin de mes.");
        } else if (pctGasto >= 75.0) {
            res.nivelRiesgoChihuan = "Moderado";
            res.analisisGeneral = String.format(
                    "Situación estable con margen de mejora. Gastas el %.1f%% de tus ingresos y ahorras S/ %.2f.",
                    pctGasto, balance);
            res.recomendacionesAhorro.add("Apunta a ahorrar al menos el 20% de tus ingresos cada mes (regla 50/30/20).");
        } else {
            res.nivelRiesgoChihuan = "Bajo";
            res.analisisGeneral = String.format(
                    "¡Excelente disciplina financiera! Gastas el %.1f%% de tus ingresos y mantienes un superávit de S/ %.2f.",
                    pctGasto, balance);
            res.recomendacionesAhorro.add("Destina tus ahorros a un fondo de emergencia o depósito a plazo fijo.");
        }

        if (hormiga.compareTo(BigDecimal.ZERO) > 0) {
            res.gastosInnecesariosDetectados.add(String.format(
                    "Gastos Hormiga detectados: S/ %.2f (%.1f%% de tus gastos). Reducirlos a la mitad te ahorraría S/ %.2f/mes.",
                    hormiga, resumen.porcentajeGastosHormiga, hormiga.divide(BigDecimal.valueOf(2), 2, RoundingMode.HALF_UP)));
        }

        res.estimadoAhorroMensualSugerido = ingresos.multiply(BigDecimal.valueOf(0.15))
                .max(BigDecimal.valueOf(50)).setScale(2, RoundingMode.HALF_UP);
        return res;
    }
}
