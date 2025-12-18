using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class UsuarioControlJuego
    {
        public long id { get; set; }
        public long usuarioId { get; set; }
        public int conAcompaniante { get; set; }
        public int isConadis { get; set; }
        public int pagoAdelantado { get; set; }

        public string usuarioNombres { get; set; }
        public DateTime fechaInicio { get; set; }
        public int minutos { get; set; }
        public int segundos { get; set; }        
        public string apoderadoNombres { get; set; }
        public string apoderadoCelular { get; set; }
        public int accionId { get; set; }
        public string estado { get; set; }
    }
}