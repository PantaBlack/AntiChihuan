package pe.antichihuan.controller;

import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.dto.DashboardDto.TransaccionRequest;
import pe.antichihuan.model.*;
import pe.antichihuan.repository.*;

import java.math.BigDecimal;
import java.nio.charset.StandardCharsets;
import java.time.LocalDate;
import java.util.List;
import java.util.Locale;
import java.util.stream.Collectors;

@RestController
@RequestMapping("/api/transacciones")
public class TransaccionController {

    private final TransaccionRepository transaccionRepository;
    private final CategoriaRepository categoriaRepository;
    private final UsuarioRepository usuarioRepository;

    public TransaccionController(
            TransaccionRepository transaccionRepository,
            CategoriaRepository categoriaRepository,
            UsuarioRepository usuarioRepository) {
        this.transaccionRepository = transaccionRepository;
        this.categoriaRepository = categoriaRepository;
        this.usuarioRepository = usuarioRepository;
    }

    @GetMapping
    public List<Transaccion> listar(
            Authentication auth,
            @RequestParam(required = false) Integer mes,
            @RequestParam(required = false) Integer anio,
            @RequestParam(required = false) String tipo,
            @RequestParam(required = false) Long categoriaId,
            @RequestParam(required = false) Boolean soloHormiga) {

        Long usuarioId = (Long) auth.getPrincipal();
        return transaccionRepository.findByUsuarioIdOrderByFechaDescIdDesc(usuarioId).stream()
                .filter(t -> mes == null || mes <= 0 || t.getFecha().getMonthValue() == mes)
                .filter(t -> anio == null || anio <= 0 || t.getFecha().getYear() == anio)
                .filter(t -> tipo == null || tipo.isBlank() || t.getTipo().name().equalsIgnoreCase(tipo))
                .filter(t -> categoriaId == null || categoriaId <= 0 || (t.getCategoria() != null && t.getCategoria().getId().equals(categoriaId)))
                .filter(t -> soloHormiga == null || !soloHormiga || t.isEsGastoHormiga())
                .collect(Collectors.toList());
    }

    @PostMapping
    public ResponseEntity<Transaccion> crear(Authentication auth, @RequestBody TransaccionRequest req) {
        Long usuarioId = (Long) auth.getPrincipal();
        Usuario u = usuarioRepository.findById(usuarioId).orElseThrow();
        Categoria cat = categoriaRepository.findById(req.categoriaId).orElseThrow();

        Transaccion t = new Transaccion();
        t.setDescripcion(req.descripcion);
        t.setMonto(req.monto);
        t.setFecha(req.fecha != null ? req.fecha : LocalDate.now());
        t.setTipo(TipoTransaccion.valueOf(req.tipo.toUpperCase()));
        t.setEsGastoHormiga(req.esGastoHormiga);
        t.setNotas(req.notas);
        t.setCategoria(cat);
        t.setUsuario(u);

        return ResponseEntity.ok(transaccionRepository.save(t));
    }

