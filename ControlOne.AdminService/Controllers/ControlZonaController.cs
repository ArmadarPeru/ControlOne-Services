using ControlOne.AdminService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Controllers
{
    public partial class AdminController: ControllerBase
    {
        [HttpGet("getcontrolsalida/{evento}")]
        public async Task<IActionResult> GetControlSalida(long evento)
        {
            try
            {
                return Ok(getControlSalidaDB(GetSafeEvento()));
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener la informacion de salida" });
            }
        }

        List<UsuarioControlSalida> getControlSalidaDB(long eventoId)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", eventoId);
                return _context.UsuariosControlSalida.FromSql("getControlSalida @eventoId", _eventoId).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }
        UsuarioControlSalida getControlSalidaRecordDB(long usuarioAccionId, long eventoId)
        {
            try
            {
                var _usuarioAccionId = new SqlParameter("@usuarioAccionId", usuarioAccionId);
                var _eventoId = new SqlParameter("@eventoId", eventoId);
                return _context.UsuariosControlSalida.FromSql("getControlSalidaRecord @usuarioAccionId, @eventoId", _usuarioAccionId, _eventoId).First();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("getusuarioseventozona/{dni}/{evento}")]
        public async Task<IActionResult> getusuarioseventozona(string dni, long evento)
        {
            try
            {
                var apoderado = _context.Apoderados.SingleOrDefault(x => x.dni == dni);
                if (apoderado == null) return Ok(new { code = 2000, message = "Apoderado no registrado" });

                dynamic response = new System.Dynamic.ExpandoObject();
                response.apoderado = apoderado;

                List<UsuarioZona> nihos = new List<UsuarioZona>();
                nihos = getUsuariosByApoderadoEventoZona(apoderado.id, GetSafeEvento());
                response.nihos = nihos;

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo buscar el Apoderado" });
            }
        }

        List<UsuarioZona> getUsuariosByApoderadoEventoZona(long apoderadoId, long eventoId)
        {
            try
            {
                var _apoderadoId = new SqlParameter("@apoderadoId", apoderadoId);
                var _eventoId = new SqlParameter("@eventoId", eventoId);
                return _context.UsuariosZona.FromSql("getusuariosByApoderadoEventoZona @apoderadoId,@eventoId", _apoderadoId, _eventoId).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("ingresarusuarioeventozona/{nihoid}/{conacompaniante}/{isconadis}/{pagoadelantado}/{eventoid}")]
        public async Task<IActionResult> ingresarNihoEventoZona(long nihoid, int conacompaniante, int isconadis, int pagoadelantado, long eventoid)
        {
            try
            {
                setAccionUsuarioJuegoDB(0, nihoid, conacompaniante, isconadis, pagoadelantado, GetSafeEvento(), 1000, 0);
                return Ok(new { code = 1000, message = "Niho ingresado correctamente" });
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo ingresar el niho al Evento" });
            }
        }
        
        [HttpPost("cobrar")]
        public async Task<IActionResult> insertCobro(Cobro cobro)
        {
            try
            {
                UsuarioControlSalida controlSalidaInfo = getControlSalidaRecordDB(cobro.nihoaccion, GetSafeEvento());

                Cobro newCobro = new Cobro()
                {
                    eventoId = GetSafeEvento(),
                    nihoaccion = cobro.nihoaccion,
                    minutos = controlSalidaInfo.tiempo,
                    monto = controlSalidaInfo.monto,
                    montoefectivo = cobro.montoefectivo,
                    montotarjeta = cobro.montotarjeta,
                    dscto = cobro.dscto,
                    dsctomotivo = cobro.dsctomotivo,
                    pagoMultiple = cobro.pagoMultiple,
                    promocionId = cobro.promocionId
                };

                if (cobro.pagoMultiple == 0)
                {
                    if ((controlSalidaInfo.monto - newCobro.dscto) != (newCobro.montoefectivo + newCobro.montotarjeta))
                        return Ok(new { code = 2000, message = "Los montos ingresados no son validos" });

                    insertCobroDB(newCobro);
                    return Ok(new { code = 1000, message = "Cobro Guardado" });
                }
                else
                {
                    string[] usuariosRow = cobro.pagosMultiples.Split(@",");
                    decimal total = 0;
                    foreach (string usuarioRow in usuariosRow)
                    {
                        UsuarioControlSalida salidaInfo = getControlSalidaRecordDB(Convert.ToInt64(usuarioRow), GetSafeEvento());
                        total += salidaInfo.monto;
                    }

                    if ((total - newCobro.dscto) != (newCobro.montoefectivo + newCobro.montotarjeta))
                        return Ok(new { code = 2000, message = "Los montos ingresados no son validos" });

                    foreach (string usuarioRow in usuariosRow)
                    {
                        if (Convert.ToInt64(usuarioRow) == cobro.nihoaccion) // Header or Main Pago
                        {
                            newCobro.monto = total;
                            insertCobroDB(newCobro);
                        }
                        else
                        {
                            long usuarioAccionId = Convert.ToInt64(usuarioRow);
                            UsuarioControlSalida salidaInfo = getControlSalidaRecordDB(usuarioAccionId, GetSafeEvento());
                            Cobro cobroRelacionado = new Cobro()
                            {
                                eventoId = GetSafeEvento(),
                                nihoaccion = usuarioAccionId,
                                minutos = salidaInfo.tiempo,
                                monto = 0,
                                montoefectivo = 0,
                                montotarjeta = 0,
                                dscto = 0,
                                dsctomotivo = cobro.dsctomotivo,
                                pagoMultiple = cobro.nihoaccion,
                                promocionId = cobro.promocionId
                            };
                            insertCobroDB(cobroRelacionado);
                        }
                    }

                    return Ok(new { code = 1000, message = "Cobro Guardado" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { code = 3000, message = "No se pudo Cobrar" });
            }
        }

        void insertCobroDB(Cobro cobro)
        {
            var eventoId = new SqlParameter("@eventoId", cobro.eventoId);
            var nihoaccion = new SqlParameter("@nihoaccion", cobro.nihoaccion);
            var minutos = new SqlParameter("@minutos", cobro.minutos);
            var monto = new SqlParameter("@monto", cobro.monto);
            var montoefectivo = new SqlParameter("@montoefectivo", cobro.montoefectivo);
            var montotarjeta = new SqlParameter("@montotarjeta", cobro.montotarjeta);
            var dscto = new SqlParameter("@dscto", cobro.dscto);
            var dsctomotivo = new SqlParameter("@dsctomotivo", cobro.dsctomotivo);
            var pagoMultiple = new SqlParameter("@pagoMultiple", cobro.pagoMultiple);
            var promocionId = new SqlParameter("@promocionId", cobro.promocionId);

            var sql = "EXEC dbo.insertCobro @eventoId, @nihoaccion, @minutos, @monto,@montoefectivo,@montotarjeta, @dscto, @dsctomotivo, @pagoMultiple, @promocionId";
            _context.Database.ExecuteSqlCommand(sql, eventoId, nihoaccion, minutos, monto, montoefectivo, montotarjeta, dscto, dsctomotivo, pagoMultiple, promocionId);
        }

        [HttpPost("registrargasto")]
        public async Task<IActionResult> insertGasto(Gasto gasto)
        {
            try
            {
                gasto.eventoId = GetSafeEvento();
                insertGastoDB(gasto);
                return Ok(new { code = 1000, message = "Gasto Guardado" });
            }
            catch (Exception e)
            {
                return BadRequest(new { code = 3000, message = "No se pudo registrar el Gasto" });
            }
        }

        void insertGastoDB(Gasto gasto)
        {
            var eventoId = new SqlParameter("@eventoId", gasto.eventoId);
            var importe = new SqlParameter("@importe", gasto.importe);
            var tipoGasto = new SqlParameter("@tipoGasto", gasto.tipoGasto);
            var tipoAbono = new SqlParameter("@tipoAbono", gasto.tipoAbono);
            var comprobante = new SqlParameter("@comprobante", gasto.comprobante);
            var nroComprobante = new SqlParameter("@nroComprobante", gasto.nroComprobante);
            var proveedor = new SqlParameter("@proveedor", gasto.proveedor);
            var gastoDetalle = new SqlParameter("@gastoDetalle", gasto.gastoDetalle);

            var sql = "EXEC dbo.insertGasto @eventoId, @importe, @tipoGasto,@tipoAbono,@comprobante, @nroComprobante, @proveedor, @gastoDetalle";
            _context.Database.ExecuteSqlCommand(sql, eventoId, importe, tipoGasto, tipoAbono, comprobante, nroComprobante, proveedor, gastoDetalle);
        }

        [HttpPost("cerrarcaja")]
        public async Task<IActionResult> cerrarCaja(CajaRequest caja)
        {
            try
            {
                Caja oCaja = _context.Cajas.SingleOrDefault(c => c.eventoId == caja.eventoId && c.fecha.Year == caja.anio && c.fecha.Month == caja.mes && c.fecha.Day == caja.dia);
                if (oCaja == null)
                {
                    Caja calculatedCaja = getCajaInfoDB(GetSafeEvento(), caja.anio, caja.mes, caja.dia)[0];
                    Caja newCaja = new Caja();

                    newCaja.eventoId = GetSafeEvento();
                    newCaja.fecha = calculatedCaja.fecha;
                    newCaja.efectivoInicial = calculatedCaja.efectivoInicial;

                    newCaja.ventaOtros = calculatedCaja.ventaOtros;
                    newCaja.ventaPOS = caja.ventaPOS;
                    newCaja.ventaEfectivo = calculatedCaja.ventaEfectivo - caja.ventaPOS; // Efectivo asincerado en base al POS Final                    
                    newCaja.ventaTotal = calculatedCaja.ventaTotal; // No, Cambia, solo cambia la distribucion del Efectivo a partir de la Venta POS

                    newCaja.gastos = calculatedCaja.gastos;
                    newCaja.efectivoCaja = calculatedCaja.efectivoInicial + newCaja.ventaEfectivo - calculatedCaja.gastos;

                    newCaja.abonosEntregables = calculatedCaja.abonosEntregables;
                    newCaja.abonos = calculatedCaja.abonos;
                    newCaja.retiros = calculatedCaja.retiros;
                    newCaja.efectivoFinalCaja = newCaja.efectivoCaja - calculatedCaja.abonosEntregables;

                    
                    newCaja.faltanteVenta = caja.faltanteVenta;
                    newCaja.sobranteVenta = caja.sobranteVenta;
                    newCaja.cierreCaja = caja.ventaPOS > 0 ? newCaja.efectivoFinalCaja - caja.faltanteVenta + caja.sobranteVenta : calculatedCaja.cierreCaja - caja.faltanteVenta + caja.sobranteVenta;

                    _context.Cajas.Add(newCaja);
                    await _context.SaveChangesAsync();

                    Evento evento = _context.Eventos.SingleOrDefault(x => x.id == caja.eventoId);

                    List<Concepto> metaGastos = getGastosAbonosDB(GetSafeEvento(), caja.anio, caja.mes, caja.dia);

                    List <Concepto> gastos = metaGastos.Where(g => g.tipo == "Gasto").ToList();
                    List <Concepto> abonos = metaGastos.Where(g => g.tipo == "Abono").ToList();

                    EmailHelper.EmailHelper.Send("antoniovargas@xtremeplaza.com.pe", "Cierre de Caja - " + evento.lugar,
                        newCaja.efectivoInicial.ToString(), newCaja.ventaEfectivo.ToString(), newCaja.ventaOtros.ToString(), newCaja.ventaPOS.ToString(), newCaja.ventaTotal.ToString(), gastos, newCaja.gastos.ToString(), newCaja.efectivoCaja.ToString(), abonos, newCaja.abonosEntregables.ToString(), newCaja.efectivoFinalCaja.ToString(), newCaja.faltanteVenta.ToString(), newCaja.sobranteVenta.ToString(), newCaja.cierreCaja.ToString());

                    return Ok(new { code = 1000, message = "Caja Cerrada" });
                }
                else
                {
                    return Ok(new { code = 2000, message = "La caja ya ha sido Cerrada" });
                }                
            }
            catch (Exception e)
            {
                return BadRequest(new { code = 3000, message = "No se pudo cerrar Caja" });
            }
        }

        [HttpPost("actualizarusuarios")]
        public async Task<IActionResult> actualizarUsuarios(UpdateNihos info)
        {
            try
            {
                if (getUsuariosEnUso(info.apoderadoDni).Count == 0)
                {
                    if (!string.IsNullOrEmpty(info.toRemove))
                    {
                        string[] idsToRemove = info.toRemove.Split(",");
                        foreach (string idToRemove in idsToRemove)
                        {
                            deleteUser(Convert.ToInt64(idToRemove));
                        }
                    }

                    if (!string.IsNullOrEmpty(info.toUpdate))
                    {
                        string[] usersToUpdate = info.toUpdate.Split(",");
                        foreach (string usuario in usersToUpdate)
                        {
                            string[] userInfo = usuario.Split("|");
                            long userId = Convert.ToInt64(userInfo[0]);
                            string userName = userInfo[1];
                            DateTime userFechaNacimiento = Convert.ToDateTime(userInfo[2]);
                            updateUser(userId, userName, userFechaNacimiento);
                        }
                    }

                    return Ok(new { code = 1000, message = "OK" });
                }
                else
                {
                    return Ok(new { code = 2000, message = "No se puede actualizar con usuarios dentro del juego" });
                }                
            }
            catch (Exception e)
            {
                return Ok(new { code = 5000, message = "Error " + e.Message });
            }
        }

        internal List<Usuario> getUsuariosEnUso(string dni)
        {
            try
            {
                var _dni = new SqlParameter("@dni", dni);
                return _context.Usuarios.FromSql("usuariosEnUso @dni", _dni).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        internal bool deleteUser(long userId)
        {
            try
            {
                var _usuarioId = new SqlParameter("@usuarioId", userId);

                var sql = "EXEC dbo.deleteUsuario @usuarioId";
                _context.Database.ExecuteSqlCommand(sql, _usuarioId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
        internal bool updateUser(long userId, string nombres, DateTime fechaNacimiento)
        {
            try
            {
                var _usuarioId = new SqlParameter("@usuarioId", userId);
                var _nombres = new SqlParameter("@nombres", nombres);
                var _fechaNacimiento = new SqlParameter("@fechaNacimiento", fechaNacimiento);

                var sql = "EXEC dbo.updateUsuario @usuarioId,@nombres,@fechaNacimiento";
                _context.Database.ExecuteSqlCommand(sql, _usuarioId, _nombres, _fechaNacimiento);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        [HttpGet("getcajainfo/{evento}/{anio}/{mes}/{dia}")]
        public async Task<IActionResult> getCajaInfo(long evento, int anio, int mes, int dia)
        {
            try
            {
                dynamic response = new System.Dynamic.ExpandoObject();

                List<Caja> caja = new List<Caja>();
                caja = getCajaInfoDB(GetSafeEvento(), anio, mes, dia);
                response.info = caja[0];

                List<Concepto> metaGastos = getGastosAbonosDB(GetSafeEvento(), anio, mes, dia);

                response.gastos = metaGastos.Where(g => g.tipo == "Gasto");
                response.abonos = metaGastos.Where(g => g.tipo == "Abono");

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener la informacion Pre Caja" });
            }
        }

        List<Caja> getCajaInfoDB(long evento, int anio, int mes, int dia)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", evento);
                var _anio = new SqlParameter("@anio", anio);
                var _mes = new SqlParameter("@mes", mes);                
                var _dia = new SqlParameter("@dia", dia);
                return _context.Cajas.FromSql("getCajaInfo @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes,_dia).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        List<Concepto> getGastosAbonosDB(long evento, int anio, int mes, int dia)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", evento);
                var _anio = new SqlParameter("@anio", anio);
                var _mes = new SqlParameter("@mes", mes);
                var _dia = new SqlParameter("@dia", dia);
                return _context.Conceptos.FromSql("getGastosAbonos @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes, _dia).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        List<Concepto> getGastosDB(long evento, int anio, int mes, int dia)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", evento);
                var _anio = new SqlParameter("@anio", anio);
                var _mes = new SqlParameter("@mes", mes);
                var _dia = new SqlParameter("@dia", dia);
                return _context.Conceptos.FromSql("getGastos @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes, _dia).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("buscarapoderado/{criterio}")]
        public async Task<IActionResult> BuscarApoderado(string criterio)
        {
            try
            {
                dynamic response = new System.Dynamic.ExpandoObject();

                List<Apoderado> resultado = new List<Apoderado>();
                resultado = buscarapoderadoDB(criterio);
                response.apoderados = resultado;

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener la informacion Pre Caja" });
            }
        }

        List<Apoderado> buscarapoderadoDB(string criterio)
        {
            try
            {
                var _criterio = new SqlParameter("@criterio", criterio);
              
                return _context.Apoderados.FromSql("buscarapoderado @criterio", _criterio).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("buscarapoderadousuarios/{criterio}")]
        public async Task<IActionResult> BuscarApoderadoUsuarios(string criterio)
        {
            try
            {
                dynamic response = new System.Dynamic.ExpandoObject();

                List<ApoderadoUsuario> resultado = new List<ApoderadoUsuario>();
                resultado = buscArapoderadoUsuarioDB(criterio);
                response.apoderados = resultado;

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener la informacion Pre Caja" });
            }
        }
        List<ApoderadoUsuario> buscArapoderadoUsuarioDB(string criterio)
        {
            try
            {
                var _criterio = new SqlParameter("@criterio", criterio);

                return _context.ApoderadosUsuarios.FromSql("buscarapoderadoyusuarios @criterio", _criterio).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("getpromociones/{evento}")]
        public async Task<IActionResult> getPromociones(long evento)
        {
            try
            {
                dynamic response = new System.Dynamic.ExpandoObject();

                List<Promocion> resultado = new List<Promocion>();
                resultado = getPromocionesDB(evento);
                response.promociones = resultado;

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener las promociones del Evento " + evento.ToString() + " mensaje: " + e.Message });
            }
        }
        List<Promocion> getPromocionesDB(long evento)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", evento);
                return _context.Promociones.FromSql("getPromociones @eventoId", _eventoId).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        List<Historial> getHistorialDB(long evento, int anio, int mes, int dia)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", evento);
                var _anio = new SqlParameter("@anio", anio);
                var _mes = new SqlParameter("@mes", mes);
                var _dia = new SqlParameter("@dia", dia);
                return _context.Historiales.FromSql("getHistorial @eventoId,@anio,@mes,@dia", _eventoId, _anio, _mes, _dia).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("historial/{evento}/{anio}/{mes}/{dia}")]
        public async Task<IActionResult> getHistorialInfo(long evento, int anio, int mes, int dia)
        {
            try
            {
                dynamic response = new System.Dynamic.ExpandoObject();
                List<Historial> historial = getHistorialDB(GetSafeEvento(), anio, mes, dia);

                response.historial = historial;

                return Ok(response);
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener la informacion Pre Caja" });
            }
        }
    }
}