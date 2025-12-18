using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Usuario
    {
        public long id { get; set; }
        public string tipo { get; set; }
        public long apoderadoId { get; set; }
        public string nombres { get; set; }
        public int edad { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public DateTime fechaCreacion { get; set; }
    }
}