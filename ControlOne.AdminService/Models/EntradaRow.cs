using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class EntradaRow
   {
      public int id { get; set; }
      public string codigo { get; set; }
      public string tipo { get; set; }
      public string titulo { get; set; }
      public string icono { get; set; }
      public string mensaje { get; set; }
      public int precio { get; set; }
   }

   public class EntradaRequest
   {
      public int id { get; set; }
      public string codigo { get; set; }
      public string tipo { get; set; }

      public string tituloAdulto { get; set; }
      public string tituloNoAdulto { get; set; }
		public string tituloEntrada3 { get; set; }
		public string tituloEntrada4 { get; set; }

		public string mensajeAdulto { get; set; }
      public string mensajeNoAdulto { get; set; }
		public string mensajeEntrada3 { get; set; }
		public string mensajeEntrada4 { get; set; }

		public int precioAdulto { get; set; }
      public int precioNoAdulto { get; set; }
		public int precioEntrada3 { get; set; }
		public int precioEntrada4 { get; set; }
	}

   public class ReprogramarEntradaRequest
   {
      public long entradaId { get; set; }
      public DateTime fecha { get; set; }
      public int horarioId { get; set; }

   }
}