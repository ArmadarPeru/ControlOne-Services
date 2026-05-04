using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class TicketControl
   {
      public long id { get; set; }
      public String codigo { get; set; }
      public int horarioId { get; set; }
      public string nombres{ get; set; }
      public string dni { get; set; }

      public long usuarioId { get; set; }
      public int adultos { get; set; }
      public int nihos { get; set; }
      public int adultosPromocion { get; set; }
      public int nihosPromocion { get; set; }
      public int isUsado { get; set; }
      public int isUsuarioRegistrado { get; set; }

      [NotMapped]
      public bool isKey { get; set; }
      [NotMapped]
      public int adultosGroup { get; set; }
      [NotMapped]
      public int nihosGroup { get; set; }
      public decimal monto { get; set; }
   }

   public class TicketControl2
   {
      public long id { get; set; }
      public String codigo { get; set; }
      public int horarioId { get; set; }
      public string estado { get; set; }

      public string nombres { get; set; }
      public string dni { get; set; }
      public long usuarioId { get; set; }

      public bool isUsado { get; set; }
      public bool isUsuarioRegistrado { get; set; }
      public decimal monto { get; set; }
      public int cantidad { get; set; }
      public List<EntradaControl> entradas { get; set; } = new List<EntradaControl>();
		public List<PromoControl> promociones { get; set; } = new List<PromoControl>();
	}

   public class GroupedTicket
   {
      public int horarioId { get; set; }
      public List<TicketControl2> tickets { get; set; } = new List<TicketControl2>();
		public int cantidad { get; set; }
	}
}
