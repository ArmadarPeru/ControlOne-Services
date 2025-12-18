using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Venta
    {
        public long id { get; set; }
        public long usuarioAccionId { get; set; }

        public string usuario { get; set; }
        public DateTime inicio { get; set; }
        public DateTime final { get; set; }

        public int minutos { get; set; }
        public int segundos { get; set; }
        public int minutosCobrados { get; set; }
        public decimal monto { get; set; }
        public decimal montoEfectivo { get; set; }
        public decimal montoTarjeta { get; set; }
        public decimal dscto { get; set; }
        public decimal total { get; set; }
    }
}