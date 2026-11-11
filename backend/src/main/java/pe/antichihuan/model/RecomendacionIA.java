package pe.antichihuan.model;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "recomendaciones_ia")
public class RecomendacionIA {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 100)
    private String titulo;

    @Column(nullable = false, length = 2000)
    private String mensaje;

    @Column(length = 50)
    private String categoriaAnalizada = "General";

    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 20)
    private NivelPrioridad prioridad = NivelPrioridad.MEDIA;

    @Column(nullable = false)
    private boolean leida = false;

    @Column(nullable = false)
    private LocalDateTime fechaGeneracion = LocalDateTime.now();

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "usuario_id", nullable = false)
    @JsonIgnore
    private Usuario usuario;

    public RecomendacionIA() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getTitulo() { return titulo; }
    public void setTitulo(String titulo) { this.titulo = titulo; }
    public String getMensaje() { return mensaje; }
    public void setMensaje(String mensaje) { this.mensaje = mensaje; }
    public String getCategoriaAnalizada() { return categoriaAnalizada; }
    public void setCategoriaAnalizada(String categoriaAnalizada) { this.categoriaAnalizada = categoriaAnalizada; }
    public NivelPrioridad getPrioridad() { return prioridad; }
    public void setPrioridad(NivelPrioridad prioridad) { this.prioridad = prioridad; }
    public boolean isLeida() { return leida; }
    public void setLeida(boolean leida) { this.leida = leida; }
    public LocalDateTime getFechaGeneracion() { return fechaGeneracion; }
    public void setFechaGeneracion(LocalDateTime fechaGeneracion) { this.fechaGeneracion = fechaGeneracion; }
    public Usuario getUsuario() { return usuario; }
    public void setUsuario(Usuario usuario) { this.usuario = usuario; }
}
