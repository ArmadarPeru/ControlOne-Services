using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
	public class PaymentInfoORM
	{
		public long id { get; set; }
		public long usuarioId { get; set; }
		public long eventoId { get; set; }
		public DateTime eventoFecha { get; set; }
		public int horarioId { get; set; }

		public int usuariosMayor4 { get; set; }
		public int usuariosMenor4 { get; set; }
		public int ticket3 { get; set; }
		public int ticket4 { get; set; }

		
		public decimal montoDec { get; set; }
		public int montoInt { get; set; }
		public string status { get; set; }
		public string codigo { get; set; }
		public string promociones { get; set; }

		public int isUsado { get; set; }
		public string token { get; set; }

		public string paymentResponse { get; set; }
		public DateTime createdOn { get; set; }

		public int cantidad { get; set; }

		public EventoORM evento { get; set; }
		[NotMapped]
		public Apoderado apoderado { get; set; }

		[NotMapped]
		public string estado { get; set; }
      [NotMapped]
		public List<PromocionInfo> promocionesList { get; set; }
	}

	public class PaymentInfo
   {
      public long id { get; set; }
		public long usuarioId { get; set; }
		public string codigo { get; set; }
      public decimal monto { get; set; }
      public DateTime eventoFecha { get; set; }
      public long eventoId { get; set; }
      public string status { get; set; }
      public string eventoLugar { get; set; }
      public string eventoJuego { get; set; }
      public int horarioId { get; set; }
      public int usuariosMayor4 { get; set; }
      public int usuariosMenor4 { get; set; }
      public DateTime createdOn { get; set; }
      public string promociones { get; set; }
      public string ticketDefinicion { get; set; }
      public string tipoTickets { get; set; }
      public List<PromocionInfo> promocionesList { get; set; }
      [NotMapped]
      public List<string> tipoTicketsList { get; set; }
      public string estado { get; set; }
   }

   public class PromocionInfo
   {
      public int id { get; set; }
      public string nombre { get; set; }
      public int adultos { get; set; }
      public int nihos { get; set; }
      public decimal precio { get; set; }
   }
}
