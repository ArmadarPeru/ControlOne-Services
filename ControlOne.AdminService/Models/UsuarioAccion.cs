using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class UsuarioAccion
    {
        public long id { get; set; }
        public long usuarioId { get; set; }
        public long eventoId { get; set; }
        public long juego { get; set; }
        public int Accion { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaFinal { get; set; }
        public int minutos { get; set; }
        public int segundos { get; set; }
    }
}