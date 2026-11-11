package pe.antichihuan.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import pe.antichihuan.model.Categoria;
import pe.antichihuan.model.TipoTransaccion;
import java.util.List;

public interface CategoriaRepository extends JpaRepository<Categoria, Long> {

    @Query("SELECT c FROM Categoria c WHERE c.esPredeterminada = true OR c.usuario.id = :usuarioId ORDER BY c.nombre ASC")
    List<Categoria> findDisponiblesByUsuario(@Param("usuarioId") Long usuarioId);

    @Query("SELECT c FROM Categoria c WHERE (c.esPredeterminada = true OR c.usuario.id = :usuarioId) AND c.tipo = :tipo ORDER BY c.nombre ASC")
    List<Categoria> findDisponiblesByUsuarioAndTipo(@Param("usuarioId") Long usuarioId, @Param("tipo") TipoTransaccion tipo);
}
