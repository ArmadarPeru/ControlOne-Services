using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class EventoRow
   {
      public long id { get; set; }
      public string lugar { get; set; }
      public string juego { get; set; }
      public int aforo { get; set; }
      public string operadorZonaEmail { get; set; }
      public string operadorJuegoEmail { get; set; }
      public string passwordZona { get; set; }
      public string passwordJuego { get; set; }
      public decimal cajaInicial { get; set; }
      public int tarifaMinutos { get; set; }
      public decimal tarifaPrecioMinuto { get; set; }
      public decimal tarifaPrecioMinutoAdicional { get; set; }
      public int activo { get; set; }
      public string ticketDefinicion { get; set; }
      public string ticketsPromociones { get; set; }
      public DateTime horaInicio { get; set; }
      public DateTime horaFinal { get; set; }
      public string descripcion { get; set; }
      public DateTime inicio { get; set; }
      public DateTime final { get; set; }
      public int isEntradaOnline { get; set; }
      public int isCompraDirecta { get; set; }
   }
}