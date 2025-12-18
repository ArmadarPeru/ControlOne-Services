using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class UsuarioControlSalida
    {
        public long id { get; set; }
        public string usuarioNombres { get; set; }
        public string dni { get; set; }
        public int minutos { get; set; }
        public int segundos { get; set; }
        public int tiempo { get; set; }
        public decimal monto { get; set; }
        public long apoderadoId { get; set; }
        public string apoderadoNombres { get; set; }
        public string apoderadoCelular { get; set; }
        public int pagoAdelantado { get; set; }
        public int isConadis { get; set; }
    }
}