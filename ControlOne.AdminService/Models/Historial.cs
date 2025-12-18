using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Historial
    {
        public long id { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFinal { get; set; }
        public string apoderado { get; set; }
        public string dni { get; set; }
        public string celular { get; set; }
        public string usuario { get; set; }
        public int minutos { get; set; }
        public decimal monto { get; set; }
        public decimal montoPago { get; set; }
        public decimal dscto { get; set; }
        public DateTime fechaPago { get; set; }
    }
}