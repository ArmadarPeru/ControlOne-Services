using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class EventoHorario
   {
      public int id { get; set; }
      public DateTime inicio { get; set; }
      public int aforo { get; set; }
      public int isSeleccionable { get; set; }
      public int vacantes { get; set; }
   }

	public class SimpleAforo
	{
		public int id { get; set; }
		public int aforo { get; set; }
		public int ocupados { get; set; }
		public int disponible { get; set; }
	}
	
   public class IziTrack
   {
      public long id { get; set; }
      public string rawJson { get; set; }
      public DateTime createdOn { get; set; }
   }

   public class EventoHorarioORM
   {
		public long eventoId { get; set; }
		public DateTime inicio { get; set; }
		public DateTime final { get; set; }
	}
}