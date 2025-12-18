using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class UpdateNihos
    {
        public string apoderadoDni { get; set; }
        public string toRemove { get; set; }
        public string toUpdate { get; set; }
    }
}