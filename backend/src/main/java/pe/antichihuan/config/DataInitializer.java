package pe.antichihuan.config;

import org.springframework.boot.CommandLineRunner;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Component;
import pe.antichihuan.model.*;
import pe.antichihuan.repository.*;

import java.math.BigDecimal;
import java.time.LocalDate;
import java.util.List;

@Component
public class DataInitializer implements CommandLineRunner {

    private final CategoriaRepository categoriaRepository;
    private final UsuarioRepository usuarioRepository;
    private final TransaccionRepository transaccionRepository;
    private final PresupuestoRepository presupuestoRepository;
    private final PasswordEncoder passwordEncoder;

    public DataInitializer(
            CategoriaRepository categoriaRepository,
            UsuarioRepository usuarioRepository,
            TransaccionRepository transaccionRepository,
            PresupuestoRepository presupuestoRepository,
            PasswordEncoder passwordEncoder) {
        this.categoriaRepository = categoriaRepository;
        this.usuarioRepository = usuarioRepository;
        this.transaccionRepository = transaccionRepository;
        this.presupuestoRepository = presupuestoRepository;
        this.passwordEncoder = passwordEncoder;
    }

    @Override
    public void run(String... args) {
        if (categoriaRepository.count() == 0) {
            categoriaRepository.saveAll(List.of(
                new Categoria("Salario / Sueldo", TipoTransaccion.INGRESO, "bi-cash-coin", "#198754", true, null),
                new Categoria("Ventas / Negocio", TipoTransaccion.INGRESO, "bi-shop", "#20c997", true, null),
                new Categoria("Freelance / Proyectos", TipoTransaccion.INGRESO, "bi-laptop", "#0dcaf0", true, null),
                new Categoria("Inversiones", TipoTransaccion.INGRESO, "bi-graph-up-arrow", "#17a2b8", true, null),
                new Categoria("Otros Ingresos", TipoTransaccion.INGRESO, "bi-plus-circle", "#6c757d", true, null),
                new Categoria("Alimentación y Supermercado", TipoTransaccion.GASTO, "bi-cart4", "#fd7e14", true, null),
                new Categoria("Vivienda y Alquiler", TipoTransaccion.GASTO, "bi-house-door", "#dc3545", true, null),
                new Categoria("Servicios (Luz, Agua, Internet)", TipoTransaccion.GASTO, "bi-lightning-charge", "#ffc107", true, null),
                new Categoria("Transporte y Gasolina", TipoTransaccion.GASTO, "bi-car-front", "#6f42c1", true, null),
                new Categoria("Salud y Medicinas", TipoTransaccion.GASTO, "bi-heart-pulse", "#e83e8c", true, null),
                new Categoria("Educación y Cursos", TipoTransaccion.GASTO, "bi-book", "#0d6efd", true, null),
                new Categoria("Entretenimiento y Salidas", TipoTransaccion.GASTO, "bi-controller", "#d63384", true, null),
                new Categoria("Gastos Hormiga (Snacks, Cafés, Antojos)", TipoTransaccion.GASTO, "bi-cup-hot", "#b02a37", true, null),
                new Categoria("Compras Personales y Ropa", TipoTransaccion.GASTO, "bi-bag-check", "#0b5ed7", true, null),
                new Categoria("Otros Gastos", TipoTransaccion.GASTO, "bi-dash-circle", "#6c757d", true, null)
            ));
        }

        if (usuarioRepository.count() == 0) {
            String pwd = passwordEncoder.encode("Password123!");
            List<Categoria> cats = categoriaRepository.findAll();
            Categoria cSueldo = cats.get(0);
            Categoria cComida = cats.get(5);
            Categoria cVivienda = cats.get(6);
            Categoria cHormiga = cats.get(12);

            seedPerfil("Gianino Hector Naranjo Servidio", "gianino@antichihuan.pe", pwd,
                    5000, 1200, 900, 350, 3000, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Alexis Mijael Huerta Usqueano", "alexis@antichihuan.pe", pwd,
                    16500, 3200, 1800, 800, 8000, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Anderson Fabrizzio Larios Tito", "anderson@antichihuan.pe", pwd,
                    2400, 2100, 1200, 620, 2200, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Samir Rojas Diaz", "samir@antichihuan.pe", pwd,
                    3300, 1100, 760, 1320, 3200, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Yimmy Diaz Jange", "yimmy@antichihuan.pe", pwd,
                    3600, 1000, 700, 250, 2500, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Carlos Slim Rodríguez", "carlos.slim@antichihuan.pe", pwd,
                    38000, 6000, 3500, 500, 15000, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Valeria Mendoza Paz", "valeria.deudas@antichihuan.pe", pwd,
                    2200, 2800, 1400, 750, 2000, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Mateo Quispe Flores", "mateo.delivery@antichihuan.pe", pwd,
                    2800, 800, 550, 1450, 2500, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Lucía Fernández Castro", "lucia.frugal@antichihuan.pe", pwd,
                    4200, 900, 520, 80, 2000, cSueldo, cVivienda, cComida, cHormiga);
            seedPerfil("Renzo Balbuena Silva", "renzo.chihuan@antichihuan.pe", pwd,
                    1800, 1900, 950, 790, 1800, cSueldo, cVivienda, cComida, cHormiga);
        }
    }

    private void seedPerfil(String nombre, String email, String pwd,
                            double ingreso, double alquiler, double comida, double hormiga, double limitePresupuesto,
                            Categoria cSueldo, Categoria cVivienda, Categoria cComida, Categoria cHormiga) {
        Usuario u = usuarioRepository.save(new Usuario(nombre, email, pwd));
        LocalDate hoy = LocalDate.now();

        crearTransaccion(u, "Sueldo Mensual Principal", ingreso, hoy.withDayOfMonth(1), TipoTransaccion.INGRESO, false, cSueldo);
        crearTransaccion(u, "Pago de Alquiler y Cuotas", alquiler, hoy.withDayOfMonth(3), TipoTransaccion.GASTO, false, cVivienda);
        crearTransaccion(u, "Supermercado y Alimentación", comida, hoy.withDayOfMonth(5), TipoTransaccion.GASTO, false, cComida);
        crearTransaccion(u, "Deliveries, Cafés y Antojos (Hormiga)", hormiga, hoy.withDayOfMonth(7), TipoTransaccion.GASTO, true, cHormiga);

        Presupuesto p = new Presupuesto();
        p.setUsuario(u);
        p.setMes(hoy.getMonthValue());
        p.setAnio(hoy.getYear());
        p.setMontoLimite(BigDecimal.valueOf(limitePresupuesto));
        presupuestoRepository.save(p);
    }

    private void crearTransaccion(Usuario u, String desc, double monto, LocalDate fecha,
                                  TipoTransaccion tipo, boolean hormiga, Categoria cat) {
        Transaccion t = new Transaccion();
        t.setUsuario(u);
        t.setDescripcion(desc);
        t.setMonto(BigDecimal.valueOf(monto));
        t.setFecha(fecha);
        t.setTipo(tipo);
        t.setEsGastoHormiga(hormiga);
        t.setCategoria(cat);
        transaccionRepository.save(t);
    }
}
