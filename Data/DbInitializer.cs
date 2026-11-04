using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NoMasChihuan.Models;

namespace NoMasChihuan.Data;

public static class DbInitializer
{
    public static async Task SeedUsersAndFinancesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var hoy = DateTime.Today;
        var mesActual = hoy.Month;
        var anioActual = hoy.Year;

        // Lista de 10 usuarios con perfiles financieros diversos
        var perfiles = new List<(string Email, string Nombre, string Password, string PerfilDesc)>
        {
            // === 5 INTEGRANTES DEL EQUIPO ===
            ("gianino@antichihuan.pe", "Gianino Hector Naranjo Servidio", "Password123!", "BD & Finanzas - Finanzas Equilibradas y Ahorrador"),
            ("alexis@antichihuan.pe", "Alexis Mijael Huerta Usqueano", "Password123!", "IA & Análisis - Alto Patrimonio / Gran Capital"),
            ("anderson@antichihuan.pe", "Anderson Fabrizzio Larios Tito", "Password123!", "Backend & Seguridad - Deudas Severas / Modo Chihuán"),
            ("samir@antichihuan.pe", "Samir Rojas Diaz", "Password123!", "Estadísticas & DevOps - Víctima de Gastos Hormiga masivos"),
            ("yimmy@antichihuan.pe", "Yimmy Diaz Jange", "Password123!", "Frontend & UI - Control Presupuestal Estricto"),

            // === 5 USUARIOS CON PERFILES FINANCIEROS EXTREMOS ===
            ("carlos.slim@antichihuan.pe", "Carlos Slim Rodríguez", "Password123!", "Empresario Millonario - Ingresos Masivos (Mucho Dinero)"),
            ("valeria.deudas@antichihuan.pe", "Valeria Mendoza Paz", "Password123!", "Quiebra Inminente - Cuotas de Tarjetas y Préstamos"),
            ("mateo.delivery@antichihuan.pe", "Mateo Quispe Flores", "Password123!", "Adicto a Delivery y Snacks - 50% Gastos Hormiga"),
            ("lucia.frugal@antichihuan.pe", "Lucía Fernández Castro", "Password123!", "Ahorro Frugal / Minimalista Financiera"),
            ("renzo.chihuan@antichihuan.pe", "Renzo Balbuena Silva", "Password123!", "Descontrol Total / Sobregiro Extremo")
        };

