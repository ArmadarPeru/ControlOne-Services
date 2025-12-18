using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class EventoInfo
    {
        public long id { get; set; }
        public string nombre { get; set; }        
        public string lugar { get; set; }
        public string juego { get; set; }
        public int aforo { get; set; }
        public bool cajaIsOpen { get; set; }
        public DateTime fecha { get; set; }
        public string operadorRol { get; set; }
        public string operadorEmail { get; set; }
        public int tarifaMinutos { get; set; }
        public decimal tarifaPrecioMinuto { get; set; }
        public decimal tarifaPrecioMinutoAdicional { get; set; }
    }
}