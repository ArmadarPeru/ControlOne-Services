using ControlOne.AdminService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Armadar.Helpers;
using System.Data;
using Newtonsoft.Json;
using RestSharp;

namespace ControlOne.AdminService.Controllers
{
    public partial class AdminController : ControllerBase
    {
        /*
        [AllowAnonymous]
        [HttpGet("payment/{id}")]
        public async Task<IActionResult> getPayment(long id)
        {
            try
            {
                dynamic response = new System.Dynamic.ExpandoObject();
                var payment = getPaymentInfoDB(id);
                if (payment == null)
                {
                    return Ok(new { code = 3000, message = "No se pudo obtener el pago", success = false });
                }
                else
                {
                    return Ok(new { code = 1000, payment = payment, message = "SUCCESS", success = payment.status == "PAID", monto = payment.monto });
                }
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener el pago, " + e.Message, success = false });
            }
        }
        PaymentInfo getPaymentInfoDB(long paymentId)
        {
            try
            {
                var id = new SqlParameter("@id", paymentId);
                return _context.PaymentInfos.FromSql("[dbo].[getPayment] @id", id).ToList().FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        bool updatePaymentDB(Payment payment, int selectedTipo)
        {
            try
            {
                if (selectedTipo == 1)
                {
                    payment.status = string.Empty;
                    payment.creditCardBrand = string.Empty;
                    payment.creditCardPAN = string.Empty;
                    payment.iziHash = string.Empty;
                    payment.iziAnswer = string.Empty;
                    payment.paidOn = DateTime.Now;
                }
                else if (selectedTipo == 2)
                {
                    payment.iziFormToken = string.Empty;
                }

                var id = new SqlParameter("@id", payment.id);
                var tipo = new SqlParameter("@tipo", selectedTipo);
                var status = new SqlParameter("@status", payment.status);
                var creditCardBrand = new SqlParameter("@creditCardBrand", payment.creditCardBrand);
                var creditCardPAN = new SqlParameter("@creditCardPAN", payment.creditCardPAN);
                var iziFormToken = new SqlParameter("@iziFormToken", payment.iziFormToken);
                var iziHash = new SqlParameter("@iziHash", payment.iziHash);
                var iziAnswer = new SqlParameter("@iziAnswer", payment.iziAnswer);
                var paidOn = new SqlParameter("@paidOn", payment.paidOn);

                var sql = "EXEC [dbo].[updatePayment] @id, @tipo, @status, @creditCardBrand, @creditCardPAN, @iziFormToken, @iziHash, @iziAnswer, @paidOn";
                _context.Database.ExecuteSqlCommand(sql, id, tipo, status, creditCardBrand, creditCardPAN, iziFormToken, iziHash, iziAnswer, paidOn);
                return true;
            }
            catch (Exception exception)
            {
                string exceptionMessage = exception.Message;
                return false;
            }
        }

        [AllowAnonymous]
        [HttpPost("paymentconfirm")]
        public IActionResult paymentConfirm([FromForm] Dictionary<string, string> model)
        {
            string krAnswer = model.GetValueOrDefault("kr-answer", string.Empty);
            string krHash = model.GetValueOrDefault("kr-hash");
            string krHashAlgorithm = model.GetValueOrDefault("kr-hash-algorithm", string.Empty);

            bool HashIsOk = HashMacHelper.CheckHash(SEC_expectedAlgorithm, krHashAlgorithm, krAnswer, krHash, SEC_apiRestKey);

            if (HashIsOk)
            {
                IziPayment paymentObject = JsonConvert.DeserializeObject<IziPayment>(krAnswer);
                if (paymentObject != null)
                {
                    if (paymentObject.orderStatus == "PAID")
                    {
                        Payment formTokenInfoForUpdate = new Payment()
                        {
                            id = Convert.ToInt64(paymentObject.orderDetails.orderId),
                            status = paymentObject.orderStatus,
                            creditCardBrand = paymentObject.transactions[0].transactionDetails.cardDetails.effectiveBrand,
                            creditCardPAN = paymentObject.transactions[0].transactionDetails.cardDetails.pan,
                            iziHash = krHash,
                            iziAnswer = krAnswer,
                            paidOn = paymentObject.serverDate
                        };

                        if (updatePaymentDB(formTokenInfoForUpdate, 2))
                        {
                            return Ok(new { code = 1000, orderId = formTokenInfoForUpdate.id, message = "SUCCESS" });
                        }
                        else
                        {
                            return Ok(new { code = 5000, orderId = 0, message = "No se pudo actualizar el pago" });
                        }
                    }
                    else
                    {
                        return Ok(new { code = 4000, orderId = 0, message = "Kr answer Status invalido" });
                    }
                }
                else
                {
                    return Ok(new { code = 3000, orderId = 0, message = "Kr answer invalido" });
                }
            }
            else
            {
                return Ok(new { code = 2000, orderId = 0, message = "Hash invalido" });
            }
        }

        private Tuple<bool, string> createIziPayFormToken(MinimalIziPayFormInfo iziPayFormInfo)
        {
            bool isFormTokenSuccess = false; string formTokenValue = string.Empty;
            using (var client = new HttpClient())
            {
                // TODO : Maping con lo minimo necesario para el pago
                IziPayCreatePaymentCustomerRequest customer = new IziPayCreatePaymentCustomerRequest()
                {
                    email = iziPayFormInfo.customerEmail,
                    reference = iziPayFormInfo.customerReference
                };

                IziPayCreatePaymentRequest payment = new IziPayCreatePaymentRequest()
                {
                    amount = iziPayFormInfo.amount,
                    currency = iziPayFormInfo.currency,
                    customer = customer,
                    orderId = iziPayFormInfo.orderId
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {getAuthorizatinoKey()}");

                var response = client.PostAsJsonAsync("https://api.micuentaweb.pe/api-payment/V4/Charge/CreatePayment/", payment).Result;

                IziPayCreatePaymentResponse iziPayPaymentResponse = response.Content.ReadAsAsync<IziPayCreatePaymentResponse>().Result;
                if (iziPayPaymentResponse.status == "SUCCESS")
                {
                    isFormTokenSuccess = true;
                    formTokenValue = iziPayPaymentResponse.answer.formToken;
                }
                else
                {
                    formTokenValue = ($"{iziPayPaymentResponse.answer.errorCode}|{iziPayPaymentResponse.answer.errorMessage}|{iziPayPaymentResponse.answer.detailedErrorMessage}");
                }
            }
            return new Tuple<bool, string>(isFormTokenSuccess, formTokenValue);
        }   
        */

