using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class User
    {
        public long id { get; set; }
        public string nombres { get; set; }        
        public string password { get; set; }
        [NotMapped]
        public string token { get; set; }
        public string email { get; set; }
        public string celular { get; set; }
        public DateTime createdon { get; set; }
    }
}