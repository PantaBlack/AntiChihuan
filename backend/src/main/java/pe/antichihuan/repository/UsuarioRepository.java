package pe.antichihuan.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import pe.antichihuan.model.Usuario;
import java.util.Optional;

public interface UsuarioRepository extends JpaRepository<Usuario, Long> {
    Optional<Usuario> findByEmailIgnoreCase(String email);
    boolean existsByEmailIgnoreCase(String email);
}
