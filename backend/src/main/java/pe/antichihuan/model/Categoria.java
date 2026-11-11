package pe.antichihuan.model;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

@Entity
@Table(name = "categorias")
public class Categoria {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @NotBlank(message = "El nombre de la categoría es obligatorio.")
    @Size(max = 60)
    @Column(nullable = false, length = 60)
    private String nombre;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false, length = 20)
    private TipoTransaccion tipo = TipoTransaccion.GASTO;

    @Column(length = 50)
    private String icono = "bi-tag";

    @Column(length = 20)
    private String colorHex = "#0d6efd";

    @Column(nullable = false)
    private boolean esPredeterminada = false;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "usuario_id")
    @JsonIgnore
    private Usuario usuario;

    public Categoria() {}

    public Categoria(String nombre, TipoTransaccion tipo, String icono, String colorHex, boolean esPredeterminada, Usuario usuario) {
        this.nombre = nombre;
        this.tipo = tipo;
        this.icono = icono;
        this.colorHex = colorHex;
        this.esPredeterminada = esPredeterminada;
        this.usuario = usuario;
    }

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }
    public TipoTransaccion getTipo() { return tipo; }
    public void setTipo(TipoTransaccion tipo) { this.tipo = tipo; }
    public String getIcono() { return icono; }
    public void setIcono(String icono) { this.icono = icono; }
    public String getColorHex() { return colorHex; }
    public void setColorHex(String colorHex) { this.colorHex = colorHex; }
    public boolean isEsPredeterminada() { return esPredeterminada; }
    public void setEsPredeterminada(boolean esPredeterminada) { this.esPredeterminada = esPredeterminada; }
    public Usuario getUsuario() { return usuario; }
    public void setUsuario(Usuario usuario) { this.usuario = usuario; }
}
