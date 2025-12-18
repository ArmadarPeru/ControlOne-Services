using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class MinimalIziPayFormInfo
   {
      public string customerReference { get; set; }
      public string customerEmail { get; set; }
      public int amount { get; set; }
      public string currency { get; set; }
      public string orderId { get; set; }
   }
}