using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Configuracion
    {
        public long id { get; set; }
        public int apoderadoVerTiempo { get; set; }
        public DateTime horaActual { get; set; }
    }
}