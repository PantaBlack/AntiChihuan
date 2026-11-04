using System.ComponentModel.DataAnnotations;

namespace NoMasChihuan.Models;

public enum TipoTransaccion
{
    [Display(Name = "Ingreso")]
    Ingreso = 1,

    [Display(Name = "Gasto")]
    Gasto = 2
}
