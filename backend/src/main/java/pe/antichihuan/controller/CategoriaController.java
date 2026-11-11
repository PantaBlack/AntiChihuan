package pe.antichihuan.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.web.bind.annotation.*;
import pe.antichihuan.model.Categoria;
import pe.antichihuan.model.TipoTransaccion;
import pe.antichihuan.model.Usuario;
import pe.antichihuan.repository.CategoriaRepository;
import pe.antichihuan.repository.TransaccionRepository;
import pe.antichihuan.repository.UsuarioRepository;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/api/categorias")
public class CategoriaController {

    private final CategoriaRepository categoriaRepository;
    private final TransaccionRepository transaccionRepository;
    private final UsuarioRepository usuarioRepository;

    public CategoriaController(
            CategoriaRepository categoriaRepository,
            TransaccionRepository transaccionRepository,
            UsuarioRepository usuarioRepository) {
        this.categoriaRepository = categoriaRepository;
        this.transaccionRepository = transaccionRepository;
        this.usuarioRepository = usuarioRepository;
    }

    @GetMapping
    public List<Categoria> listar(Authentication auth, @RequestParam(required = false) String tipo) {
        Long usuarioId = (Long) auth.getPrincipal();
        if (tipo != null && !tipo.isBlank()) {
            return categoriaRepository.findDisponiblesByUsuarioAndTipo(
                    usuarioId, TipoTransaccion.valueOf(tipo.toUpperCase()));
        }
        return categoriaRepository.findDisponiblesByUsuario(usuarioId);
    }

    @PostMapping
    public ResponseEntity<Categoria> crear(Authentication auth, @RequestBody Categoria req) {
        Long usuarioId = (Long) auth.getPrincipal();
        Usuario u = usuarioRepository.findById(usuarioId).orElseThrow();

        req.setId(null);
        req.setEsPredeterminada(false);
        req.setUsuario(u);
        return ResponseEntity.ok(categoriaRepository.save(req));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<?> eliminar(Authentication auth, @PathVariable Long id) {
        if (transaccionRepository.existsByCategoriaId(id)) {
            return ResponseEntity.badRequest()
                    .body(Map.of("message", "No puedes eliminar una categoría con transacciones asociadas."));
        }
        categoriaRepository.deleteById(id);
        return ResponseEntity.noContent().build();
    }
}
