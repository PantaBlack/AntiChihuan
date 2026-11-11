package pe.antichihuan.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import pe.antichihuan.model.Transaccion;
import java.time.LocalDate;
import java.util.List;
import java.util.Optional;

public interface TransaccionRepository extends JpaRepository<Transaccion, Long> {
    List<Transaccion> findByUsuarioIdOrderByFechaDescIdDesc(Long usuarioId);
    List<Transaccion> findByUsuarioIdAndFechaBetweenOrderByFechaDescIdDesc(Long usuarioId, LocalDate inicio, LocalDate fin);
    Optional<Transaccion> findByIdAndUsuarioId(Long id, Long usuarioId);
    boolean existsByCategoriaId(Long categoriaId);
}
