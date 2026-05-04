using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class Apoderado
   {
      public long id { get; set; }
      public string nombres { get; set; }
      public string pais { get; set; }
      public string dni { get; set; }
      public string celular { get; set; }
      public string email { get; set; }
      [JsonIgnore]
      public DateTime fechaCreacion { get; set; }
      [NotMapped]
      public string usuarios { get; set; }
      public string firma { get; set; }
   }

    public class ApoderadoUsuario
    {
        public string tipo { get; set; }
        public long id { get; set; }
        public long apoderadoId { get; set; }
        public string nombres { get; set; }
        public string pais { get; set; }
        public string dni { get; set; }
        public string celular { get; set; }
        public string email { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public int edad { get; set; }
    }
}