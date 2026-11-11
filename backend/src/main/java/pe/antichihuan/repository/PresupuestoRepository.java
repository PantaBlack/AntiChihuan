package pe.antichihuan.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import pe.antichihuan.model.Presupuesto;
import java.util.List;
import java.util.Optional;

public interface PresupuestoRepository extends JpaRepository<Presupuesto, Long> {
    List<Presupuesto> findByUsuarioIdAndMesAndAnio(Long usuarioId, int mes, int anio);
    Optional<Presupuesto> findByIdAndUsuarioId(Long id, Long usuarioId);
}
