using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class UserRole
    {
        public long id { get; set; }
        public long userId { get; set; }
        public long roleId { get; set; }
    }
}