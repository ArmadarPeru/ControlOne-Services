using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Gasto
    {
        public long id { get; set; }
        public long eventoId { get; set; }
        public decimal importe { get; set; }
        public string tipoGasto { get; set; }
        public string tipoAbono { get; set; }
        public string comprobante { get; set; }
        public string nroComprobante { get; set; }
        public string proveedor { get; set; }
        public string gastoDetalle { get; set; }
        public DateTime fecha { get; set; }
    }
}