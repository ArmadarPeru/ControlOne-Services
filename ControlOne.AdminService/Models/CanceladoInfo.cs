using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class CanceladoInfo
    {
        public long id { get; set; }
        public DateTime fecha { get; set; }
        public string apoderado { get; set; }
        public string celular { get; set; }
        public string usuario { get; set; }
    }
}