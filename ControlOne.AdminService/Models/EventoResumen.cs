using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class EventoResumen
    {
        public long id { get; set; }
        public string lugar { get; set; }
        public string juego { get; set; }
        public int jugando { get; set; }
        public int jugaron { get; set; }
        public int pagaron { get; set; }
        public decimal cajaAcumulada { get; set; }
        public decimal dscto { get; set; }
        public decimal pagosOnline { get; set; }
        public int isEntradaOnline { get; set; }
    }
}