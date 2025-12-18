using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Models
{
   public class IziPayCreatePaymentRequest
   {
      public int amount { get; set; }
      public string currency { get; set; }
      public string orderId { get; set; }
      public IziPayCreatePaymentCustomerRequest customer { get; set; }
   }
   public class IziPayCreatePaymentCustomerRequest
   {
      public string email { get; set; }
      public string reference { get; set; }
   }

   public class IziPayCreatePaymentResponse
   {
      public string webService { get; set; }
      public string version { get; set; }
      public string applicationVersion { get; set; }
      public string status { get; set; }
      public IziPayCreatePaymentAnswerResponse answer { get; set; }
      public string ticket { get; set; }
      public string serverDate { get; set; }
      public string applicationProvider { get; set; }
      public string metadata { get; set; }
      public string mode { get; set; }
      public string serverUrl { get; set; }
      public string _type { get; set; }
   }
   public class IziPayCreatePaymentAnswerResponse
   {
      public string formToken { get; set; }
      public string _type { get; set; }
      public string errorMessage { get; set; }
      public string errorCode { get; set; }
      public string detailedErrorCode { get; set; }
      public string detailedErrorMessage { get; set; }
   }
}