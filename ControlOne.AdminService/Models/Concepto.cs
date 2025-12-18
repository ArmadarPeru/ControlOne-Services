using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Concepto
    {
        public long id { get; set; }
        public string tipo { get; set; }
        public string concepto { get; set; }
        public decimal importe { get; set; }        
    }
}