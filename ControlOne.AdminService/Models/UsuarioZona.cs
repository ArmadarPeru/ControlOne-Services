using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class UsuarioZona
    {
        public long id { get; set; }
        public string tipo { get; set; }
        public long usuarioId { get; set; }
        public string nombres { get; set; }
        public int edad { get; set; }
        public int enZona { get; set; }
    }
}