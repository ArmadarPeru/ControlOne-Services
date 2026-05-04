using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class Promocion
    {
        public long id { get; set; }
        public long eventoId { get; set; }
        public int minutos { get; set; }
        public decimal preciominuto { get; set; }
        public decimal preciominutoadicional { get; set; }
        public DateTime inicio { get; set; }
        public DateTime final { get; set; }
        public int activo { get; set; }
        public DateTime horaactual { get; set; }
    }

   public class PromocionProgramacion
   {
      public long eventoId { get; set; }
      public int mes { get; set; }
      public string promocionesFlat { get; set; }
   }

   public class PromocionFecha
   {
      public int id { get; set; }
      public int dia { get; set; }
      public string nombre { get; set; }
      public int cantidada { get; set; }
      public int cantidadb { get; set; }
      public decimal precio { get; set; }
      public string descripcion { get; set; }
   }

	public class PromoBought
	{
		public long paymentId { get; set; }
		public int promoId { get; set; }
		public int cantidad { get; set; }
	}

   public class PromoControl
   {
		public long id { get; set; }
		public int promoId { get; set; }
		public string nombre { get; set; }
		public string descripcion { get; set; }
		public int ticket1 { get; set; }
		public int ticket2 { get; set; }
		public int ticket3 { get; set; }
		public int ticket4 { get; set; }
		public int cantidad { get; set; }
	}
}