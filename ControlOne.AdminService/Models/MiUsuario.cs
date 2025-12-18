using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class MiUsuario
    {
        public long id { get; set; }
        public string usuario { get; set; }        
        public int edad { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public int minutos { get; set; }
        public int segundos { get; set; }
        public int accionId { get; set; }
        public string estado { get; set; }
        public long eventoId { get; set; }
    }
}