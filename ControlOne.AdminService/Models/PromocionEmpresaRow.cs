using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
    public class PromocionEmpresaRow
    {
        public long id { get; set; }
        public string empresa { get; set; }
        public int descuento { get; set; }
        public DateTime inicio { get; set; }
        public DateTime final { get; set; }
    }
}