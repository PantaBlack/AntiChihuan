package pe.antichihuan.dto;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import pe.antichihuan.model.RecomendacionIA;
import pe.antichihuan.model.Transaccion;

public class DashboardDto {
    public String usuarioNombre;
    public int mesActual;
    public int anioActual;
    public String nombreMesActual;
    public BigDecimal totalIngresosMes = BigDecimal.ZERO;
    public BigDecimal totalGastosMes = BigDecimal.ZERO;
    public BigDecimal balanceMes = BigDecimal.ZERO;
    public BigDecimal presupuestoMes = BigDecimal.ZERO;
    public double porcentajePresupuestoConsumido;
    public BigDecimal totalGastosHormigaMes = BigDecimal.ZERO;
    public double porcentajeGastosHormiga;
    public String estadoSemaforo; // EXCELENTE, ACEPTABLE, ALERTA, CHIHUAN
    public String mensajeSemaforo;
    public List<Transaccion> ultimasTransacciones = new ArrayList<>();
    public List<CategoriaGastoItem> gastosPorCategoria = new ArrayList<>();
    public List<MesHistoricoItem> historicoUltimosMeses = new ArrayList<>();
    public List<RecomendacionIA> alertasIA = new ArrayList<>();

    public static class CategoriaGastoItem {
        public String categoria;
        public BigDecimal monto;
        public String colorHex;
        public String icono;
        public double porcentaje;
    }

    public static class MesHistoricoItem {
        public String mesNombre;
        public BigDecimal ingresos;
        public BigDecimal gastos;
    }

    public static class IAConsejoDto {
        public String analisisGeneral;
        public String nivelRiesgoChihuan;
        public List<String> gastosInnecesariosDetectados = new ArrayList<>();
        public List<String> recomendacionesAhorro = new ArrayList<>();
        public List<String> alertasPresupuesto = new ArrayList<>();
        public BigDecimal estimadoAhorroMensualSugerido = BigDecimal.ZERO;
        public LocalDateTime fechaAnalisis = LocalDateTime.now();
    }

    public static class TransaccionRequest {
        public String descripcion;
        public BigDecimal monto;
        public LocalDate fecha;
        public String tipo; // INGRESO o GASTO
        public boolean esGastoHormiga;
        public String notas;
        public Long categoriaId;
    }

    public static class PresupuestoRequest {
        public int mes;
        public int anio;
        public BigDecimal montoLimite;
        public Long categoriaId;
    }
}
