package pe.antichihuan.controller;

import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.dto.DashboardDto.TransaccionRequest;
import pe.antichihuan.model.*;
import pe.antichihuan.repository.*;

import java.nio.charset.StandardCharsets;
import java.time.LocalDate;
import java.util.List;
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

        StringBuilder sb = new StringBuilder("\uFEFFID,Fecha,Tipo,Categoría,Descripción,Monto,EsGastoHormiga,Notas\n");
        for (Transaccion t : lista) {
            sb.append(String.format("%d,%s,%s,\"%s\",\"%s\",%.2f,%s,\"%s\"\n",
                    t.getId(),
                    t.getFecha(),
                    t.getTipo(),
                    t.getCategoria() != null ? t.getCategoria().getNombre() : "General",
                    t.getDescripcion().replace("\"", "\"\""),
                    t.getMonto(),
                    t.isEsGastoHormiga(),
                    t.getNotas() != null ? t.getNotas().replace("\"", "\"\"") : ""));
        }

        byte[] bytes = sb.toString().getBytes(StandardCharsets.UTF_8);
        return ResponseEntity.ok()
                .header(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=\"Transacciones_AntiChihuan.csv\"")
                .contentType(MediaType.parseMediaType("text/csv; charset=UTF-8"))
                .body(bytes);
    }
}
