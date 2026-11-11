package pe.antichihuan.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.dto.DashboardDto.PresupuestoRequest;
import pe.antichihuan.model.Presupuesto;
import pe.antichihuan.model.Usuario;
import pe.antichihuan.repository.CategoriaRepository;
import pe.antichihuan.repository.PresupuestoRepository;
import pe.antichihuan.repository.UsuarioRepository;

import java.time.LocalDate;
import java.util.List;

@RestController
@RequestMapping("/api/presupuestos")
public class PresupuestoController {

    private final PresupuestoRepository presupuestoRepository;
    private final CategoriaRepository categoriaRepository;
    private final UsuarioRepository usuarioRepository;

    public PresupuestoController(
            PresupuestoRepository presupuestoRepository,
            CategoriaRepository categoriaRepository,
            UsuarioRepository usuarioRepository) {
        this.presupuestoRepository = presupuestoRepository;
        this.categoriaRepository = categoriaRepository;
        this.usuarioRepository = usuarioRepository;
    }

    @GetMapping
    public List<Presupuesto> listar(
            Authentication auth,
            @RequestParam(required = false) Integer mes,
            @RequestParam(required = false) Integer anio) {
        Long usuarioId = (Long) auth.getPrincipal();
        LocalDate hoy = LocalDate.now();
        int m = mes != null ? mes : hoy.getMonthValue();
        int a = anio != null ? anio : hoy.getYear();
        return presupuestoRepository.findByUsuarioIdAndMesAndAnio(usuarioId, m, a);
    }

    @PostMapping
    public ResponseEntity<Presupuesto> crear(Authentication auth, @RequestBody PresupuestoRequest req) {
        Long usuarioId = (Long) auth.getPrincipal();
        Usuario u = usuarioRepository.findById(usuarioId).orElseThrow();

        Presupuesto p = new Presupuesto();
        p.setUsuario(u);
        p.setMes(req.mes);
        p.setAnio(req.anio);
        p.setMontoLimite(req.montoLimite);
        if (req.categoriaId != null) {
            p.setCategoria(categoriaRepository.findById(req.categoriaId).orElse(null));
        }

        return ResponseEntity.ok(presupuestoRepository.save(p));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> eliminar(Authentication auth, @PathVariable Long id) {
        Long usuarioId = (Long) auth.getPrincipal();
        presupuestoRepository.findByIdAndUsuarioId(id, usuarioId)
                .ifPresent(presupuestoRepository::delete);
        return ResponseEntity.noContent().build();
    }
}
