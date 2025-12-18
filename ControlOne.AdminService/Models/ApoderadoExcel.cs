using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class ApoderadoExcel
    {
        public long id { get; set; }
        public string nombres { get; set; }
        public string dni { get; set; }
        public string celular { get; set; }
        public string email { get; set; }
    }
}