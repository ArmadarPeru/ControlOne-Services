using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class AforoInfo
   {
      public int id { get; set; }
      public int aforo { get; set; }
      public int ocupados { get; set; }
      public bool disponible{ get; set; }

   }
}
