using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class TicketGroup
   {
      public int id { get; set; }
      public List<long> items { get; set; }
      public long middleIndex { get; set; }
      public int totalAdults { get; set; }
      public int totalKids { get; set; }
   }
}
