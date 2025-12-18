using ControlOne.AdminService.Models;
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
        [HttpGet("getcontroljuego/{evento}")]
        public async Task<IActionResult> GetControlJuego(long evento)
        {
            try
            {
                return Ok(getControlJuegoDB(GetSafeEvento()));
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo obtener la informacion del Juego" });
            }
        }

        List<UsuarioControlJuego> getControlJuegoDB(long eventoId)
        {
            try
            {
                var _eventoId = new SqlParameter("@eventoId", eventoId);
                var _juegoId = new SqlParameter("@juegoId", 1000);
                return _context.UsuariosControlJuego.FromSql("getControlJuego @eventoId, @juegoId", _eventoId, _juegoId).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet("iniciarnihojuego/{nihojuegoid}")]
        public async Task<IActionResult> IniciarNihoJuego(long nihojuegoid)
        {
            try
            {
                setAccionUsuarioJuegoDB(nihojuegoid, 0, 0, 0, 0, 0, 0, 1);
                return Ok(new { code = 1000, message = "Juego Iniciado correctamente" });
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo Iniciar el juego" });
            }
        }

        [HttpGet("pausarnihojuego/{nihojuegoid}")]
        public async Task<IActionResult> PausarNihoJuego(long nihojuegoid)
        {
            try
            {
                setAccionUsuarioJuegoDB(nihojuegoid, 0, 0,0, 0,0, 0, 2);
                UsuarioAccion nihoAccion = _context.UsuarioAccion.SingleOrDefault(na => na.id == nihojuegoid);
                return Ok(new { code = 1000, nihoAccion.minutos, nihoAccion.segundos, message = "Juego Pausado correctamente" });
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo Pusar el juego" });
            }
        }

        [HttpGet("finalizarnihojuego/{nihojuegoid}")]
        public async Task<IActionResult> FinalizarNihoJuego(long nihojuegoid)
        {
            try
            {
                setAccionUsuarioJuegoDB(nihojuegoid, 0, 0, 0, 0, 0, 0, 3);
                return Ok(new { code = 1000, message = "Finalizar Juego correctamente" });
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo Finalizar el juego" });
            }
        }

        [HttpGet("finalizarvariosjuegos/{juegos}")]
        public async Task<IActionResult> FinalizarVariosJuegos(string juegos)
        {
            try
            {
                string[] juegosIds = juegos.Split(",");
                foreach (string juegoId in juegosIds)
                {
                    setAccionUsuarioJuegoDB(Convert.ToInt64(juegoId), 0, 0, 0, 0, 0, 0, 3);
                }
                
                return Ok(new { code = 1000, message = "Juegos Finalizados correctamente " + juegos });
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo Finalizar los juegos " + juegos });
            }
        }


        [HttpGet("cancelarnihojuego/{nihojuegoid}")]
        public async Task<IActionResult> CancelarNihoJuego(long nihojuegoid)
        {
            try
            {
                setAccionUsuarioJuegoDB(nihojuegoid, 0, 0, 0, 0, 0, 0, 5);
                return Ok(new { code = 1000, message = "Cancelar Juego correctamente" });
            }
            catch (Exception e)
            {
                return Ok(new { code = 2000, message = "No se pudo Cancelar el juego" });
            }
        }

        void setAccionUsuarioJuegoDB(long usuarioAccionId, long nihoId, int conAcompaniante,int isConadis, int pagoAdelantado, long eventoId, long juegoId, int accionId)
        {
            var _usuarioAccionId = new SqlParameter("@usuarioAccionId", usuarioAccionId);
            var _usuarioId = new SqlParameter("@usuarioId", nihoId);
            var _conAcompaniante = new SqlParameter("@conAcompaniante", conAcompaniante);
            var _isConadis = new SqlParameter("@isConadis", isConadis);
            var _pagoAdelantado = new SqlParameter("@pagoAdelantado", pagoAdelantado);
            var _eventoId = new SqlParameter("@eventoId", eventoId);
            var _juegoId = new SqlParameter("@juegoId", juegoId);
            var _accionId = new SqlParameter("@accionId", accionId);

            var sql = "EXEC dbo.setAccionUsuarioJuego @usuarioAccionId,@usuarioId,@conAcompaniante,@isConadis,@pagoAdelantado,@eventoId,@juegoId,@accionId";
            _context.Database.ExecuteSqlCommand(sql, _usuarioAccionId, _usuarioId, _conAcompaniante, _isConadis, _pagoAdelantado, _eventoId, _juegoId, _accionId);
        }
    }
}