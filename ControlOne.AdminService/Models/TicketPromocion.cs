using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class TicketPromocion
   {
      public int id { get; set; }
      public long eventoId { get; set; }
      public string tipo { get; set; }
      public DateTime fecha { get; set; }
      public int adultos { get; set; }
      public int nihos { get; set; }
      public string nombre { get; set; }
      public decimal precio { get; set; }
      public string descripcion { get; set; }
   }

   public class TicketPrecio
   {
      public int id { get; set; }
      public int adulto { get; set; }
      public int niho { get; set; }
   }
}