        foreach (var p in perfiles)
        {
            var user = await userManager.FindByEmailAsync(p.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = p.Email,
                    Email = p.Email,
                    NombreCompleto = p.Nombre,
                    EmailConfirmed = true,
                    FechaRegistro = DateTime.UtcNow.AddMonths(-5)
                };

                var res = await userManager.CreateAsync(user, p.Password);
                if (res.Succeeded)
                {
                    await SeedUserDataAsync(context, user.Id, p.Email, hoy);
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedUserDataAsync(ApplicationDbContext context, string userId, string email, DateTime hoy)
    {
        var m = hoy.Month;
        var y = hoy.Year;

        switch (email)
        {
            // 1. GIANINO: Equilibrado y prudente (Ingresos S/ 5,000 | Gastos S/ 2,450 | Ahorro S/ 2,550)
            case "gianino@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 3000m, CategoriaId = null });
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 800m, CategoriaId = 6 }); // Alimentos

                AddHistorico(context, userId, hoy, 4500m, 2300m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Desarrollo Software", Monto = 4200m, Fecha = new DateTime(y, m, 2), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Consultoría BD Freelance", Monto = 800m, Fecha = new DateTime(y, m, 15), CategoriaId = 3 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alquiler departamento", Monto = 1100m, Fecha = new DateTime(y, m, 3), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Supermercado quincenal", Monto = 650m, Fecha = new DateTime(y, m, 5), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Recibos Luz, Agua y Fibra", Monto = 260m, Fecha = new DateTime(y, m, 8), CategoriaId = 8 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Gasolina del mes", Monto = 220m, Fecha = new DateTime(y, m, 12), CategoriaId = 9 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Café y snacks biblioteca", Monto = 45m, Fecha = new DateTime(y, m, 14), CategoriaId = 13, EsGastoHormiga = true }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Diagnóstico IA: Excelente Salud Financiera",
                    Mensaje = "Tus gastos están en 49% de tus ingresos. Mantienes una capacidad de ahorro de S/ 2,550 mensuales. Sigue derivando este excedente a fondos de inversión.",
                    Prioridad = NivelPrioridad.Baja,
                    CategoriaAnalizada = "General"
                });
                break;

            // 2. ALEXIS: Mucho Dinero / Tech Lead (Ingresos S/ 16,500 | Gastos S/ 5,800 | Ahorro S/ 10,700)
            case "alexis@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 8000m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 16000m, 5500m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Salario Senior AI Tech Lead", Monto = 13500m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Rendimientos Dividendos e Inversiones", Monto = 3000m, Fecha = new DateTime(y, m, 10), CategoriaId = 4 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Departamento Miraflores", Monto = 2800m, Fecha = new DateTime(y, m, 2), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Mantenimiento Camioneta y Gasolina", Monto = 850m, Fecha = new DateTime(y, m, 6), CategoriaId = 9 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Cena Ejecutiva & Vinos", Monto = 750m, Fecha = new DateTime(y, m, 11), CategoriaId = 12 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Suscripciones Cloud & APIs OpenAI", Monto = 420m, Fecha = new DateTime(y, m, 13), CategoriaId = 11 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Compras de ropa de marca", Monto = 980m, Fecha = new DateTime(y, m, 18), CategoriaId = 14 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Diagnóstico IA: Gran Capital y Alta Liquidez",
                    Mensaje = "Tienes un superávit mensual de S/ 10,700. Te sugerimos diversificar en activos de renta fija internacional y optimizar la deducción de impuestos corporativos.",
                    Prioridad = NivelPrioridad.Baja,
                    CategoriaAnalizada = "Inversiones"
                });
                break;

            // 3. ANDERSON: Endeudamiento y Crisis (Ingresos S/ 2,400 | Gastos S/ 3,920 | Déficit S/ -1,520) MODO CHIHUÁN
            case "anderson@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 2200m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 2400m, 3600m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Neto de Planilla", Monto = 2400m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Cuota Préstamo Bancario BCP", Monto = 1450m, Fecha = new DateTime(y, m, 3), CategoriaId = 15 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Pago Mínimo Tarjeta de Crédito", Monto = 890m, Fecha = new DateTime(y, m, 4), CategoriaId = 15 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alquiler de cuarto", Monto = 650m, Fecha = new DateTime(y, m, 5), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alimentos y menú del día", Monto = 520m, Fecha = new DateTime(y, m, 10), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Delivery por estrés y snacks", Monto = 180m, Fecha = new DateTime(y, m, 12), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Intereses por sobregiro", Monto = 230m, Fecha = new DateTime(y, m, 16), CategoriaId = 15 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "¡ALERTA ROJA CHIHUÁN! Estado Crítico de Endeudamiento",
                    Mensaje = "Tus deudas e intereses se comen el 65% de tus ingresos totales. Has gastado 163% de lo que ganas. ¡Pide compra de deuda inmediata para consolidar a menor tasa!",
                    Prioridad = NivelPrioridad.Critica,
                    CategoriaAnalizada = "Otros Gastos"
                });
                break;

            // 4. SAMIR: Víctima de Gastos Hormiga (Ingresos S/ 3,300 | Gastos S/ 3,180 | Fuga Hormiga S/ 1,320)
            case "samir@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 2800m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 3300m, 3100m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Analista Estadístico", Monto = 3300m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alquiler compartido", Monto = 850m, Fecha = new DateTime(y, m, 3), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Servicios e Internet", Monto = 190m, Fecha = new DateTime(y, m, 5), CategoriaId = 8 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Compras básicas mercado", Monto = 420m, Fecha = new DateTime(y, m, 7), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Rappi almuerzo con postre", Monto = 185m, Fecha = new DateTime(y, m, 9), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Cafés Starbucks y galletas (x5)", Monto = 160m, Fecha = new DateTime(y, m, 11), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Taxis de aplicativo por flojera", Monto = 210m, Fecha = new DateTime(y, m, 13), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Compras en Grifo y gaseosas", Monto = 145m, Fecha = new DateTime(y, m, 15), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Salidas fines de semana y bebidas", Monto = 620m, Fecha = new DateTime(y, m, 17), CategoriaId = 12 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Golosinas y antojos de noche", Monto = 90m, Fecha = new DateTime(y, m, 19), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Suscripciones no utilizadas", Monto = 110m, Fecha = new DateTime(y, m, 20), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Cigarros y chicles", Monto = 200m, Fecha = new DateTime(y, m, 22), CategoriaId = 13, EsGastoHormiga = true }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Alerta IA: Fuga Extrema de Gastos Hormiga",
                    Mensaje = "¡El 41.5% de tus gastos (S/ 1,320) corresponde a micro-compras no esenciales! Prepara café en casa y elimina deliveries impulsivos para ahorrar S/ 800 este mes.",
                    Prioridad = NivelPrioridad.Alta,
                    CategoriaAnalizada = "Gastos Hormiga"
                });
                break;

            // 5. YIMMY: Presupuesto y Diseño (Ingresos S/ 3,600 | Gastos S/ 1,950 | Ahorro S/ 1,650)
            case "yimmy@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 2300m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 3500m, 1900m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Diseñador Frontend", Monto = 2900m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Diseño de Landing Page freelance", Monto = 700m, Fecha = new DateTime(y, m, 14), CategoriaId = 3 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alquiler y mantenimiento", Monto = 920m, Fecha = new DateTime(y, m, 4), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Supermercado planificado", Monto = 510m, Fecha = new DateTime(y, m, 8), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Servicios digitales e Internet", Monto = 180m, Fecha = new DateTime(y, m, 10), CategoriaId = 8 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Curso Figma & Design Systems", Monto = 220m, Fecha = new DateTime(y, m, 15), CategoriaId = 11 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Café en coworking", Monto = 40m, Fecha = new DateTime(y, m, 18), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Transporte público", Monto = 80m, Fecha = new DateTime(y, m, 20), CategoriaId = 9 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Diagnóstico IA: Excelente Disciplina Presupuestal",
                    Mensaje = "Has consumido solo el 84% de tu presupuesto mensual. Tu margen de ahorro es del 45.8%. Tus finanzas están completamente blindadas.",
                    Prioridad = NivelPrioridad.Baja,
                    CategoriaAnalizada = "General"
                });
                break;

            // 6. CARLOS SLIM: Empresario Millonario (Ingresos S/ 38,000 | Gastos S/ 9,200 | Ahorro S/ 28,800)
            case "carlos.slim@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 15000m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 35000m, 8800m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Dividendos Holding Empresarial", Monto = 26000m, Fecha = new DateTime(y, m, 1), CategoriaId = 4 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Facturación Ventas B2B", Monto = 12000m, Fecha = new DateTime(y, m, 12), CategoriaId = 2 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Hipoteca Casa de Playa & Residencia", Monto = 4500m, Fecha = new DateTime(y, m, 3), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Seguro Vehicular y Mantenimiento BMW", Monto = 1800m, Fecha = new DateTime(y, m, 7), CategoriaId = 9 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Club Campestre y Membresía Golf", Monto = 1400m, Fecha = new DateTime(y, m, 10), CategoriaId = 12 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Supermercado Gourmet y Vinos", Monto = 1500m, Fecha = new DateTime(y, m, 15), CategoriaId = 6 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Diagnóstico IA: Gran Fortaleza Financiera",
                    Mensaje = "Superávit mensual masivo de S/ 28,800. Se recomienda canalizar excedentes hacia bienes raíces comerciales o instrumentos financieros de bajo riesgo.",
                    Prioridad = NivelPrioridad.Baja,
                    CategoriaAnalizada = "Inversiones"
                });
                break;

            // 7. VALERIA: Quiebra inminente / Endeudamiento Total (Ingresos S/ 1,900 | Gastos S/ 4,650 | Déficit S/ -2,750)
            case "valeria.deudas@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 1800m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 1900m, 4200m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Auxiliar Administrativo", Monto = 1900m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Préstamo Personal Interbank", Monto = 1250m, Fecha = new DateTime(y, m, 2), CategoriaId = 15 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Tarjeta CMR Falabella cuota atrasada", Monto = 1100m, Fecha = new DateTime(y, m, 3), CategoriaId = 15 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Préstamo familiar por cobrar", Monto = 750m, Fecha = new DateTime(y, m, 5), CategoriaId = 15 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alquiler mini departamento", Monto = 750m, Fecha = new DateTime(y, m, 6), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Comida y víveres mínimos", Monto = 480m, Fecha = new DateTime(y, m, 10), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Interés moratorio por pago tardío", Monto = 320m, Fecha = new DateTime(y, m, 14), CategoriaId = 15 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "¡COLAPSO FINANCIERO! Gastos duplican tus ingresos",
                    Mensaje = "Tienes un déficit de S/ 2,750 este mes. El 73% de tus pagos van a amortizar deudas. Urge asesoría legal financiera o congelamiento de tarjetas antes de reporte en Infocorp.",
                    Prioridad = NivelPrioridad.Critica,
                    CategoriaAnalizada = "Otros Gastos"
                });
                break;

            // 8. MATEO: Adicto al Delivery (Ingresos S/ 2,800 | Gastos S/ 2,740 | Hormiga S/ 1,420)
            case "mateo.delivery@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 2500m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 2800m, 2600m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Call Center Bilingüe", Monto = 2800m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Habitación compartida", Monto = 600m, Fecha = new DateTime(y, m, 3), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Servicios y plan celular", Monto = 120m, Fecha = new DateTime(y, m, 5), CategoriaId = 8 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Supermercado básico", Monto = 300m, Fecha = new DateTime(y, m, 7), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "PedidosYa Burger King", Monto = 145m, Fecha = new DateTime(y, m, 9), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Rappi Sushi Night", Monto = 210m, Fecha = new DateTime(y, m, 11), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Starbucks frappé diario x6", Monto = 180m, Fecha = new DateTime(y, m, 13), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Gaseosas, papitas y vapes grifo", Monto = 290m, Fecha = new DateTime(y, m, 15), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Uber Eats pizza con amigos", Monto = 230m, Fecha = new DateTime(y, m, 17), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Delivery helados medianoche", Monto = 125m, Fecha = new DateTime(y, m, 19), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Videojuegos Steam y skins", Monto = 240m, Fecha = new DateTime(y, m, 21), CategoriaId = 13, EsGastoHormiga = true },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Salida bar fin de semana", Monto = 300m, Fecha = new DateTime(y, m, 22), CategoriaId = 12 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Alerta IA: Más del 50% de tus gastos son Hormiga",
                    Mensaje = "Gastas S/ 1,420 en apps de comida rápida y compras de impulso. Cocinar 3 días a la semana liberaría más de S/ 700 mensuales de liquidez inmediata.",
                    Prioridad = NivelPrioridad.Alta,
                    CategoriaAnalizada = "Gastos Hormiga"
                });
                break;

            // 9. LUCIA: Ahorro Frugal y Disciplinado (Ingresos S/ 4,500 | Gastos S/ 1,600 | Ahorro S/ 2,900)
            case "lucia.frugal@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 2000m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 4400m, 1550m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Salario Profesora Universitaria", Monto = 3800m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Asesoría de Tesis", Monto = 700m, Fecha = new DateTime(y, m, 10), CategoriaId = 3 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Vivienda compartida", Monto = 700m, Fecha = new DateTime(y, m, 4), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Mercado y comida saludable", Monto = 450m, Fecha = new DateTime(y, m, 8), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Servicios compartidos", Monto = 130m, Fecha = new DateTime(y, m, 12), CategoriaId = 8 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Transporte tarjeta tren", Monto = 90m, Fecha = new DateTime(y, m, 15), CategoriaId = 9 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Libros académicos", Monto = 180m, Fecha = new DateTime(y, m, 18), CategoriaId = 11 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Té y merienda", Monto = 50m, Fecha = new DateTime(y, m, 20), CategoriaId = 13, EsGastoHormiga = true }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "Diagnóstico IA: Perfil Frugal Excepcional",
                    Mensaje = "Ahorras el 64.4% de tus ingresos netos. Tienes un fondo de seguridad ejemplar para contingencias.",
                    Prioridad = NivelPrioridad.Baja,
                    CategoriaAnalizada = "General"
                });
                break;

            // 10. RENZO: Descontrol Total y Sobregiro (Ingresos S/ 1,800 | Gastos S/ 3,450 | Déficit S/ -1,650)
            case "renzo.chihuan@antichihuan.pe":
                context.Presupuestos.Add(new Presupuesto { UsuarioId = userId, Mes = m, Anio = y, MontoLimite = 1700m, CategoriaId = null });
                AddHistorico(context, userId, hoy, 1800m, 3200m);
                context.Transacciones.AddRange(
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Ingreso, Descripcion = "Sueldo Básico Empleado", Monto = 1800m, Fecha = new DateTime(y, m, 1), CategoriaId = 1 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Zapatillas de marca en cuotas", Monto = 850m, Fecha = new DateTime(y, m, 3), CategoriaId = 14 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Farra y tragos discoteca fin de semana", Monto = 780m, Fecha = new DateTime(y, m, 5), CategoriaId = 12 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Alquiler atrasado", Monto = 600m, Fecha = new DateTime(y, m, 8), CategoriaId = 7 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Comida rápida y hamburguesas", Monto = 420m, Fecha = new DateTime(y, m, 12), CategoriaId = 6 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Apuestas deportivas online", Monto = 500m, Fecha = new DateTime(y, m, 16), CategoriaId = 15 },
                    new Transaccion { UsuarioId = userId, Tipo = TipoTransaccion.Gasto, Descripcion = "Intereses por sobregiro", Monto = 300m, Fecha = new DateTime(y, m, 20), CategoriaId = 15 }
                );
                context.RecomendacionesIA.Add(new RecomendacionIA
                {
                    UsuarioId = userId,
                    Titulo = "¡ALERTA MÁXIMA CHIHUÁN! Presupuesto Excedido en 202%",
                    Mensaje = "Estás gastando el doble de tu sueldo en ocio, ropa y apuestas. Tu presupuesto mensual colapsó. Cancela compras impulsivas y suspende tus tarjetas de crédito ya.",
                    Prioridad = NivelPrioridad.Critica,
                    CategoriaAnalizada = "Entretenimiento y Salidas"
                });
                break;
        }

        await context.SaveChangesAsync();
    }

    private static void AddHistorico(ApplicationDbContext context, string userId, DateTime hoy, decimal avgIngreso, decimal avgGasto)
    {
        // Generar 5 meses anteriores para que el gráfico histórico de Chart.js muestre tendencias ricas
        var rnd = new Random(userId.GetHashCode());
        for (int i = 5; i >= 1; i--)
        {
            var dt = hoy.AddMonths(-i);
            var variacionIng = avgIngreso * (decimal)(0.9 + rnd.NextDouble() * 0.2);
            var variacionGas = avgGasto * (decimal)(0.85 + rnd.NextDouble() * 0.3);

            context.Transacciones.Add(new Transaccion
            {
                UsuarioId = userId,
                Tipo = TipoTransaccion.Ingreso,
                Descripcion = "Ingresos Históricos del Mes",
                Monto = Math.Round(variacionIng, 2),
                Fecha = new DateTime(dt.Year, dt.Month, 5),
                CategoriaId = 1
            });

            context.Transacciones.Add(new Transaccion
            {
                UsuarioId = userId,
                Tipo = TipoTransaccion.Gasto,
                Descripcion = "Gastos Totales Históricos",
                Monto = Math.Round(variacionGas, 2),
                Fecha = new DateTime(dt.Year, dt.Month, 15),
                CategoriaId = 6
            });
        }
    }
}
