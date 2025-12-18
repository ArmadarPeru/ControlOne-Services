using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class TicketDefinicion
   {
      public int id { get; set; }
      public string codigo { get; set; }
      public string tipo { get; set; }
      public string titulo { get; set; }
      public string icono { get; set; }
      public string mensaje { get; set; }
      public int precio { get; set; }
   }
}