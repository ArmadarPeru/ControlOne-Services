using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Cobro
    {
        public long id { get; set; }
        public long eventoId { get; set; }
        public long nihoaccion { get; set; }
        public int minutos { get; set; }
        public decimal monto { get; set; }
        public decimal montoefectivo { get; set; }
        public decimal montotarjeta { get; set; }
        public decimal dscto { get; set; }
        public string dsctomotivo { get; set; }
        public long pagoMultiple { get; set; }
        public long promocionId { get; set; }
        [NotMapped]
        public string pagosMultiples { get; set; }
    }
}