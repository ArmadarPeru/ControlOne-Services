using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class CulqiChargeRequest
    {
        public string source_id { get; set; }
        public int amount { get; set; }
        public string currency_code { get; set; }
        public string email { get; set; }
    }
}