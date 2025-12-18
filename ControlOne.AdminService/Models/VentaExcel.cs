using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class VentaExcel
    {
        public string fecha { get; set; }
        public decimal total { get; set; }
        public decimal efectivo { get; set; }
        public decimal pos { get; set; }
    }
}