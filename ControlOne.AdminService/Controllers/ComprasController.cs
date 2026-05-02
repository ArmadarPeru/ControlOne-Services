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
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ControlOne.AdminService.Controllers
{
   public partial class AdminController : ControllerBase
   {
      private const string SEC_apiRestUser = "24178927";
      private const string SEC_apiRestKey = "testpassword_N6qdlgYPARLwaNBBpFXAGDykfj4Y2TisN9KvTPgISjdLd";
      private const string SEC_expectedAlgorithm = "sha256_hmac";

      [AllowAnonymous]
      [HttpGet("eventosonline/")]
      public IActionResult getEventosOnline()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         var eventos = getEventosOnlineDB();
         response.eventosOnline = eventos;
         return Ok(response);
      }
      List<EventoOnline> getEventosOnlineDB()
      {
         try
         {
            return _context.EventosOnline.FromSql("getEventosOnline").ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      /*
      [AllowAnonymous]
      [HttpGet("getevento/{eventoid}")]
      public IActionResult getEvento(long eventoid)
      {
         dynamic response = new System.Dynamic.ExpandoObject();

         var evento = _context.EventosORM.First(e => e.id == eventoid);
         var oEvento = new
         {
            evento.id,
            evento.lugar,
            evento.juego,
            evento.descripcion,
            evento.inicio,
            evento.final,
            evento.isCompraDirecta
         };
         response.evento = oEvento;
         return Ok(response);
      }
      */

		[AllowAnonymous]
      [HttpGet("eventohorarios/{evento}/{pipedFecha}")]
      public async Task<IActionResult> getHorariosEvento(long evento, string pipedFecha)
      {
         try
         {
            string[] granularDate = pipedFecha.Split("|");
            DateTime eventoFecha = new DateTime(Convert.ToInt32(granularDate[2]), Convert.ToInt32(granularDate[1]), Convert.ToInt32(granularDate[0]));
            return Ok(getHorariosEventoDB(evento, eventoFecha));
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener los horarios del Evento" });
         }
      }
      List<EventoHorario> getHorariosEventoDB(long eventoId, DateTime eventoFecha)
      {
         try
         {
            var evento = _context.EventosORM.First(e => e.id == eventoId);
            DateTime inicio = evento.horaInicio, final = evento.horaFinal;
            TimeSpan diferencia = final - inicio;

            List<EventoHorario> horariosInfo = new List<EventoHorario>();

            var mediasHoras = diferencia.TotalHours * 2;

            for (int i = 0; i < mediasHoras; i++)
            {
               horariosInfo.Add(new EventoHorario()
               {
                  id = i == 0 ? int.Parse((inicio.Hour.ToString("00") + inicio.Minute.ToString("00"))) : int.Parse((horariosInfo[i - 1].inicio.AddMinutes(30).Hour.ToString("00") + (horariosInfo[i - 1].inicio.AddMinutes(30).Minute.ToString("00")))),
                  inicio = i == 0 ? inicio : horariosInfo[i - 1].inicio.AddMinutes(30),
						isSeleccionable = 1,
						aforo = evento.aforo,                  
                  vacantes = evento.aforo
               });
            }

            foreach (var h in horariosInfo)
            {
               if (eventoFecha.Date == DateTime.UtcNow.AddHours(-5))
               {
						h.isSeleccionable = h.inicio.TimeOfDay < DateTime.UtcNow.AddHours(-5).AddMinutes(-15).TimeOfDay ? 0 : 1;
					}               
            }

            var paysByHorarios = _context.PaymentInfoORMs.Where(pay => pay.eventoId == eventoId && pay.eventoFecha.Date == eventoFecha.Date && pay.horarioId != 0 && pay.status == "PAID").
               GroupBy(pay => pay.horarioId).
               Select(g => new
               {
                  horarioId= g.Key,
                  totalCount =g.Sum(x=>x.cantidad)
               }).ToList();

				foreach (var h in horariosInfo)
				{
               if (h.isSeleccionable == 1 || 1 == 1)
               {
                  if (paysByHorarios.Select(x => x.horarioId).Contains(h.id))
                  {
                     h.aforo = evento.aforo;
                     h.vacantes = evento.aforo - paysByHorarios.First(p => p.horarioId == h.id).totalCount;
						}
               }
				}

            return horariosInfo;
            /* TODO DELETE REPLACED BY ABOVE EF
				var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _eventoFecha = new SqlParameter("@eventoFecha", eventoFecha);
            return _context.EventoHorarios.FromSql("[dbo].[getHorariosByEventoId] @eventoId,@eventoFecha", _eventoId, _eventoFecha).ToList();
            */
         }
         catch (Exception e)
         {
            return null;
         }
      }

      SimpleAforo getSimpleAforoDB(long eventoId)
      {
         try
         {
            var evento = _context.EventosORM.First(e => e.id == eventoId);
            var ocupados = _context.PaymentInfoORMs.
               Where(pay => pay.eventoId == eventoId && pay.status == "PAID").
               Sum(p => p.cantidad);

            return new SimpleAforo()
            {
               aforo = evento.aforo,
               ocupados = ocupados,
               disponible = evento.aforo - ocupados,
               id = 1
            };
            /* TODO TO REMOVE BY EF USAGE
				var _eventoId = new SqlParameter("@eventoId", eventoId);
				return _context.SimpleAforos.FromSql("[dbo].[geAforoByEventoId] @eventoId", _eventoId).ToList().FirstOrDefault();
            */
         }
         catch (Exception e)
         {
            return null;
         }
      }		

      private Tuple<bool, string> generarCulqiCargo(string token, int monto, string email)
      {
         Tuple<bool, string> result = new Tuple<bool, string>(false, string.Empty);

         try
         {
            string llaveprivada_PRD = "sk_live_1854910889cd0bf3";
            string llavePrivada_TEST = "sk_test_GlLVNypqgJF2aWzX";

				string llavePrivada = llavePrivada_TEST;
            var client = new RestClient("https://api.culqi.com/v2/charges");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + llavePrivada);
            request.AddHeader("content-type", "application/json");

            var culquiChargeRequest = new CulqiChargeRequest
            {
               source_id = token,
               amount = monto,
               currency_code = "PEN",
               email = email
            };

            string planeCulqiChargeRequest = JsonConvert.SerializeObject(culquiChargeRequest);
            request.AddParameter("application/json", planeCulqiChargeRequest, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.Created)
            {
               result = new Tuple<bool, string>(true, response.Content);
            }
            else
            {
               result = new Tuple<bool, string>(false, response.Content);
            }
         }
         catch (Exception)
         {

         }

         return result;
      }

      [AllowAnonymous]
      [HttpPost("paymentorder")]
      public IActionResult paymentOrder([FromBody] Payment paymentOrder)
      {
         if (paymentOrder != null) // Validar integridad de cada propiedad desde la UI
         {
            paymentOrder.eventoFecha = paymentOrder.eventoFecha.ToUniversalTime().AddHours(-5);

				var evento = _context.EventosORM.First(e => e.id == paymentOrder.eventoId);
            if (evento.isCompraDirecta==1) {
               paymentOrder.eventoFecha = evento.inicio;
				}

            CalcularMontoyCantidad(paymentOrder);
            AssignUniquePaymentCodigo(paymentOrder);

            var culqiCargo = generarCulqiCargo(paymentOrder.token, paymentOrder.montoInt, paymentOrder.usuarioEmail);
            paymentOrder.paymentResponse = culqiCargo.Item2;
            paymentOrder.status = culqiCargo.Item1 ? "PAID" : "ERROR";
            long orderId = insertPaymentDB(paymentOrder);

            if (culqiCargo.Item1)
            {
               return Ok(new { code = 1000, orderId = orderId, codigo = paymentOrder.codigo, monto = paymentOrder.montoDec });
            }
            else
            {
               return Ok(new { code = 3000, message = "No se pudo crear el Cargo de Pago" });
            }
         }
         else
         {
            return Ok(new { code = 2000, message = "Informacion invalida" });
         }
      }

      void CalcularMontoyCantidad(Payment payment)
      {
         // Calcular Entradas individuales

         int adultPrice = 0, kidPrice = 0, ticket3Price = 0, ticket4Price = 0;
         if (payment.eventoId != 0)
         {
            try
            {
               var ticketsInfo = getTicketTiposByEventoIdDB(payment.eventoId);

               if (ticketsInfo.Count == 1)
               {
                  adultPrice = ticketsInfo[0].precio;
               }
               else if (ticketsInfo.Count == 2)
               {
						adultPrice = ticketsInfo[0].precio;
						kidPrice = ticketsInfo[1].precio;
               }
               else if (ticketsInfo.Count == 3)
               {
						adultPrice = ticketsInfo[0].precio;
						kidPrice = ticketsInfo[1].precio;
						ticket3Price = ticketsInfo[2].precio;
               }
               else if (ticketsInfo.Count == 4)
               {
						adultPrice = ticketsInfo[0].precio;
						kidPrice = ticketsInfo[1].precio;
						ticket3Price = ticketsInfo[2].precio;
						ticket4Price = ticketsInfo[3].precio;
               }
            }
            catch (Exception)
            {

            }
         }

			// Calcular Promociones

			decimal promocionesTotal = 0;

			if (payment.promociones.Length>0)
         {
				var selectedPromosInfo = payment.promociones.Split('|');				

				if (selectedPromosInfo.Any())
				{
					var promosByEvento = getTicketPromocionesByEventoIdAndDiaDB(payment.eventoId, DateTime.UtcNow.AddHours(-5));

					//if (payment.promociones.Count != promosByEvento.Count()) throw new Exception("Las Promociones no coinciden");

					for (int i = 0; i < selectedPromosInfo.Length; i++)
					{
						var selectedPromoInfo = selectedPromosInfo[i].Split(',');
						int promoId = int.Parse(selectedPromoInfo[0]);
						int promoCount = int.Parse(selectedPromoInfo[1]);
						if (promoCount > 0)
						{
							var oPromo = promosByEvento.Find(pro => pro.id == promoId);

							promocionesTotal += oPromo.precio * promoCount;
							payment.cantidad += (oPromo.adultos + oPromo.nihos + oPromo.ticket3 + oPromo.ticket4) * promoCount;
						}
					}
				}
			}			

			payment.cantidad += payment.usuariosMayor4 + payment.usuariosMenor4 + payment.ticket3 + payment.ticket4;

			payment.montoDec = (payment.usuariosMayor4 * adultPrice) + (payment.usuariosMenor4 * kidPrice) + (payment.ticket3 * ticket3Price) + (payment.ticket4 * ticket4Price) + 
            promocionesTotal;
         payment.montoInt = (int)(payment.montoDec * 100);
      }

      static void AssignUniquePaymentCodigo(Payment paymentInfo)
      {
         DateTime moment = DateTime.Now.ToUniversalTime().AddHours(-5);

         var result = new StringBuilder();
         result.Append(String.Format("{0:000000000}", paymentInfo.usuarioId));
         result.Append("-");
         result.Append(moment.Year.ToString());
         result.Append(String.Format("{0:00}", moment.Month));
         result.Append(String.Format("{0:00}", moment.Day));
         result.Append("-");
         result.Append(String.Format("{0:00}", moment.Hour));
         result.Append(String.Format("{0:00}", moment.Minute));
         result.Append(String.Format("{0:00}", moment.Second));

         paymentInfo.codigo = result.ToString();
      }

      long insertPaymentDB(Payment payment)
      {
         var usuarioId = new SqlParameter("@usuarioId", payment.usuarioId);
         var eventoId = new SqlParameter("@eventoId", payment.eventoId);
         var eventoFecha = new SqlParameter("@eventoFecha", payment.eventoFecha);
         var horarioId = new SqlParameter("@horarioId", payment.horarioId);
         var usuariosMayor4 = new SqlParameter("@usuariosMayor4", payment.usuariosMayor4);
         var usuariosMenor4 = new SqlParameter("@usuariosMenor4", payment.usuariosMenor4);
			var ticket3 = new SqlParameter("@ticket3", payment.ticket3);
			var ticket4 = new SqlParameter("@ticket4", payment.ticket4);
			var montoInt = new SqlParameter("@montoInt", payment.montoInt);
         var montoDec = new SqlParameter("@montoDec", payment.montoDec);
         var status = new SqlParameter("@status", payment.status);
         var codigo = new SqlParameter("@codigo", payment.codigo);
         var promociones = new SqlParameter("@promociones", string.Join("|", payment.promociones));
         var token = new SqlParameter("@token", payment.token);
			var cantidad = new SqlParameter("@cantidad", payment.cantidad);
			var paymentResponse = new SqlParameter("@paymentResponse", payment.paymentResponse);
         var paymentOut = new SqlParameter("@paymentId", SqlDbType.BigInt); paymentOut.Direction = ParameterDirection.Output;

         var sql = "EXEC dbo.insertPayment @usuarioId, @eventoId, @eventoFecha, @horarioId, @usuariosMayor4, @usuariosMenor4, @montoInt, @montoDec, @status, @codigo, @promociones, @token, @paymentResponse, @ticket3, @ticket4, @cantidad, @paymentId OUTPUT";
         var data = _context.Database.ExecuteSqlCommand(sql, usuarioId, eventoId, eventoFecha, horarioId, usuariosMayor4, usuariosMenor4, montoInt, montoDec, status, codigo, promociones, token, paymentResponse, ticket3, ticket4, cantidad, paymentOut);

         return (long)paymentOut.Value;
      }

      [HttpPost("insertpaymentoffline")]
      public IActionResult insertpaymentoffline(InsertPaymentOfflineRequest request)
      {
         try
         {
            long id = insertPaymentOfflineDB(new Payment()
            {
               usuarioId = request.usuarioId,
               eventoId = request.eventoId,
               eventoFecha = DateTime.UtcNow.AddHours(-5),
               horarioId = request.horarioId,
               usuariosMayor4 = request.adultos,
               usuariosMenor4 = request.noAdultos
            });
            return Ok(new { code = 1000, message = "El pago ha sido registrado" });
         }
         catch (Exception ex)
         {
            return Ok(new { code = 2000, message = "El pago no ha sido registrado " + ex.Message });
         }
      }
      long insertPaymentOfflineDB(Payment payment)
      {
         var usuarioId = new SqlParameter("@usuarioId", payment.usuarioId);
         var eventoId = new SqlParameter("@eventoId", payment.eventoId);
         var eventoFecha = new SqlParameter("@eventoFecha", payment.eventoFecha);
         var horarioId = new SqlParameter("@horarioId", payment.horarioId);
         var usuariosMayor4 = new SqlParameter("@usuariosMayor4", payment.usuariosMayor4);
         var usuariosMenor4 = new SqlParameter("@usuariosMenor4", payment.usuariosMenor4);
         var paymentOut = new SqlParameter("@paymentId", SqlDbType.BigInt); paymentOut.Direction = ParameterDirection.Output;

         var sql = "EXEC dbo.insertPaymentOffline @usuarioId, @eventoId, @eventoFecha, @horarioId, @usuariosMayor4, @usuariosMenor4, @paymentId OUTPUT";
         var data = _context.Database.ExecuteSqlCommand(sql, usuarioId, eventoId, eventoFecha, horarioId, usuariosMayor4, usuariosMenor4, paymentOut);

         return (long)paymentOut.Value;
      }

      [AllowAnonymous]
      [HttpGet("mistickets/{usuarioId}")]
      public async Task<IActionResult> myTickets(long usuarioId)
      {
         try
         {
            return Ok(new { code = 1000, message = "SUCCESS", mistickets = myTicketsDB(usuarioId) });
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener los Izi Tracks" });
         }
      }

      string getEstado(PaymentInfoORM payment)
      {
         if (payment.isUsado == 1)
            return "USADO";
         if (payment.isUsado == 0 && (payment.eventoFecha < DateTime.UtcNow.AddHours(-5)))
            return "VENCIDO";
         if (payment.isUsado == 0 && (payment.eventoFecha >= DateTime.UtcNow.AddHours(-5)))
         {
            return "ACTIVO";
         }
         else { return string.Empty; }
      }

      dynamic myTicketsDB(long userId)
      {
         try
         {
            var payments = _context.PaymentInfoORMs.Where(pay => pay.usuarioId == userId && pay.status == "PAID" && pay.isUsado == 0).Include(ev => ev.evento).ToList();				
            var eventosTipoTickets = payments.Select(p => p.evento.ticketDefinicion).Distinct().ToList();

				var bougthPromos = new List<PromoBought>();

            for (int i = 0; i < payments.Count; i++)
            {
               if (payments[i].promociones.Length > 0)
               {
                  var promos = payments[i].promociones.Split('|');
                  foreach (var promoFlat in promos)
                  {
                     var promoInfo = promoFlat.Split(',');
                     bougthPromos.Add(new
                     PromoBought
                     {
                        paymentId = payments[i].id,
                        promoId = int.Parse(promoInfo[0]),
                        count = int.Parse(promoInfo[1])
                     });
                  }
               }
            }

				var eventosIds = payments.Select(p => p.eventoId).Distinct().ToList();

				var tickesDefinicion = _context.TicketTipos.
               Where(t => eventosTipoTickets.Contains(t.codigo)).
               ToList();

				var metaEventosPromocion = _context.EventosPromocion.
					Where(p => eventosIds.Contains(p.eventoId))
					.ToList();

				var ticketsPromoInfo = _context.TicketPromociones.
					Where(tp => (bougthPromos.Select(x => x.promoId)).
						Contains(tp.id)
					).ToList();

            var misTickets = payments.
               Select(pay => new
               {                  
						pay.id,
                  pay.codigo,
                  monto = pay.montoDec,
                  pay.eventoId,
                  pay.eventoFecha,
                  estado = getEstado(pay),
                  eventoLugar = pay.evento.lugar,
                  eventoJuego = pay.evento.juego,
                  pay.horarioId,
                  //pay.usuariosMayor4,
                  //pay.usuariosMenor4,
                  //pay.ticket3,
                  //pay.ticket4,
                  pay.createdOn,
                  //promocionesflat = pay.promociones,
						entradas = tickesDefinicion.Where(td => td.codigo == pay.evento.ticketDefinicion).Select((td, index) => new {
							tipo = td.tipo == "ADULTO" ? "ticket1" : td.tipo == "NOADULTO" ? "ticket2" : td.tipo == "ENTRADA3" ? "ticket3" : "ticket4",
							td.titulo,
							count = td.tipo == "ADULTO" ? pay.usuariosMayor4 : td.tipo == "NOADULTO" ? pay.usuariosMenor4 : td.tipo == "ENTRADA3" ? pay.ticket3 : pay.ticket4
						}).ToList(),
						promociones = bougthPromos.Where(bPromo => bPromo.paymentId == pay.id).Select((boughtPromo,index) => new { 
                     id = boughtPromo.paymentId,
                     boughtPromo.promoId,
							ticketsPromoInfo.Where(pro => pro.id == boughtPromo.promoId).FirstOrDefault().nombre,
							ticketsPromoInfo.Where(pro=>pro.id == boughtPromo.promoId).FirstOrDefault().descripcion,
                     ticket1= ticketsPromoInfo.Where(pro => pro.id == boughtPromo.promoId).FirstOrDefault().adultos,
							ticket2 = ticketsPromoInfo.Where(pro => pro.id == boughtPromo.promoId).FirstOrDefault().nihos,
							ticketsPromoInfo.Where(pro => pro.id == boughtPromo.promoId).FirstOrDefault().ticket3,
							ticketsPromoInfo.Where(pro => pro.id == boughtPromo.promoId).FirstOrDefault().ticket4,
                     boughtPromo.count,
						}).ToList()
               }).ToList().OrderByDescending(o => o.id);
            return misTickets;                                

            //List<PaymentInfo> result = new List<PaymentInfo>();
            //var _userId = new SqlParameter("@usuarioId", userId);
            //result = _context.PaymentInfos.FromSql("[dbo].[misTickets] @usuarioId", _userId).ToList();
            //foreach (PaymentInfo payment in result)
            //{
            //   makeTicketPromocionesList(payment);
            //   makeTicketTiposList(payment);
            //}

            //return result;
         }
         catch (Exception e)
         {
            return null;
         }
      }

      internal void makeTicketPromocionesList(PaymentInfo payment)
      {
         List<PromocionInfo> promocionesList = new List<PromocionInfo>();
         try
         {
            if (payment.promociones.Length > 0)
            {
               string[] promociones = payment.promociones.Split(",");
               foreach (var metaPromocion in promociones)
               {
                  string[] promocionData = metaPromocion.Split("|");

                  promocionesList.Add(
                     new PromocionInfo()
                     {
                        id = Convert.ToInt32(promocionData[0]),
                        nombre = promocionData[1],
                        adultos = Convert.ToInt32(promocionData[2]),
                        nihos = Convert.ToInt32(promocionData[3]),
                        precio = Convert.ToDecimal(promocionData[4])
                     }
                     );
               }
               payment.promocionesList = promocionesList;
            }
         }
         catch (Exception ex)
         {

         }
      }

      internal void makeTicketTiposList(PaymentInfo payment)
      {
         List<string> tipoTicketsList = new List<string>();
         try
         {
            if (payment.tipoTickets.Length > 0)
            {
               string[] tipoTickets = payment.tipoTickets.Split("|");
               foreach (var tipoTicket in tipoTickets)
               {
                  tipoTicketsList.Add(tipoTicket);
               }
               payment.tipoTicketsList = tipoTicketsList;
            }
         }
         catch (Exception ex)
         {

         }
      }

      [AllowAnonymous]
      [HttpGet("aforoinfo/{eventoId}/{pipedFecha}/{horarioId}")]
      public async Task<IActionResult> getAforoInfo(long eventoId, string pipedFecha, int horarioId)
      {
         try
         {
            var evento = _context.EventosORM.First(e => e.id == eventoId);

            if (evento.isCompraDirecta == 1)
            {
               var simpleAforo = getSimpleAforoDB(eventoId);

               var aforoInfo = new
               {
                  id = 0,
                  aforo = simpleAforo.aforo,
                  ocupados = simpleAforo.ocupados,
                  disponible = simpleAforo.disponible
               };

               return Ok(new { code = 1000, message = "SUCCESS", aforoInfo = aforoInfo });
            }
            else
            {
               string[] granularDate = pipedFecha.Split("|");
               DateTime eventoFecha = new DateTime(Convert.ToInt32(granularDate[2]), Convert.ToInt32(granularDate[1]), Convert.ToInt32(granularDate[0]));

               var horariosByEvento = getHorariosEventoDB(eventoId, eventoFecha);
               var horarioRequested = horariosByEvento.FirstOrDefault(horario => horario.id == horarioId);

               var aforoInfo = new
               {
                  id = horarioRequested.id,
                  aforo = horarioRequested.aforo,
                  ocupados = horarioRequested.aforo - horarioRequested.vacantes,
                  disponible = horarioRequested.vacantes
               };

               return Ok(new { code = 1000, message = "SUCCESS", aforoInfo = aforoInfo });
            }
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener el aforo" });
         }
      }

      [AllowAnonymous]
      [HttpGet("ticketspromociones/{eventoId}/{pipedfecha}")]
      public async Task<IActionResult> getTicketsPromociones(long eventoId, string pipedFecha)
      {
         try
         {
            string[] granularDate = pipedFecha.Split("|");
            DateTime fecha = new DateTime(Convert.ToInt32(granularDate[2]), Convert.ToInt32(granularDate[1]), Convert.ToInt32(granularDate[0]));

            List<TicketDefinicion> tickets = getTicketTiposByEventoIdDB(eventoId);
            List<TicketPromocion> promociones = getTicketPromocionesByEventoIdAndDiaDB(eventoId, fecha);

            return Ok(new { code = 1000, message = "Tickets y Promociones", tickets, promociones });
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener los Tickets y Promociones" });
         }
      }

      List<TicketDefinicion> getTicketTiposByEventoIdDB(long eventoId)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            return _context.TicketTipos.FromSql("[getTipoTicketByEventoId] @eventoId", _eventoId).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }
      List<TicketPromocion> getTicketPromocionesByEventoIdAndDiaDB(long eventoId, DateTime fecha)
      {
         try
         {
            var evento = _context.EventosORM.First(e => e.id == eventoId);

            var promocionesPorEvento = _context.EventosPromocion.Where(p => p.eventoId == eventoId).Select(p => p.promocionId).ToList();
            var promocionesIndividuales = _context.TicketPromociones.Where(tp => promocionesPorEvento.Contains(tp.id)).ToList();

            //TODO : SE PUEDE REFACTORIZAR SOLO CON EF
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _fecha = new SqlParameter("@fecha", fecha);
            var promocionesPorEventoyFecha = _context.TicketPromociones.FromSql("[getTicketPromocionesByEventoIdAndDia] @eventoId, @fecha", _eventoId, _fecha).ToList();

            promocionesIndividuales.AddRange(promocionesPorEventoyFecha);

            return promocionesIndividuales;
         }
         catch (Exception e)
         {
            return null;
         }
      }

      List<TicketPromocion> getTicketPromocionesAllDB(long eventoId, DateTime fecha)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _fecha = new SqlParameter("@fecha", fecha);
            return _context.TicketPromociones.FromSql("[getTicketPromocionesAllAndEventoIdAndDia] @eventoId, @fecha",_eventoId, _fecha).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [AllowAnonymous]
      [HttpGet("ticketscontrol/{eventoId}/{pipedFecha}")]
      public async Task<IActionResult> ticketsControlHoy(long eventoId, string pipedFecha)
      {
         try
         {
            string[] granularDate = pipedFecha.Split("|");
            DateTime fecha = new DateTime(Convert.ToInt32(granularDate[2]), Convert.ToInt32(granularDate[1]), Convert.ToInt32(granularDate[0]));

            List<TicketControl> rawTickets = getTicketsControlHoyByEventoDB(eventoId, fecha);
            CheckGroups(rawTickets);
            return Ok(new { code = 1000, message = "Tickets para Control por Evento", tickets = rawTickets });
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener los Tickets" });
         }
      }

      private void CheckGroups(List<TicketControl> tickets)
      {
         List<int> horariosGroup = new List<int>();
         foreach (var t in tickets)
         {
            if (!horariosGroup.Contains(t.horarioId))
            {
               horariosGroup.Add(t.horarioId);
            }
         }
         List<TicketGroup> ticketGroups = new List<TicketGroup>();
         foreach (var group in horariosGroup)
         {
            ticketGroups.Add(new TicketGroup() { id = group, items = new List<long>() });
         }

         foreach (var t in tickets)
         {
            foreach (var g in ticketGroups)
            {
               if (t.horarioId == g.id)
               {
                  g.items.Add(t.id);
               }
            }
         }

         foreach (var group in ticketGroups)
         {
            if (group.items.Count == 1)
            {
               group.middleIndex = group.items[0];
            }
            else
            {
               if (group.items.Count % 2 == 0)
               {
                  group.middleIndex = group.items[(group.items.Count / 2) - 1];
               }
               else
               {
                  group.middleIndex = group.items[Convert.ToInt32((group.items.Count / 2))];
               }
            }
         }

         foreach (var ticket in tickets)
         {
            foreach (var group in ticketGroups)
            {
               if (ticket.horarioId == group.id)
               {
                  group.totalAdults += ticket.adultos;
                  group.totalAdults += ticket.adultosPromocion;
                  group.totalKids += ticket.nihos;
                  group.totalKids += ticket.nihosPromocion;
               }
            }
         }

         foreach (var group in ticketGroups)
         {
            TicketControl tc = tickets.Where(t => t.id == group.middleIndex).FirstOrDefault();
            if (tc != null)
            {
               tc.isKey = true;
               tc.adultosGroup = group.totalAdults;
               tc.nihosGroup = group.totalKids;
            }
         }
      }


      List<TicketControl> getTicketsControlHoyByEventoDB(long eventoId, DateTime fecha)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _fecha = new SqlParameter("@fecha", fecha);
            return _context.TicketsControl.FromSql("[getTicketsByEventoId] @eventoId,@fecha", _eventoId, _fecha).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [AllowAnonymous]
      [HttpGet("marcarticket/{id}")]
      public async Task<IActionResult> marcarTicket(long id)
      {
         if (marcarTicketDB(id))
         {
            return Ok(new { code = 1000, message = "Ticket marcado" });
         }
         else
         {
            return Ok(new { code = 2000, message = "No se pudo marcar el Ticket" });
         }
      }
      bool marcarTicketDB(long id)
      {
         var _ticketId = new SqlParameter("@id", id);

         try
         {
            var sql = "EXEC dbo.marcarTicket @id";
            _context.Database.ExecuteSqlCommand(sql, _ticketId);
            return true;
         }
         catch (Exception)
         {
            return false;
         }
      }

      [HttpPost("gestionarpromociones")]
      public async Task<IActionResult> gestionarPromociones(PromocionProgramacion metaPromocion)
      {
         if (metaPromocion.promocionesFlat.Length > 0)
         {
            try
            {
               string[] promocionesArray = metaPromocion.promocionesFlat.Split("|");
               if (promocionesArray.Count()>0)
               {
                  eliminarPromocionesPorEventoYMes(metaPromocion.eventoId, metaPromocion.mes);
                  foreach (string promocion in promocionesArray)
                  {
                     string[] promocionBody = promocion.Split("<>");
                     PromocionFecha oPromocion = new PromocionFecha()
                     {
                        dia = Convert.ToInt32(promocionBody[0]),
                        nombre = Convert.ToString(promocionBody[1]),
                        cantidada = Convert.ToInt32(promocionBody[2]),
                        cantidadb = Convert.ToInt32(promocionBody[3]),
                        precio = Convert.ToDecimal(promocionBody[4]),
                        descripcion = Convert.ToString(promocionBody[5])
                     };

                     gestionarEntradaPromocionDB(1, new TicketPromocion()
                     {
                        id = 0,
                        eventoId = metaPromocion.eventoId,
                        tipo = "Fecha",
                        fecha = new DateTime(DateTime.UtcNow.Year, metaPromocion.mes, oPromocion.dia),
                        nombre = oPromocion.nombre,
                        adultos = oPromocion.cantidada,
                        nihos = oPromocion.cantidadb,
                        precio = oPromocion.precio,
                        descripcion = oPromocion.descripcion
                     }); ;
                  }
                  return Ok(new { code = 1000, message = "Promociones guardadas" });
               }
               else
               {
                  return Ok(new { code = 2000, message = "No se encontraron Promociones" });
               }
            }
            catch (Exception e)
            {
               return Ok(new { code = 3000, message = "No se pudo guardar las Promociones " + e.Message });
            }
         }
         else
         {
            return Ok(new { code = 2000, message = "No hay informacion de Promociones" });
         }
      }

      void eliminarPromocionesPorEventoYMes(long eventoId, int mes)
      {
         var _eventoId = new SqlParameter("@eventoId", eventoId);
         var _mes = new SqlParameter("@mes", mes);

         var sql = "EXEC dbo.eliminarPromocionesPorEventoYMes @eventoId,@mes";
         _context.Database.ExecuteSqlCommand(sql, _eventoId, _mes);
      }

      [HttpGet("promocioneseventomes/{eventoId}/{mes}")]
      public async Task<IActionResult> getPromocionesPorEventoYMes(long eventoId, int mes)
      {
         try
         {
            List<PromocionFecha> promociones = getPromocionesPorEventoYMesDB(eventoId, mes);

            return Ok(new { code = 1000, message = "Promociones por Mes", promociones });
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener las Promociones" });
         }
      }

      List<PromocionFecha> getPromocionesPorEventoYMesDB(long eventoId, int mes)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _mes = new SqlParameter("@mes", mes);
            return _context.PromocionesFecha.FromSql("[getPromocionesPorEventoYMes] @eventoId,@mes", _eventoId, _mes).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }
   }
}