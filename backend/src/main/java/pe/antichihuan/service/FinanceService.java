package pe.antichihuan.service;

import org.springframework.stereotype.Service;
import pe.antichihuan.dto.DashboardDto;
import pe.antichihuan.dto.DashboardDto.CategoriaGastoItem;
import pe.antichihuan.dto.DashboardDto.MesHistoricoItem;
import pe.antichihuan.model.*;
import pe.antichihuan.repository.*;

import java.math.BigDecimal;
import java.math.RoundingMode;
import java.time.LocalDate;
import java.time.YearMonth;
import java.time.format.TextStyle;
import java.util.*;
import java.util.stream.Collectors;

@Service
public class FinanceService {

    private final UsuarioRepository usuarioRepository;
    private final TransaccionRepository transaccionRepository;
    private final PresupuestoRepository presupuestoRepository;
    private final RecomendacionIARepository recomendacionIARepository;

    public FinanceService(
            UsuarioRepository usuarioRepository,
            TransaccionRepository transaccionRepository,
            PresupuestoRepository presupuestoRepository,
            RecomendacionIARepository recomendacionIARepository) {
        this.usuarioRepository = usuarioRepository;
        this.transaccionRepository = transaccionRepository;
        this.presupuestoRepository = presupuestoRepository;
        this.recomendacionIARepository = recomendacionIARepository;
    }

