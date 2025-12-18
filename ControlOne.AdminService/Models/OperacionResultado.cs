using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class OperacionResultado
    {
      public int id { get; set; }
      public int resultado { get; set; }
      public string mensaje { get; set; }
   }
}