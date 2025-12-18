using Armadar.Helpers;
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
    public partial class AdminController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody]User user)
        {
            if (user != null)
            {
                if (hasEmailAndPassword(user))
                {
                    if (!emailDoesNotExist(user.email))
                    {                        
                        User oUser = loginv0(user.email);
                        UserRole role = _context.UserRoles.SingleOrDefault(x => x.userId == oUser.id);

                        if (role != null)
                        {
                            if (role.roleId != 1) // No Admin Mode
                            {
                                Evento evento = null;

                                Evento eventoZona = _context.Eventos.SingleOrDefault(x => x.operadorZona == oUser.id);
                                if (eventoZona != null)
                                {
                                    evento = eventoZona;
                                    if (evento.password != user.password)
                                    {
                                        return Ok(new { code = 3000, message = "Password Invalido" });
                                    }
                                }
                                else
                                {
                                    Evento eventoJuego = _context.Eventos.SingleOrDefault(x => x.operadorJuego == oUser.id);
                                    if (eventoJuego != null)
                                    {
                                        evento = eventoJuego;
                                        if (evento.password2 != user.password)
                                        {
                                            return Ok(new { code = 3000, message = "Password Invalido" });
                                        }
                                    }
                                }

                                if (evento != null)
                                {
                                    List<String> myRol = myFlatRoles(oUser.id);// Solo uno

                                    Configuracion configuracion = getConfiguracion();
                                    Caja caja = _context.Cajas.SingleOrDefault(c => c.eventoId == evento.id &&
                                    c.fecha.Year == configuracion.horaActual.Year &&
                                    c.fecha.Month == configuracion.horaActual.Month &&
                                    c.fecha.Day == configuracion.horaActual.Day);

                                    EventoInfo oEvento = new EventoInfo()
                                    {
                                        id = evento.id,
                                        nombre = evento.nombre,
                                        lugar = evento.lugar,
                                        juego = evento.juego,
                                        aforo = evento.aforo,
                                        cajaIsOpen = caja == null,
                                        fecha = configuracion.horaActual,
                                        tarifaMinutos = evento.tarifaMinutos,
                                        tarifaPrecioMinuto = evento.tarifaPreciominuto,
                                        tarifaPrecioMinutoAdicional = evento.tarifaPreciominutoadicional,
                                        operadorEmail = user.email,
                                        operadorRol = myRol[0]
                                    };
                                    string token = JWT.CreateToken(_appSettings, oUser.id.ToString(), myRol, evento.id);
                                    return Ok(new { code = 1000, message = "usuario autenticado", evento = oEvento, token });
                                }
                                else
                                {
                                    return Ok(new { code = 2500, message = "El Operador no tiene Evento asociado" });
                                }
                            } else { // Admin Mode
                                if (oUser.password != user.password)
                                {
                                    return Ok(new { code = 3000, message = "Password Invalido" });
                                }

                                List<String> myRol = myFlatRoles(oUser.id);// Solo uno
                                string token = JWT.CreateToken(_appSettings, oUser.id.ToString(), myRol, 1000);
                                
                                dynamic info = new System.Dynamic.ExpandoObject();
                                info.mode = "admin";
                                return Ok(new { code = 1000, message = "usuario autenticado", info, token });
                            }                            
                        }
                        else
                        {
                            return Ok(new { code = 2000, message = "usuario sin rol asignado"});
                        }                                                    
                    }
                    else
                    {
                        return BadRequest(new { code = 3001, message = "El usuario no existe" });
                    }
                }
                else
                {
                    return BadRequest(new { code = 4001, message = "informacion no valida" });
                }
            }
            else
            {
                return BadRequest(new { code = 6001, message = "no se encontro informacion" });
            }
        }

        bool hasEmailAndPassword(User user)
        {
            return user.email != null && user.email.Length > 0 && user.password != null && user.password.Length > 0;
        }

        bool emailDoesNotExist(string email)
        {
                var oUser = _context.Users.SingleOrDefault(x => x.email == email);
            return oUser == null;
        }

        User loginv0(string user)
        {
            User oUser = _context.Users.SingleOrDefault(x => x.email == user);
            return oUser;
        }

        List<String> myFlatRoles(long userId)
        {
            List<String> myFlatRolesReturn = new List<string>();
            List<Role> myRoles = getUserRoles(userId);
            if (myRoles != null)
            {
                myRoles.ForEach(role => {
                    myFlatRolesReturn.Add(role.name);
                });
            }
            return myFlatRolesReturn;
        }

        List<Role> getUserRoles(long userId)
        {
            try
            {
                var _userId = new SqlParameter("@parameter", userId);
                return _context.Roles.FromSql("getRolesByUser @parameter", _userId).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        Configuracion getConfiguracion()
        {
            try
            {
                return _context.Configuraciones.FromSql("getConfiguracion").ToList()[0];
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}