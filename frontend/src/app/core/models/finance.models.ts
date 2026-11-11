export interface AuthResponse {
  token: string;
  usuarioId: number;
  nombreCompleto: string;
  email: string;
}

export interface Categoria {
  id: number;
  nombre: string;
  tipo: 'INGRESO' | 'GASTO';
  icono: string;
  colorHex: string;
  esPredeterminada: boolean;
}

export interface Transaccion {
  id: number;
  descripcion: string;
  monto: number;
  fecha: string;
  tipo: 'INGRESO' | 'GASTO';
  esGastoHormiga: boolean;
  notas?: string;
  categoria?: Categoria;
}

export interface Presupuesto {
  id: number;
  mes: number;
  anio: number;
  montoLimite: number;
  categoria?: Categoria;
}

export interface RecomendacionIA {
  id: number;
  titulo: string;
  mensaje: string;
  categoriaAnalizada: string;
  prioridad: string;
  fechaGeneracion: string;
}

export interface IAConsejo {
  analisisGeneral: string;
  nivelRiesgoChihuan: string;
  gastosInnecesariosDetectados: string[];
  recomendacionesAhorro: string[];
  alertasPresupuesto: string[];
  estimadoAhorroMensualSugerido: number;
  fechaAnalisis: string;
}

export interface DashboardData {
  usuarioNombre: string;
  mesActual: number;
  anioActual: number;
  nombreMesActual: string;
  totalIngresosMes: number;
  totalGastosMes: number;
  balanceMes: number;
  presupuestoMes: number;
  porcentajePresupuestoConsumido: number;
  totalGastosHormigaMes: number;
  porcentajeGastosHormiga: number;
  estadoSemaforo: 'EXCELENTE' | 'ACEPTABLE' | 'ALERTA' | 'CHIHUAN';
  mensajeSemaforo: string;
  ultimasTransacciones: Transaccion[];
  gastosPorCategoria: {
    categoria: string;
    monto: number;
    colorHex: string;
    icono: string;
    porcentaje: number;
  }[];
  historicoUltimosMeses: {
    mesNombre: string;
    ingresos: number;
    gastos: number;
  }[];
  alertasIA: RecomendacionIA[];
}
