using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
	public class EventoPromocion
	{
		public int id { get; set; }
		public long eventoId { get; set; }
		public int promocionId { get; set; }
		public EventoORM evento { get; set; }
	}
}