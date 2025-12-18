using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class PromocionRow
    {
        public long id { get; set; }
        public long eventoId { get; set; }
        public DateTime inicio { get; set; }
        public DateTime final { get; set; }

        public int inicioHora { get; set; }
        public int inicioMinuto { get; set; }
        public int finalHora { get; set; }
        public int finalMinuto { get; set; }

        public int minutos { get; set; }
        public decimal precio { get; set; }
        public decimal precioAdicional { get; set; }
    }
}