    public DashboardDto getDashboardData(Long usuarioId, Integer mes, Integer anio) {
        LocalDate hoy = LocalDate.now();
        int m = (mes != null && mes >= 1 && mes <= 12) ? mes : hoy.getMonthValue();
        int a = (anio != null && anio >= 2020) ? anio : hoy.getYear();

        Locale esPe = Locale.forLanguageTag("es-PE");
        YearMonth ym = YearMonth.of(a, m);
        String nombreMes = ym.getMonth().getDisplayName(TextStyle.FULL, esPe);
        nombreMes = nombreMes.substring(0, 1).toUpperCase() + nombreMes.substring(1);

        Usuario user = usuarioRepository.findById(usuarioId).orElseThrow();

        List<Transaccion> transaccionesMes = transaccionRepository
                .findByUsuarioIdAndFechaBetweenOrderByFechaDescIdDesc(usuarioId, ym.atDay(1), ym.atEndOfMonth());

        BigDecimal totalIngresos = transaccionesMes.stream()
                .filter(t -> t.getTipo() == TipoTransaccion.INGRESO)
                .map(Transaccion::getMonto)
                .reduce(BigDecimal.ZERO, BigDecimal::add);

        BigDecimal totalGastos = transaccionesMes.stream()
                .filter(t -> t.getTipo() == TipoTransaccion.GASTO)
                .map(Transaccion::getMonto)
                .reduce(BigDecimal.ZERO, BigDecimal::add);

        BigDecimal totalHormiga = transaccionesMes.stream()
                .filter(t -> t.getTipo() == TipoTransaccion.GASTO &&
                        (t.isEsGastoHormiga() || (t.getCategoria() != null && t.getCategoria().getNombre().contains("Hormiga"))))
                .map(Transaccion::getMonto)
                .reduce(BigDecimal.ZERO, BigDecimal::add);

        List<Presupuesto> presupuestosMes = presupuestoRepository.findByUsuarioIdAndMesAndAnio(usuarioId, m, a);
        BigDecimal presupuestoTotal = presupuestosMes.stream()
                .filter(p -> p.getCategoria() == null)
                .map(Presupuesto::getMontoLimite)
                .findFirst()
                .orElseGet(() -> presupuestosMes.stream()
                        .map(Presupuesto::getMontoLimite)
                        .reduce(BigDecimal.ZERO, BigDecimal::add));

        double pctConsumido = presupuestoTotal.compareTo(BigDecimal.ZERO) > 0
                ? totalGastos.multiply(BigDecimal.valueOf(100)).divide(presupuestoTotal, 2, RoundingMode.HALF_UP).doubleValue()
                : 0.0;

        // Algoritmo del Semáforo Financiero AntiChihuan
        String estado = "EXCELENTE";
        String mensaje = "Tus finanzas van por excelente camino. ¡Sigue así!";

        if (totalGastos.compareTo(totalIngresos) > 0 && totalIngresos.compareTo(BigDecimal.ZERO) > 0) {
            estado = "CHIHUAN";
            mensaje = "¡Alerta Roja Chihuán! Estás gastando más de lo que ganas este mes.";
        } else if (presupuestoTotal.compareTo(BigDecimal.ZERO) > 0 && totalGastos.compareTo(presupuestoTotal) > 0) {
            estado = "CHIHUAN";
            mensaje = "¡Excediste tu presupuesto mensual! Corta gastos no esenciales de inmediato.";
        } else if (presupuestoTotal.compareTo(BigDecimal.ZERO) > 0 && pctConsumido >= 85.0) {
            estado = "ALERTA";
            mensaje = String.format("¡Cuidado! Ya consumiste el %.1f%% de tu presupuesto mensual.", pctConsumido);
        } else if (totalIngresos.compareTo(BigDecimal.ZERO) > 0) {
            double ratioAhorro = totalIngresos.subtract(totalGastos)
                    .divide(totalIngresos, 4, RoundingMode.HALF_UP).doubleValue();
            if (ratioAhorro < 0.10) {
                estado = "ALERTA";
                mensaje = "Margen de ahorro muy bajo (menos del 10%). Evita gastos innecesarios.";
            } else if (ratioAhorro < 0.20) {
                estado = "ACEPTABLE";
                mensaje = "Finanzas estables, pero puedes optimizar tus gastos hormiga para ahorrar más.";
            }
        }

        // Gastos agrupados por categoría (para gráfico de dona en Angular + Chart.js)
        Map<Categoria, BigDecimal> agrupado = transaccionesMes.stream()
                .filter(t -> t.getTipo() == TipoTransaccion.GASTO && t.getCategoria() != null)
                .collect(Collectors.groupingBy(
                        Transaccion::getCategoria,
                        Collectors.reducing(BigDecimal.ZERO, Transaccion::getMonto, BigDecimal::add)
                ));

        List<CategoriaGastoItem> gastosPorCat = agrupado.entrySet().stream()
                .map(entry -> {
                    CategoriaGastoItem item = new CategoriaGastoItem();
                    item.categoria = entry.getKey().getNombre();
                    item.monto = entry.getValue();
                    item.colorHex = entry.getKey().getColorHex();
                    item.icono = entry.getKey().getIcono();
                    item.porcentaje = totalGastos.compareTo(BigDecimal.ZERO) > 0
                            ? entry.getValue().multiply(BigDecimal.valueOf(100)).divide(totalGastos, 1, RoundingMode.HALF_UP).doubleValue()
                            : 0.0;
                    return item;
                })
                .sorted((a1, b1) -> b1.monto.compareTo(a1.monto))
                .collect(Collectors.toList());

        // Histórico últimos 6 meses (para gráfico de barras)
        List<MesHistoricoItem> historico = new ArrayList<>();
        for (int i = 5; i >= 0; i--) {
            YearMonth ymHist = YearMonth.now().minusMonths(i);
            List<Transaccion> tHist = transaccionRepository.findByUsuarioIdAndFechaBetweenOrderByFechaDescIdDesc(
                    usuarioId, ymHist.atDay(1), ymHist.atEndOfMonth());

            MesHistoricoItem h = new MesHistoricoItem();
            h.mesNombre = ymHist.getMonth().getDisplayName(TextStyle.SHORT, esPe) + " " + ymHist.getYear();
            h.ingresos = tHist.stream().filter(t -> t.getTipo() == TipoTransaccion.INGRESO)
                    .map(Transaccion::getMonto).reduce(BigDecimal.ZERO, BigDecimal::add);
            h.gastos = tHist.stream().filter(t -> t.getTipo() == TipoTransaccion.GASTO)
                    .map(Transaccion::getMonto).reduce(BigDecimal.ZERO, BigDecimal::add);
            historico.add(h);
        }

        DashboardDto dto = new DashboardDto();
        dto.usuarioNombre = user.getNombreCompleto();
        dto.mesActual = m;
        dto.anioActual = a;
        dto.nombreMesActual = nombreMes;
        dto.totalIngresosMes = totalIngresos;
        dto.totalGastosMes = totalGastos;
        dto.balanceMes = totalIngresos.subtract(totalGastos);
        dto.presupuestoMes = presupuestoTotal;
        dto.porcentajePresupuestoConsumido = pctConsumido;
        dto.totalGastosHormigaMes = totalHormiga;
        dto.porcentajeGastosHormiga = totalGastos.compareTo(BigDecimal.ZERO) > 0
                ? totalHormiga.multiply(BigDecimal.valueOf(100)).divide(totalGastos, 1, RoundingMode.HALF_UP).doubleValue()
                : 0.0;
        dto.estadoSemaforo = estado;
        dto.mensajeSemaforo = mensaje;
        dto.ultimasTransacciones = transaccionRepository.findByUsuarioIdOrderByFechaDescIdDesc(usuarioId)
                .stream().limit(8).collect(Collectors.toList());
        dto.gastosPorCategoria = gastosPorCat;
        dto.historicoUltimosMeses = historico;
        dto.alertasIA = recomendacionIARepository.findTop4ByUsuarioIdOrderByFechaGeneracionDesc(usuarioId);

        return dto;
    }
}
