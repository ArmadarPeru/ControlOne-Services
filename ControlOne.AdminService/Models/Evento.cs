using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Evento
    {
        public long id { get; set; }
        public string nombre { get; set; }        
        public string lugar { get; set; }
        public string juego { get; set; }
        public int aforo { get; set; }        
        public DateTime fechainicio { get; set; }
        public long operadorZona { get; set; }
        public long operadorJuego { get; set; }
        public string password { get; set; }
        public string password2 { get; set; }
        public int tarifaMinutos { get; set; }
        public decimal tarifaPreciominuto { get; set; }
        public decimal tarifaPreciominutoadicional { get; set; }
        public int activo { get; set; }
    }
}