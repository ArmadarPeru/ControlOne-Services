using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class EventoOnline
   {
      public long id { get; set; }
      public string lugar { get; set; }
      public string juego { get; set; }
      public string descripcion { get; set; }
      public DateTime inicio { get; set; }
      public DateTime final { get; set; }
      public int diasDuracion { get; set; }
   }
}