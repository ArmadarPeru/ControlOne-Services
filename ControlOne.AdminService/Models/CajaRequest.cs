using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class CajaRequest
    {        
        public long eventoId { get; set; }
        public int anio { get; set; }
        public int mes { get; set; }
        public int dia { get; set; }

        public decimal ventaPOS { get; set; }
        public decimal faltanteVenta { get; set; }
        public decimal sobranteVenta { get; set; }
    }
}