package pe.antichihuan.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import pe.antichihuan.model.RecomendacionIA;
import java.util.List;

public interface RecomendacionIARepository extends JpaRepository<RecomendacionIA, Long> {
    List<RecomendacionIA> findTop10ByUsuarioIdOrderByFechaGeneracionDesc(Long usuarioId);
    List<RecomendacionIA> findTop4ByUsuarioIdOrderByFechaGeneracionDesc(Long usuarioId);
}
