using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Armadar.Helpers;
using ClosedXML.Excel;
using ControlOne.AdminService.Data;
using ControlOne.AdminService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ControlOne.AdminService.Controllers
{
    [Authorize]
   [Route("[controller]")]
   [ApiController]
   public partial class AdminController : ControllerBase
   {
      private readonly AdminContext _context;
      private readonly AppSettings _appSettings;

      public AdminController(AdminContext context, IOptions<AppSettings> appSettings)
      {
         _context = context;
         _appSettings = appSettings.Value;
      }

      [HttpGet("getcajas/{eventoid}/{desde}/{hasta}")]
      public async Task<IActionResult> GetCajas(long eventoid, DateTime desde, DateTime hasta)
      {
         try
         {
            dynamic response = new System.Dynamic.ExpandoObject();

            List<CajaView> resultado = new List<CajaView>();
            resultado = getCajasDB(eventoid, desde, hasta);
            foreach (CajaView caja in resultado)
            {
               if (caja.faltanteVenta > caja.sobranteVenta)
               {
                  caja.sobranteVenta = caja.faltanteVenta;
               }
            }

            response.cajas = resultado;

            return Ok(response);
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener las cajas" });
         }
      }

      List<CajaView> getCajasDB(long eventoId, DateTime desde, DateTime hasta)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _desde = new SqlParameter("@desde", desde);
            var _hasta = new SqlParameter("@hasta", hasta);

            return _context.CajaViews.FromSql("getCajas @eventoId,@desde,@hasta", _eventoId, _desde, _hasta).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      /*
      // GET: api/Admin
      [HttpGet]
      public IEnumerable<string> Get()
      {
          return new string[] { "value1", "value2" };
      }

      // GET: api/Admin/5
      [HttpGet("{id}", Name = "Get")]
      public string Get(int id)
      {
          return "value";
      }

      // POST: api/Admin
      [HttpPost]
      public void Post([FromBody] string value)
      {
      }

      // PUT: api/Admin/5
      [HttpPut("{id}")]
      public void Put(int id, [FromBody] string value)
      {
      }

      // DELETE: api/ApiWithActions/5
      [HttpDelete("{id}")]
      public void Delete(int id)
      {
      }
      */

      [HttpGet("getdescuentos/{eventoid}/{anio}/{mes}/{dia}")]
      public IActionResult getDescuentosForExport(long eventoId, int anio, int mes, int dia)
      {
         List<Descuento> descuentos = getDescuentosByDiaDB(eventoId, anio, mes, dia);

         using (MemoryStream stream = new MemoryStream())
         {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "Descuentos" };

            int columns = 6;
            int rowStart = 3;

            foreach (var sheetname in SheetNames)
            {
               var worksheet = workbook.Worksheets.Add(sheetname);
               worksheet.ColumnWidth = 20;
               for (int i = 1; i <= columns; i++)
               {
                  worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
               }

               //Titles
               worksheet.Cell("A1").Value = "Hora";
               worksheet.Cell("B1").Value = "Apoderado";
               worksheet.Cell("C1").Value = "Celular";
               worksheet.Cell("D1").Value = "Usuario";
               worksheet.Cell("E1").Value = "Dscto";
               worksheet.Cell("F1").Value = "Motivo";

               //Headers Format
               var rngTable = worksheet.Range("A1:F1");
               rngTable.Style
               .Font.SetBold()
               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

               worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               decimal totalDsctos = 0;

               descuentos.ForEach(item =>
               {
                  worksheet.Cell("A" + rowStart).Value = item.fecha.ToShortTimeString();
                  worksheet.Cell("B" + rowStart).Value = item.apoderado;
                  worksheet.Cell("C" + rowStart).Value = item.celular;
                  worksheet.Cell("D" + rowStart).Value = item.usuario;
                  worksheet.Cell("E" + rowStart).Value = item.monto;
                  worksheet.Column(5).Style.NumberFormat.NumberFormatId = 2;
                  worksheet.Cell("F" + rowStart).Value = item.motivo;

                  rowStart++;
                  totalDsctos += item.monto;
               });

               int rowTotales = rowStart + 1;

               worksheet.Cell("D" + rowTotales).Value = "Total";
               worksheet.Cell("D" + rowTotales).Style.Font.SetBold();
               worksheet.Cell("E" + rowTotales).Value = totalDsctos;
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                // By setting a file download name the framework will
                // automatically add the attachment Content-Disposition header
                fileDownloadName: "Descuentos.xlsx"
            );
         }
      }

      [HttpGet("getgastos/{eventoid}/{anio}/{mes}/{dia}")]
      public IActionResult getGastosForExport(long eventoId, int anio, int mes, int dia)
      {
         List<Concepto> gastos = getGastosDB(eventoId, anio, mes, dia);

         using (MemoryStream stream = new MemoryStream())
         {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "Gastos" };

            int columns = 6;
            int rowStart = 3;

            foreach (var sheetname in SheetNames)
            {
               var worksheet = workbook.Worksheets.Add(sheetname);
               worksheet.ColumnWidth = 20;
               for (int i = 1; i <= columns; i++)
               {
                  worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
               }

               //Titles
               worksheet.Cell("A1").Value = "Concepto";
               worksheet.Cell("B1").Value = "Importe";

               //Headers Format
               var rngTable = worksheet.Range("A1:B1");
               rngTable.Style
               .Font.SetBold()
               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

               worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               decimal total = 0;

               gastos.ForEach(item =>
               {
                  worksheet.Cell("A" + rowStart).Value = item.concepto;
                  worksheet.Cell("B" + rowStart).Value = item.importe;
                  worksheet.Column(2).Style.NumberFormat.NumberFormatId = 2;

                  rowStart++;
                  total += item.importe;
               });

               int rowTotales = rowStart + 1;

               worksheet.Cell("A" + rowTotales).Value = "Total";
               worksheet.Cell("B" + rowTotales).Style.Font.SetBold();
               worksheet.Cell("B" + rowTotales).Value = total;
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                // By setting a file download name the framework will
                // automatically add the attachment Content-Disposition header
                fileDownloadName: "Gastos.xlsx"
            );
         }
      }

      [HttpGet("getapoderados")]
      public IActionResult getApoderadosForExport()
      {
         List<ApoderadoExcel> gastos = getApoderadosDB();

         using (MemoryStream stream = new MemoryStream())
         {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "Apoderados" };

            int columns = 5;
            int rowStart = 3;

            foreach (var sheetname in SheetNames)
            {
               var worksheet = workbook.Worksheets.Add(sheetname);
               worksheet.ColumnWidth = 20;
               for (int i = 1; i <= columns; i++)
               {
                  worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
               }

               //Titles
               worksheet.Cell("A1").Value = "Id";
               worksheet.Cell("B1").Value = "Nombres";
               worksheet.Cell("C1").Value = "DNI";
               worksheet.Cell("D1").Value = "Celular";
               worksheet.Cell("E1").Value = "Email";

               //Headers Format
               var rngTable = worksheet.Range("A1:E1");
               rngTable.Style
               .Font.SetBold()
               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

               worksheet.Cell("A1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               gastos.ForEach(item =>
               {
                  worksheet.Cell("A" + rowStart).Value = item.id;
                  worksheet.Cell("B" + rowStart).Value = item.nombres;
                  worksheet.Cell("C" + rowStart).Value = item.dni;
                  worksheet.Cell("D" + rowStart).Value = item.celular;
                  worksheet.Cell("E" + rowStart).Value = item.email;

                  rowStart++;
               });
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                // By setting a file download name the framework will
                // automatically add the attachment Content-Disposition header
                fileDownloadName: "Apoderados.xlsx"
            );
         }
      }

      List<ApoderadoExcel> getApoderadosDB()
      {
         try
         {
            return _context.ApoderadosExcel.FromSql("getApoderados").ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("getexportar/{eventoid}/{desde}/{hasta}")]
      public IActionResult getCajasForExport(long eventoId, DateTime desde, DateTime hasta)
      {
         //Get Evento Info
         string lugar = "Todos";
         string juego = string.Empty;

         if (eventoId != 0)
         {
            Evento evento = _context.Eventos.SingleOrDefault(x => x.id == eventoId);
            if (evento != null)
            {
               lugar = evento.lugar;
               juego = evento.juego;
            }
         }

         string rangoFechas = $"Del {desde.ToShortDateString()} al {hasta.ToShortDateString()}";

         using (MemoryStream stream = new MemoryStream())
         {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "Ventas" };

            List<CajaView> cajas = getCajasDB(eventoId, desde, hasta);
            int rowStart = 5;

            foreach (var sheetname in SheetNames)
            {
               var worksheet = workbook.Worksheets.Add(sheetname);
               worksheet.ColumnWidth = 20;
               for (int i = 1; i <= 10; i++)
               {
                  worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
               }

               for (int i = 2; i <= 10; i++)
               {
                  worksheet.Column(i).Style.NumberFormat.NumberFormatId = 2;
               }

               //Titles
               worksheet.Cell("D1").Value = lugar;
               worksheet.Cell("E1").Value = juego;
               worksheet.Cell("F1").Value = rangoFechas;
               worksheet.Range("F1:G1").Merge();
               worksheet.Row(1).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               //Headers Group
               worksheet.Cell("B3").Value = "Ventas";
               worksheet.Range("B3:E3").Merge();

               worksheet.Cell("F3").Value = "Abonos";
               worksheet.Range("F3:G3").Merge();
               worksheet.Row(3).Style.Font.SetBold().Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               //Headers
               worksheet.Cell("A4").Value = "Fecha";
               worksheet.Cell("B4").Value = "Total";
               worksheet.Cell("C4").Value = "Efectivo";
               worksheet.Cell("D4").Value = "POS";
               worksheet.Cell("E4").Value = "Otros";
               worksheet.Cell("F4").Value = "Retiro Efectivo";
               worksheet.Cell("G4").Value = "Deposito Cta.";
               worksheet.Cell("H4").Value = "Sob / Fal";
               worksheet.Cell("I4").Value = "Gastos";
               worksheet.Cell("J4").Value = "Dsctos";

               //Headers Format
               var rngTable = worksheet.Range("A4:J4");
               rngTable.Style
               .Font.SetBold()
               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               decimal totalVentasTotal = 0, totalVentasEfectivo = 0, totalVentasPOS = 0, totalVentasOtros = 0, totalRetiroEfectivo = 0, totalDepositos = 0, totalSobFal = 0, totalGastos = 0, totalDsctos = 0;

               cajas.ForEach(v =>
               {
                  worksheet.Cell("A" + rowStart).Value = v.fecha.ToShortDateString();
                  worksheet.Cell("B" + rowStart).Value = v.ventaTotal;
                  worksheet.Cell("C" + rowStart).Value = v.ventaEfectivo;
                  worksheet.Cell("D" + rowStart).Value = v.ventaPOS;
                  worksheet.Cell("E" + rowStart).Value = v.ventaOtros;
                  worksheet.Cell("F" + rowStart).Value = v.retiros;
                  worksheet.Cell("G" + rowStart).Value = v.abonos;
                  worksheet.Cell("H" + rowStart).Value = Convert.ToDecimal("0.00");
                  if (v.sobranteVenta > v.faltanteVenta) { worksheet.Cell("H" + rowStart).Value = v.sobranteVenta; }
                  if (v.faltanteVenta > v.sobranteVenta) { worksheet.Cell("H" + rowStart).Value = v.faltanteVenta; }
                  worksheet.Cell("I" + rowStart).Value = v.gastos;
                  worksheet.Cell("J" + rowStart).Value = v.dscto;

                  rowStart++;
                  totalVentasTotal += v.ventaTotal;
                  totalVentasEfectivo += v.ventaEfectivo;
                  totalVentasPOS += v.ventaPOS;
                  totalVentasOtros += v.ventaOtros;
                  totalRetiroEfectivo += v.retiros;
                  totalDepositos += v.abonos;
                  if (v.sobranteVenta == v.faltanteVenta) { totalSobFal += 0; }
                  if (v.sobranteVenta > v.faltanteVenta) { totalSobFal += v.sobranteVenta; }
                  if (v.faltanteVenta > v.sobranteVenta) { totalSobFal += v.faltanteVenta; }
                  totalGastos += v.gastos;
                  totalDsctos += v.dscto;
               });

               int rowTotales = rowStart + 1;

               worksheet.Cell("A" + rowTotales).Value = "Totales";
               worksheet.Cell("A" + rowTotales).Style.Font.SetBold();
               worksheet.Cell("B" + rowTotales).Value = totalVentasTotal;
               worksheet.Cell("C" + rowTotales).Value = totalVentasEfectivo;
               worksheet.Cell("D" + rowTotales).Value = totalVentasPOS;
               worksheet.Cell("E" + rowTotales).Value = totalVentasOtros;
               worksheet.Cell("F" + rowTotales).Value = totalRetiroEfectivo;
               worksheet.Cell("G" + rowTotales).Value = totalDepositos;
               worksheet.Cell("H" + rowTotales).Value = totalSobFal;
               worksheet.Cell("I" + rowTotales).Value = totalGastos;
               worksheet.Cell("J" + rowTotales).Value = totalDsctos;
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                // By setting a file download name the framework will
                // automatically add the attachment Content-Disposition header
                fileDownloadName: "Ventas.xlsx"
            );
         }
      }

      [HttpGet("geteventos/")]
      public IActionResult getEventos()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         var eventos = _context.Eventos.Where(e => e.activo == 1);
         response.eventos = eventos;
         return Ok(response);
      }      

      public IActionResult getCajasForExport_0(long eventoId, DateTime desde, DateTime hasta)
      {
         using (MemoryStream stream = new MemoryStream())
         {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "Ventas" };

            List<CajaView> cajas = getCajasDB(eventoId, desde, hasta);
            int rowStart = 5;

            foreach (var sheetname in SheetNames)
            {
               var worksheet = workbook.Worksheets.Add(sheetname);
               worksheet.ColumnWidth = 20;
               worksheet.Column(1).Width = 40;
               for (int i = 2; i <= 8; i++)
               {
                  worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
               }

               for (int i = 3; i <= 8; i++)
               {
                  worksheet.Column(i).Style.NumberFormat.NumberFormatId = 2;
               }

               //Headers
               worksheet.Cell("A4").Value = "Lugar";
               worksheet.Cell("B4").Value = "Fecha";
               worksheet.Cell("C4").Value = "Venta Total";
               worksheet.Cell("D4").Value = "Venta Efectivo";
               worksheet.Cell("E4").Value = "Venta POS";
               worksheet.Cell("F4").Value = "Retiro Efectivo";
               worksheet.Cell("G4").Value = "Deposito Cta.";
               worksheet.Cell("H4").Value = "Sob / Fal";

               //Headers Format
               var rngTable = worksheet.Range("A4:H4");
               rngTable.Style
               .Font.SetBold()
               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

               cajas.ForEach(v =>
               {
                  worksheet.Cell("A" + rowStart).Value = v.Lugar;
                  worksheet.Cell("B" + rowStart).Value = v.fecha.ToShortDateString();
                  worksheet.Cell("C" + rowStart).Value = v.ventaTotal;
                  worksheet.Cell("D" + rowStart).Value = v.ventaEfectivo;
                  worksheet.Cell("E" + rowStart).Value = v.ventaPOS;
                  worksheet.Cell("F" + rowStart).Value = v.retiros;
                  worksheet.Cell("G" + rowStart).Value = v.abonos;
                  worksheet.Cell("H" + rowStart).Value = Convert.ToDecimal("0.00");
                  if (v.sobranteVenta > v.faltanteVenta) { worksheet.Cell("H" + rowStart).Value = v.sobranteVenta; }
                  if (v.faltanteVenta > v.sobranteVenta) { worksheet.Cell("H" + rowStart).Value = v.faltanteVenta; }
                  rowStart++;
               });
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                // By setting a file download name the framework will
                // automatically add the attachment Content-Disposition header
                fileDownloadName: "Ventas.xlsx"
            );
         }
      }

      long GetSafeEvento()
      {
         var principal = HttpContext.User;
         if (principal?.Claims != null)
         {
            var evento = principal.Claims.Where(c => c.Type == "evento")
               .Select(c => c.Value).SingleOrDefault();
            return evento != null ? Convert.ToInt64(evento) : 0;
         }
         else
         {
            return 0;
         }
      }

      [HttpGet("comovamos/")]
      public IActionResult getComoVamos()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.comovamos = getComoVamosDB();
         response.horaactual = getHoraActualDB().First().horaActual;
         return Ok(response);
      }

      List<EventoResumen> getComoVamosDB()
      {
         try
         {
            return _context.EventoResumenes.FromSql("comovamos").ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      List<HoraActual> getHoraActualDB()
      {
         try
         {
            return _context.HorasActuales.FromSql("gethoraactual").ToList();
         }
         catch (Exception e)
         {

            return null;
         }
      }

      List<Descuento> getDescuentosByDiaDB(long evento, int anio, int mes, int dia)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", evento);
            var _anio = new SqlParameter("@anio", anio);
            var _mes = new SqlParameter("@mes", mes);
            var _dia = new SqlParameter("@dia", dia);
            return _context.Descuentos.FromSql("getDescuentosByDia @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes, _dia).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("eventostodos/")]
      public IActionResult getEventosRows()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.eventos = getEventosRowsDB();
         return Ok(response);
      }
      List<EventoRow> getEventosRowsDB()
      {
         try
         {
            return _context.EventoRows.FromSql("getEventosAll").ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("entradas/")]
      public IActionResult getEntradasRows()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.entradas = getEntradasRowsDB();
         return Ok(response);
      }
      List<EntradaRow> getEntradasRowsDB()
      {
         try
         {
            return _context.EntradaRows.FromSql("getEntradas").ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("entradapromociones/")]
      public IActionResult getEntradaPromociones()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.promociones = getEntradaPromocionesDB();
         return Ok(response);
      }
      List<TicketPromocion> getEntradaPromocionesDB()
      {
         try
         {
            return _context.EntradaPromocionesRows.FromSql("getEntradaPromociones").ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

        [AllowAnonymous]
        [HttpPost("subirimagen")]
        public async Task<ActionResult> subirImagen([FromForm] IFormFile formData)
        {
            var file = Request.Form.Files[0];
            string fileName = file.Name + Path.GetExtension(file.FileName);
            string uploadUrl = String.Format("{0}/{1}/{2}", "ftp://ftp.experienciasxtreme.com", "", "game-" + fileName);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = new NetworkCredential("xp_user_games", "usergames_");           
            request.Method = WebRequestMethods.Ftp.UploadFile;

            try
            {
                byte[] fileContents = null;
                if (file.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        fileContents = ms.ToArray();
                    }
                }

                // Copy the contents of the file to the request stream.
                request.ContentLength = fileContents.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {

                return Ok(new { code = 2000, message = "No se pudo guardar la imagen" });
            }
                        
            return Ok(new { code = 1000, message = "Imagen guardada" });
        }

        [HttpPost("gestionarevento/{tipo}")]
      public async Task<IActionResult> gestionarEvento(int tipo, EventoRow evento)
      {
         try
         {
            evento.horaInicio = evento.horaInicio.ToUniversalTime().AddHours(-5);
            evento.horaFinal = evento.horaFinal.ToUniversalTime().AddHours(-5);
            gestionarEventoDB(tipo, evento);

            return Ok(new { code = 1000, message = "Evento guardado" });
         }
         catch (Exception e)
         {
            return BadRequest(new { code = 3000, message = "No se pudo guardar el Evento" });
         }
      }
      void gestionarEventoDB(int tipoAccion, EventoRow evento)
      {
         var tipo = new SqlParameter("@tipo", tipoAccion);
         var id = new SqlParameter("@id", evento.id);
         var lugar = new SqlParameter("@lugar", evento.lugar);
         var juego = new SqlParameter("@juego", evento.juego);
         var aforo = new SqlParameter("@aforo", evento.aforo);
         var operadorZona = new SqlParameter("@operadorZona", evento.operadorZonaEmail);
         var operadorJuego = new SqlParameter("@operadorJuego", evento.operadorJuegoEmail);
         var password = new SqlParameter("@password", evento.passwordZona);
         var password2 = new SqlParameter("@password2", evento.passwordJuego);

         var cajaInicial = new SqlParameter("@cajaInicial", evento.cajaInicial);
         var tarifaMinutos = new SqlParameter("@tarifaMinutos", evento.tarifaMinutos);
         var tarifaPrecioMinuto = new SqlParameter("@tarifaPrecioMinuto", evento.tarifaPrecioMinuto);
         var tarifaPrecioMinutoAdicional = new SqlParameter("@tarifaPrecioMinutoAdicional", evento.tarifaPrecioMinutoAdicional);

         var activo = new SqlParameter("@activo", evento.activo);
         var ticketDefinicion = new SqlParameter("@ticketDefinicion", evento.ticketDefinicion);
         var ticketsPromociones = new SqlParameter("@ticketsPromociones", evento.ticketsPromociones);

         var horaInicio = new SqlParameter("@horaInicio", evento.horaInicio);
         var horaFinal = new SqlParameter("@horaFinal", evento.horaFinal);
         var descripcion = new SqlParameter("@descripcion", evento.descripcion);
         var inicio = new SqlParameter("@inicio", evento.inicio);
         var final = new SqlParameter("@final", evento.final);
            var isEntradaOnline = new SqlParameter("@isEntradaOnline", evento.isEntradaOnline);

            var sql = "EXEC dbo.gestionarEvento @tipo,@id,@lugar,@juego,@aforo,@operadorZona,@operadorJuego,@password,@password2,@cajaInicial,@tarifaMinutos,@tarifaPrecioMinuto,@tarifaPrecioMinutoAdicional,@activo,@ticketDefinicion,@ticketsPromociones,@horaInicio,@horaFinal,@descripcion,@inicio,@final,@isEntradaOnline";
            _context.Database.ExecuteSqlCommand(sql, tipo, id, lugar, juego, aforo, operadorZona, operadorJuego, password, password2, cajaInicial, tarifaMinutos, tarifaPrecioMinuto, tarifaPrecioMinutoAdicional, activo, ticketDefinicion, ticketsPromociones, horaInicio, horaFinal, descripcion, inicio, final, isEntradaOnline);
      }

      [HttpPost("gestionarpromocion/{tipo}")]
      public async Task<IActionResult> gestionarPromocion(int tipo, PromocionRow promocion)
      {
         try
         {
            if (tipo != 3)
            {
               DateTime inicioFixed = new DateTime(promocion.inicio.Year, promocion.inicio.Month, promocion.inicio.Day);
               DateTime finalFixed = new DateTime(promocion.final.Year, promocion.final.Month, promocion.final.Day);

               DateTime inicioFixed2 = inicioFixed.AddHours(promocion.inicioHora).AddMinutes(promocion.inicioMinuto);
               DateTime finalFixed2 = finalFixed.AddHours(promocion.finalHora).AddMinutes(promocion.finalMinuto);

               promocion.inicio = inicioFixed2;
               promocion.final = finalFixed2;

               gestionarPromocionDB(tipo, promocion);

               return Ok(new { code = 1000, message = "Promocion guardada" });
            }
            else
            {
               eliminarPromocionDB(promocion.id);

               return Ok(new { code = 1000, message = "Promocion Eliminada" });
            }
         }
         catch (Exception e)
         {
            return BadRequest(new { code = 3000, message = "No se pudo guardar la promocion" });
         }
      }

      [HttpPost("gestionarentrada/{operacion}")]
      public async Task<IActionResult> gestionarEntrada(int operacion, EntradaRequest entrada)
      {
         try
         {
            if (entrada.tipo == "EG" || entrada.tipo == "EN")
            {
               EntradaRow entradaRow = new EntradaRow()
               {
                  id = entrada.id,
                  codigo = entrada.codigo,
                  tipo = entrada.tipo == "EG" ? "ADULTO" : "NOADULTO",
                  titulo = entrada.tipo == "EG" ? entrada.tituloAdulto : entrada.tituloNoAdulto,
                  icono = entrada.tipo == "EG" ? "adult.svg" : "kid.svg",
                  mensaje = entrada.tipo == "EG" ? entrada.mensajeAdulto : entrada.mensajeNoAdulto,
                  precio = entrada.tipo == "EG" ? entrada.precioAdulto : entrada.precioNoAdulto,
               };
               gestionarEntradaDB(operacion, entradaRow);
               return Ok(new { code = 1000, message = "Entrada guardada" });
            }
            else
            {
               List<EntradaRow> allEntradas = getEntradasRowsDB();
               int adultoId = 0;
               int noAdultoId = 0;

               if (operacion == 0)// Obener Ids solo si se esta actualizando
               {
                  adultoId = allEntradas.Where(ent => ent.codigo == entrada.codigo && ent.tipo == "ADULTO").FirstOrDefault().id;
                  noAdultoId = allEntradas.Where(ent => ent.codigo == entrada.codigo && ent.tipo == "NOADULTO").FirstOrDefault().id;
               }

               EntradaRow entradaRowAdulto = new EntradaRow()
               {
                  id = adultoId,
                  codigo = entrada.codigo,
                  tipo = "ADULTO",
                  titulo = entrada.tituloAdulto,
                  icono = "adult.svg",
                  mensaje = entrada.mensajeAdulto,
                  precio = entrada.precioAdulto
               };
               EntradaRow entradaRowNoAdulto = new EntradaRow()
               {
                  id = noAdultoId,
                  codigo = entrada.codigo,
                  tipo = "NOADULTO",
                  titulo = entrada.tituloNoAdulto,
                  icono = "kid.svg",
                  mensaje = entrada.mensajeNoAdulto,
                  precio = entrada.precioNoAdulto
               };
               gestionarEntradaDB(operacion, entradaRowAdulto);
               gestionarEntradaDB(operacion, entradaRowNoAdulto);
               return Ok(new { code = 1000, message = "Entrada guardada" });
            }
         }
         catch (Exception e)
         {
            return BadRequest(new { code = 3000, message = "No se pudo guardar la Entrada" });
         }
      }

      void gestionarEntradaDB(int operacionId, EntradaRow entrada)
      {
         try
         {
            var operacion = new SqlParameter("@operacion", operacionId);
            var id = new SqlParameter("@id", entrada.id);
            var codigo = new SqlParameter("@codigo", entrada.codigo);
            var tipo = new SqlParameter("@tipo", entrada.tipo);
            var titulo = new SqlParameter("@titulo", entrada.titulo);
            var icono = new SqlParameter("@icono", entrada.icono);
            var mensaje = new SqlParameter("@mensaje", entrada.mensaje);
            var precio = new SqlParameter("@precio", entrada.precio);

            var sql = "EXEC dbo.gestionarEntrada @operacion,@id,@codigo,@tipo,@titulo,@icono,@mensaje,@precio";
            _context.Database.ExecuteSqlCommand(sql, operacion, id, codigo, tipo, titulo, icono, mensaje, precio);
         }
         catch (Exception)
         {
         }         
      }

      [HttpPost("gestionarentradapromocion/{tipo}")]
      public async Task<IActionResult> gestionarEntradaPromocion(int tipo, TicketPromocion promocion)
      {
         try
         {
            gestionarEntradaPromocionDB(tipo, promocion);
            return Ok(new { code = 1000, message = "Promocion guardada" });
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo guardar la promocion" });
         }
      }
      void gestionarEntradaPromocionDB(int operacionId, TicketPromocion promocion)
      {
         try
         {
            var operacion = new SqlParameter("@operacion", operacionId);
            var id = new SqlParameter("@id", promocion.id);
            var eventoId = new SqlParameter("@eventoId", promocion.eventoId);
            var tipo = new SqlParameter("@tipo", promocion.tipo);
            var fecha = new SqlParameter("@fecha", promocion.fecha);
            var nombre = new SqlParameter("@nombre", promocion.nombre);
            var adultos = new SqlParameter("@adultos", promocion.adultos);
            var nihos = new SqlParameter("@nihos", promocion.nihos);
            var precio = new SqlParameter("@precio", promocion.precio);
            var descripcion = new SqlParameter("@descripcion", promocion.descripcion);

            var sql = "EXEC dbo.gestionarEntradaPromocion @operacion,@id,@eventoId,@tipo,@fecha,@nombre,@adultos,@nihos,@precio,@descripcion";
            _context.Database.ExecuteSqlCommand(sql, operacion, id, eventoId, tipo, fecha, nombre, adultos, nihos, precio, descripcion);
         }
         catch (Exception)
         {
         }
      }

      void gestionarPromocionDB(int tipoAccion, PromocionRow promocion)
      {
         var tipo = new SqlParameter("@tipo", tipoAccion);
         var id = new SqlParameter("@id", promocion.id);
         var eventoId = new SqlParameter("@eventoId", promocion.eventoId);
         var minutos = new SqlParameter("@minutos", promocion.minutos);
         var precioMinuto = new SqlParameter("@precioMinuto", promocion.precio);
         var precioMinutoAdicuonal = new SqlParameter("@precioMinutoAdicuonal", promocion.precioAdicional);
         var inicio = new SqlParameter("@inicio", promocion.inicio);
         var final = new SqlParameter("@final", promocion.final);


         var sql = "EXEC dbo.gestionarPromocion @tipo,@id,@eventoId,@minutos,@precioMinuto,@precioMinutoAdicuonal,@inicio,@final";
         _context.Database.ExecuteSqlCommand(sql, tipo, id, eventoId, minutos, precioMinuto, precioMinutoAdicuonal, inicio, final);
      }

      void eliminarPromocionDB(long promocionId)
      {
         var _promocionId = new SqlParameter("@promocionId", promocionId);

         var sql = "EXEC dbo.eliminarPromocion @promocionId";
         _context.Database.ExecuteSqlCommand(sql, _promocionId);
      }

      [HttpPost("gestionarpromocionempresa/{tipo}")]
      public async Task<IActionResult> gestionarPromocionEmpresa(int tipo, PromocionEmpresaRow promocion)
      {
         try
         {
            if (tipo != 3)
            {
               DateTime inicioFixed = new DateTime(promocion.inicio.Year, promocion.inicio.Month, promocion.inicio.Day);
               DateTime finalFixed = new DateTime(promocion.final.Year, promocion.final.Month, promocion.final.Day);

               promocion.inicio = inicioFixed;
               promocion.final = finalFixed;

               gestionarPromocionEmpresaDB(tipo, promocion);

               return Ok(new { code = 1000, message = "Promocion guardada" });
            }
            else
            {
               promocion.inicio = DateTime.Now;
               promocion.final = DateTime.Now;
               promocion.empresa = string.Empty;
               gestionarPromocionEmpresaDB(tipo, promocion);
               return Ok(new { code = 1000, message = "Promocion Eliminada" });
            }
         }
         catch (Exception e)
         {
            return BadRequest(new { code = 3000, message = "No se pudo guardar la promocion" });
         }
      }

      void gestionarPromocionEmpresaDB(int tipoAccion, PromocionEmpresaRow promocion)
      {
         var tipo = new SqlParameter("@tipo", tipoAccion);
         var id = new SqlParameter("@id", promocion.id);
         var empresa = new SqlParameter("@empresa", promocion.empresa);
         var descuento = new SqlParameter("@descuento", promocion.descuento);
         var inicio = new SqlParameter("@inicio", promocion.inicio);
         var final = new SqlParameter("@final", promocion.final);


         var sql = "EXEC dbo.gestionarPromocionEmpresa @tipo,@id,@empresa,@descuento,@inicio,@final";
         _context.Database.ExecuteSqlCommand(sql, tipo, id, empresa, descuento, inicio, final);
      }

      [HttpGet("getedadinfo/{eventoid}/{anio}/{mes}/{dia}")]
      public IActionResult getEdadInfoForExport(long eventoId, int anio, int mes, int dia)
      {
         List<EdadInfo> items = getEdadInfoDB(eventoId, anio, mes, dia);

         if (items != null)
         {
            using (MemoryStream stream = new MemoryStream())
            {
               var workbook = new XLWorkbook();
               var SheetNames = new List<string>() { "Edad Info" };

               int columns = 2;
               int rowStart = 3;

               foreach (var sheetname in SheetNames)
               {
                  var worksheet = workbook.Worksheets.Add(sheetname);
                  worksheet.ColumnWidth = 20;
                  for (int i = 1; i <= columns; i++)
                  {
                     worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                  }

                  //Titles
                  worksheet.Cell("A1").Value = "Edad";
                  worksheet.Cell("B1").Value = "Total";

                  //Headers Format
                  var rngTable = worksheet.Range("A1:B1");
                  rngTable.Style
                  .Font.SetBold()
                  .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                  items.ForEach(item =>
                  {
                     worksheet.Cell("A" + rowStart).Value = item.edad;
                     worksheet.Cell("B" + rowStart).Value = item.total;

                     rowStart++;
                  });
               }

               workbook.SaveAs(stream);
               stream.Seek(0, SeekOrigin.Begin);

               return this.File(
                   fileContents: stream.ToArray(),
                   contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                   // By setting a file download name the framework will
                   // automatically add the attachment Content-Disposition header
                   fileDownloadName: "EdadInfo.xlsx"
               );
            }
         }
         else
         {
            return null;
         }
      }

      List<EdadInfo> getEdadInfoDB(long evento, int anio, int mes, int dia)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", evento);
            var _anio = new SqlParameter("@anio", anio);
            var _mes = new SqlParameter("@mes", mes);
            var _dia = new SqlParameter("@dia", dia);
            return _context.EdadInfos.FromSql("getinfoByEdades @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes, _dia).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("getcanceladosinfo/{eventoid}/{anio}/{mes}/{dia}")]
      public IActionResult getCanceladosForExport(long eventoId, int anio, int mes, int dia)
      {
         List<CanceladoInfo> descuentos = getCanceladosByDiaDB(eventoId, anio, mes, dia);

         using (MemoryStream stream = new MemoryStream())
         {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "Cancelados" };

            int columns = 4;
            int rowStart = 3;

            foreach (var sheetname in SheetNames)
            {
               var worksheet = workbook.Worksheets.Add(sheetname);
               worksheet.ColumnWidth = 20;
               for (int i = 1; i <= columns; i++)
               {
                  worksheet.Column(i).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
               }

               //Titles
               worksheet.Cell("A1").Value = "Fecha";
               worksheet.Cell("B1").Value = "Apoderado";
               worksheet.Cell("C1").Value = "Celular";
               worksheet.Cell("D1").Value = "Usuario";

               //Headers Format
               var rngTable = worksheet.Range("A1:D1");
               rngTable.Style
               .Font.SetBold()
               .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);

               descuentos.ForEach(item =>
               {
                  worksheet.Cell("A" + rowStart).Value = item.fecha.ToShortTimeString();
                  worksheet.Cell("B" + rowStart).Value = item.apoderado;
                  worksheet.Cell("C" + rowStart).Value = item.celular;
                  worksheet.Cell("D" + rowStart).Value = item.usuario;

                  rowStart++;
               });
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",

                // By setting a file download name the framework will
                // automatically add the attachment Content-Disposition header
                fileDownloadName: "Cancelados.xlsx"
            );
         }
      }

      List<CanceladoInfo> getCanceladosByDiaDB(long evento, int anio, int mes, int dia)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", evento);
            var _anio = new SqlParameter("@anio", anio);
            var _mes = new SqlParameter("@mes", mes);
            var _dia = new SqlParameter("@dia", dia);
            return _context.Cancelados.FromSql("getCanceladosByDia @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes, _dia).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("promocionesempresa/")]
      public IActionResult getPromocionesEmpresa()
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.promociones = getPromocionesEmpresaDB();
         return Ok(response);
      }

      List<PromocionEmpresaRow> getPromocionesEmpresaDB()
      {
         try
         {
            var _tipo = new SqlParameter("@tipo", 1);
            return _context.PromocionesEmpresaRows.FromSql("getPromocionesEmpresa @tipo", _tipo).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpGet("promocionesempresaevento/{eventoid}")]
      public IActionResult getPromocionesEmpresaPorEvento(long eventoid)
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.promociones = getPromocionesEmpresaPorEventoDB(eventoid);
         return Ok(response);
      }

      List<PromocionEmpresaRow> getPromocionesEmpresaPorEventoDB(long eventoId)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            return _context.PromocionesEmpresaRows.FromSql("getPromocionesEmpresaPorEvento @eventoId", _eventoId).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpPost("gestionarpromocionesempresaevento/{eventoid}/{promocionesempresa}")]
      public IActionResult gestionarPromocionesEmpresaEvento(long eventoid, string promocionesempresa)
      {
         try
         {
            string[] promociones = promocionesempresa.Split(",");
            gestionarPromocionEmpresaEventoDB(eventoid, 0, 0);
            for (int i = 0; i < promociones.Length; i++)
            {
               gestionarPromocionEmpresaEventoDB(eventoid, Convert.ToInt64(promociones[i]), 1);
            }
            dynamic response = new System.Dynamic.ExpandoObject();
            response.promociones = getPromocionesEmpresaPorEventoDB(eventoid);
            return Ok(new { code = 1000, message = "Promocion Eliminada" });
         }
         catch (Exception ex)
         {

            return Ok(new { code = 3000, message = "No se pudo guardar las promociones " + ex.Message });
         }
      }

      void gestionarPromocionEmpresaEventoDB(long eventoId, long promocionEmpreaId, int tipo)
      {
         var _eventoId = new SqlParameter("@eventoId", eventoId);
         var _promocionEmpreaId = new SqlParameter("@promocionEmpreaId", promocionEmpreaId);
         var _tipo = new SqlParameter("@tipo", tipo);

         var sql = "EXEC dbo.gestionarPromocionEmpresaEvento @eventoId,@promocionEmpreaId,@tipo";
         _context.Database.ExecuteSqlCommand(sql, _eventoId, _promocionEmpreaId, _tipo);
      }

      [HttpPost("reprogramarentrada")]
      public IActionResult reprogramarEntrada(ReprogramarEntradaRequest request)
      {
         try
         {
            reprogramarEntradaDB(request.entradaId, request.fecha, request.horarioId);            
            return Ok(new { code = 1000, message = "La entrada ha sido reprogramada" });
         }
         catch (Exception ex)
         {
            return Ok(new { code = 2000, message = "No se pudo reprogramar la entrada " + ex.Message });
         }
      }

      void reprogramarEntradaDB(long id, DateTime fecha, int horarioId)
      {
         var _id = new SqlParameter("@id", id);
         var _fecha = new SqlParameter("@fecha", fecha);
         var _horarioId = new SqlParameter("@horarioId", horarioId);

         var sql = "EXEC dbo.reprogramarEntrada @id,@fecha,@horarioId";
         _context.Database.ExecuteSqlCommand(sql, _id, _fecha, _horarioId);
      }

      [HttpPost("eliminarevento/{eventoid}")]
      public IActionResult eliminarEvento(long eventoId)
      {
         try
         {
            OperacionResultado result = eliminarEventoDB(eventoId);
            return Ok(new { resultado = result });
         }
         catch (Exception ex)
         {
            return Ok(new { code = 2000, message = "No se pudo eliminar la promocion " + ex.Message });
         }
      }
      OperacionResultado eliminarEventoDB(long eventoId)
      {
         try
         {
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            return _context.OperacionResultados.FromSql("deleteEvento @eventoId", _eventoId).ToList().FirstOrDefault();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpPost("eliminarticketpromocion/{promocionid}")]
      public IActionResult eliminarPromocion(long promocionid)
      {
         try
         {
            OperacionResultado result = eliminarTicketPromocionDB(promocionid);
            return Ok(new { resultado = result });
         }
         catch (Exception ex)
         {
            return Ok(new { code = 2000, message = "No se pudo eliminar la promocion " + ex.Message });
         }
      }
      OperacionResultado eliminarTicketPromocionDB(long promocionId)
      {
         try
         {
            var _promocionId = new SqlParameter("@promocionId", promocionId);
            return _context.OperacionResultados.FromSql("deletePromocion @promocionId", _promocionId).ToList().FirstOrDefault();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [HttpPost("eliminarentrada/{id}")]
      public IActionResult eliminarTicketDefinicion(int id)
      {
         try
         {
            OperacionResultado result = eliminarTicketDefinicionDB(id);
            return Ok(new { resultado = result });
         }
         catch (Exception ex)
         {
            return Ok(new { code = 2000, message = "No se pudo eliminar la Entrada " + ex.Message });
         }
      }
      OperacionResultado eliminarTicketDefinicionDB(int id)
      {
         try
         {
            var _id = new SqlParameter("@id", id);
            return _context.OperacionResultados.FromSql("deleteEntrada @id", _id).ToList().FirstOrDefault();
         }
         catch (Exception e)
         {
            return null;
         }
      }
   }
}