        /*[AllowAnonymous]*/
        [HttpPost("_paymentorder_")]
        public IActionResult _paymentOrder_([FromBody] Payment paymentOrder)
        {
            if (paymentOrder != null) // Validar integridad de cada propiedad desde la UI
            {
                paymentOrder.eventoFecha = paymentOrder.eventoFecha.ToUniversalTime().AddHours(-5);
                CalcularMonto(paymentOrder);
                AssignUniquePaymentCodigo(paymentOrder);
                long orderId = insertPaymentDB(paymentOrder);

                if (orderId != 0)
                {
                    MinimalIziPayFormInfo createFormTokenInfo = new MinimalIziPayFormInfo()
                    {
                        customerReference = paymentOrder.usuarioId.ToString(),
                        customerEmail = paymentOrder.usuarioEmail,
                        amount = paymentOrder.montoInt,
                        currency = "PEN",
                        orderId = orderId.ToString()
                    };

                    if (true)//if (updatePaymentDB(formTokenInfoForUpdate, 1))
                    {
                        return Ok(new { code = 1000, orderId = orderId, codigo = paymentOrder.codigo });
                    }
                    else
                    {
                        return Ok(new { code = 4000, message = "No se pudo actualizar el Form Token" });
                    }
                }
                else
                {
                    return Ok(new { code = 3000, message = "No se pudo crear la Orden de Pago" });
                }
            }
            else
            {
                return Ok(new { code = 2000, message = "Informacion invalida" });
            }
        }

        [AllowAnonymous]
        [HttpPost("izitrack")]
        public IActionResult iziTrack([FromForm] Dictionary<string, string> model)
        {
            string x = model.GetValueOrDefault("nickName");

            insertIziTrack("value : " + x);
            return Ok(new { code = 1000 });
        }

        void insertIziTrack(string iziTrackInfo)
        {
            var rawJson = new SqlParameter("@rawJson", iziTrackInfo);

            var sql = "EXEC dbo.insertIziTrack @rawJson";
            _context.Database.ExecuteSqlCommand(sql, rawJson);
        }

        List<IziTrack> getIziTracksDB()
        {
            try
            {
                return _context.IziTracks.FromSql("[dbo].[getIziTracks]").ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        [AllowAnonymous]
        [HttpGet("izitracks")]
        public async Task<IActionResult> getIziTracks()
        {
            try
            {
                return Ok(getIziTracksDB());
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener los Izi Tracks" });
            }
        }

        string getAuthorizatinoKey()
        {
            string user = SEC_apiRestUser;
            string password = SEC_apiRestKey;
            string result =
               Convert.ToBase64String(
               Encoding.UTF8.GetBytes($"{user}:{password}")
               );
            return result;
        }
    }
}