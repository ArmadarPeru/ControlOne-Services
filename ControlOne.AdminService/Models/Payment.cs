using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class Payment
   {
      public long id { get; set; }
      public string codigo { get; set; }
      public long usuarioId { get; set; }
      public string usuarioEmail { get; set; }
      public long eventoId { get; set; }
      public DateTime eventoFecha { get; set; }
      public int horarioId { get; set; }
      public int usuariosMayor4 { get; set; }
      public int usuariosMenor4 { get; set; }
		public int ticket3 { get; set; }
		public int ticket4 { get; set; }
		public int montoInt { get; set; }
      public decimal montoDec { get; set; }
      public string status { get; set; }
      public DateTime createdOn { get; set; }
      public string token { get; set; }
      public string paymentResponse { get; set; }
      [NotMapped]
      public List<int> promociones { get; set; }
   }

   public class IziPayment
   {
      public string orderStatus { get; set; }
      public DateTime serverDate { get; set; }
      public IziOrderDetails orderDetails { get; set; }
      public List<IziTransaction> transactions { get; set; }
   }

   public class IziOrderDetails {
      public int orderTotalAmount { get; set; }
      public string orderId { get; set; }
   }

   public class IziTransaction
   {
      public string uuid { get; set; }
      public string paymentMethodType { get; set; }
      public IziTransactionDetails transactionDetails { get; set; }
   }

   public class IziTransactionDetails
   {
      public IziCardDetails cardDetails { get; set; }
   }

   public class IziCardDetails
   {
      public string pan { get; set; }
      public string effectiveBrand { get; set; }
   }

   public class InsertPaymentOfflineRequest
   {
      public long usuarioId { get; set; }
      public long eventoId { get; set; }
      public DateTime eventoFecha { get; set; }
      public int horarioId { get; set; }
      public int adultos { get; set; }
      public int noAdultos { get; set; }
      public long id { get; set; }
   }
}