    @PutMapping("/{id}")
    public ResponseEntity<Transaccion> editar(Authentication auth, @PathVariable Long id, @RequestBody TransaccionRequest req) {
        Long usuarioId = (Long) auth.getPrincipal();
        Transaccion t = transaccionRepository.findByIdAndUsuarioId(id, usuarioId).orElseThrow();
        Categoria cat = categoriaRepository.findById(req.categoriaId).orElseThrow();

        t.setDescripcion(req.descripcion);
        t.setMonto(req.monto);
        t.setFecha(req.fecha);
        t.setTipo(TipoTransaccion.valueOf(req.tipo.toUpperCase()));
        t.setEsGastoHormiga(req.esGastoHormiga);
        t.setNotas(req.notas);
        t.setCategoria(cat);

        return ResponseEntity.ok(transaccionRepository.save(t));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> eliminar(Authentication auth, @PathVariable Long id) {
        Long usuarioId = (Long) auth.getPrincipal();
        transaccionRepository.findByIdAndUsuarioId(id, usuarioId)
                .ifPresent(transaccionRepository::delete);
        return ResponseEntity.noContent().build();
    }

    @GetMapping("/exportar-csv")
    public ResponseEntity<byte[]> exportarCsv(Authentication auth) {
        Long usuarioId = (Long) auth.getPrincipal();
        List<Transaccion> lista = transaccionRepository.findByUsuarioIdOrderByFechaDescIdDesc(usuarioId);

        // BOM UTF-8 nativo (\uFEFF) para que Excel reconozca tildes y caracteres en español sin alterar la codificación
        StringBuilder sb = new StringBuilder();
        sb.append("\uFEFF");
        sb.append("ID;Fecha;Tipo de Movimiento;Categoría;Descripción;Monto (S/);Es Gasto Hormiga;Notas\r\n");

        BigDecimal totalIngresos = BigDecimal.ZERO;
        BigDecimal totalGastos = BigDecimal.ZERO;
        BigDecimal totalHormiga = BigDecimal.ZERO;

        for (Transaccion t : lista) {
            String tipo = t.getTipo() == TipoTransaccion.INGRESO ? "INGRESO" : "GASTO";
            String cat = t.getCategoria() != null ? t.getCategoria().getNombre() : "General";
            String desc = t.getDescripcion() != null ? t.getDescripcion() : "";
            String esHormiga = t.isEsGastoHormiga() ? "Sí (Gasto Hormiga)" : "No";
            String notas = (t.getNotas() != null && !t.getNotas().trim().isEmpty()) ? t.getNotas().trim() : "-";
            String montoStr = String.format(Locale.US, "%.2f", t.getMonto());

            if (t.getTipo() == TipoTransaccion.INGRESO) {
                totalIngresos = totalIngresos.add(t.getMonto());
            } else {
                totalGastos = totalGastos.add(t.getMonto());
                if (t.isEsGastoHormiga()) {
                    totalHormiga = totalHormiga.add(t.getMonto());
                }
            }

            sb.append(t.getId()).append(";")
              .append(t.getFecha()).append(";")
              .append(tipo).append(";")
              .append(escaparCsv(cat)).append(";")
              .append(escaparCsv(desc)).append(";")
              .append(montoStr).append(";")
              .append(escaparCsv(esHormiga)).append(";")
              .append(escaparCsv(notas)).append("\r\n");
        }

        // Fila en blanco y Cuadro de Resumen Financiero ordenado y alineado al pie
        BigDecimal balanceNeto = totalIngresos.subtract(totalGastos);
        sb.append("\r\n");
        sb.append(";;;CUADRO DE RESUMEN FINANCIERO;VALOR TOTAL (S/);;;\r\n");
        sb.append(";;;TOTAL INGRESOS (+);").append(String.format(Locale.US, "%.2f", totalIngresos)).append(";;;\r\n");
        sb.append(";;;TOTAL GASTOS (-);").append(String.format(Locale.US, "%.2f", totalGastos)).append(";;;\r\n");
        sb.append(";;;TOTAL FUGAS HORMIGA;").append(String.format(Locale.US, "%.2f", totalHormiga)).append(";;;\r\n");
        sb.append(";;;BALANCE DISPONIBLE NETO;").append(String.format(Locale.US, "%.2f", balanceNeto)).append(";;;\r\n");

        byte[] bytes = sb.toString().getBytes(StandardCharsets.UTF_8);
        return ResponseEntity.ok()
                .header(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"Transacciones_AntiChihuan.csv\"")
                .contentType(MediaType.parseMediaType("text/csv; charset=UTF-8"))
                .body(bytes);
    }

    private String escaparCsv(String valor) {
        if (valor == null || valor.trim().isEmpty()) {
            return "-";
        }
        String texto = valor.trim();
        if (texto.contains(";") || texto.contains("\"") || texto.contains("\n") || texto.contains("\r")) {
            return "\"" + texto.replace("\"", "\"\"") + "\"";
        }
        return texto;
    }
}
