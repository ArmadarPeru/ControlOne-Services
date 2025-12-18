using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Caja
    {
        public long id { get; set; }
        public long eventoId { get; set; }
        public DateTime fecha { get; set; }

        public decimal efectivoInicial { get; set; }
        public decimal ventaEfectivo { get; set; }
        public decimal ventaOtros { get; set; }
        public decimal ventaTotal { get; set; }

        public decimal gastos { get; set; }
        public decimal efectivoCaja { get; set; }
        public decimal abonosEntregables { get; set; }
        public decimal abonos { get; set; }
        public decimal retiros { get; set; }
        public decimal efectivoFinalCaja { get; set; }

        public decimal ventaPOS { get; set; }
        public decimal faltanteVenta { get; set; }
        public decimal sobranteVenta { get; set; }
        public decimal cierreCaja { get; set; }
    }
}