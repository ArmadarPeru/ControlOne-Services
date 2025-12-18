using ControlOne.AdminService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ControlOne.AdminService.Controllers
{
   public partial class AdminController : ControllerBase
   {
      [AllowAnonymous]
      [HttpGet("horaactual/")]
      public async Task<IActionResult> getHoraActual(string dni)
      {
         try
         {
            dynamic response = new System.Dynamic.ExpandoObject();
            response.horaActual = getConfiguracion().horaActual;
            return Ok(new { code = 1000, response.horaActual });
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo obtener la hora actual" });
         }
      }

      [AllowAnonymous]
      [HttpGet("getapoderado/{dni}")]
      public async Task<IActionResult> getApoderado(string dni)
      {
         try
         {
            var apoderado = _context.Apoderados.SingleOrDefault(x => x.dni == dni);
            if (apoderado == null) return Ok(new { code = 2000, message = "Apoderado no registrado" });

            dynamic response = getApoderadoInfoDB(apoderado);
            return Ok(response);
         }
         catch (Exception e)
         {
            return Ok(new { code = 2000, message = "No se pudo buscar el Apoderado" });
         }
      }

      dynamic getApoderadoInfoDB(Apoderado apoderado)
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.apoderado = apoderado;
         response.horaActual = getConfiguracion().horaActual;

         List<MiUsuario> misUsuarios = getMisUsuariosDB(apoderado.id);
         response.nihos = misUsuarios;

         bool isUsuariosEnEvento = misUsuarios.FindAll(u => u.accionId < 4 && u.fechaInicio.Year != 2001).Count() > 0;
         int tarifaMinutos = 0;
         decimal tarifaPrecioMinuto = 0;
         decimal tarifaPrecioMinutoAdicional = 0;

         Boolean isPromocion = false;
         Promocion promocion = new Promocion();

         if (isUsuariosEnEvento)
         {
            long eventoId = misUsuarios.FindAll(u => u.accionId < 4 && u.fechaInicio.Year != 2001).First().eventoId;
            Evento evento = _context.Eventos.SingleOrDefault(x => x.id == eventoId);
            if (evento != null)
            {
               tarifaMinutos = evento.tarifaMinutos;
               tarifaPrecioMinuto = evento.tarifaPreciominuto;
               tarifaPrecioMinutoAdicional = evento.tarifaPreciominutoadicional;

               promocion = getPromocionesDB(eventoId).Where(x => x.activo == 1).FirstOrDefault();
               // check Time of the Promocion
               if (promocion != null)
               {
                  TimeSpan inicio = promocion.inicio.TimeOfDay;
                  TimeSpan final = promocion.final.TimeOfDay;
                  TimeSpan ahora = promocion.horaactual.TimeOfDay;
                  if (ahora >= inicio && ahora <= final)
                  {
                     isPromocion = true;
                  }
               }
            }
         }

         response.isUsuariosEnEvento = isUsuariosEnEvento;
         response.tarifaMinutos = tarifaMinutos;
         response.tarifaPrecioMinuto = tarifaPrecioMinuto;
         response.tarifaPrecioMinutoAdicional = tarifaPrecioMinutoAdicional;

         response.isPromocion = isPromocion;
         if (isPromocion)
         {
            response.tarifaMinutos = promocion.minutos;
            response.tarifaPrecioMinuto = promocion.preciominuto;
            response.tarifaPrecioMinutoAdicional = promocion.preciominutoadicional;
         }

         return response;
      }

      List<MiUsuario> getMisUsuariosDB(long apoderadoId)
      {
         try
         {
            var _apoderadoId = new SqlParameter("@parameter", apoderadoId);
            return _context.MisUsuarios.FromSql("getMisUsuarios @parameter", _apoderadoId).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      List<Usuario> getUsuariosByApoderado(long apoderadoId)
      {
         try
         {
            var _apoderadoId = new SqlParameter("@parameter", apoderadoId);
            return _context.Usuarios.FromSql("getusuariosByApoderado @parameter", _apoderadoId).ToList();
         }
         catch (Exception e)
         {
            return null;
         }
      }

      [AllowAnonymous]
      [HttpPost("insertapoderado")]
      public async Task<IActionResult> insertApoderado(Apoderado apoderado)
      {
         try
         {
            if (apoderado.id == 0)
            {
               Apoderado validateApoderado = _context.Apoderados.SingleOrDefault(x => x.dni == apoderado.dni);
               if (validateApoderado != null) return BadRequest(new { code = 2000, message = "El Apoderado con DNI " + apoderado.dni + " ya se encuentra registrado" });
            }            

            Apoderado newApoderado = new Apoderado()
            {
               nombres = apoderado.nombres,
               pais = apoderado.pais,
               dni = apoderado.dni,
               email = apoderado.email,
               celular = apoderado.celular,
               firma = apoderado.firma
            };

            long apoderadoId = apoderado.id == 0 ? insertApoderadoDB(newApoderado) : apoderado.id;
            if (apoderadoId != 0)
            {
               string[] usuariosArray = apoderado.usuarios.Split("|");

               foreach (string usuario in usuariosArray)
               {
                  string[] usuarioBody = usuario.Split(",");
                  Usuario oUsuario = new Usuario()
                  {
                     tipo = "N",
                     apoderadoId = apoderadoId,
                     nombres = usuarioBody[0],
                     fechaNacimiento = Convert.ToDateTime(usuarioBody[1]),
                     edad = DateTime.Today.Year - Convert.ToDateTime(usuarioBody[1]).Year
                  };
                  insertUsuarioDB(oUsuario);
               }

               if (apoderado.id == 0)
               {
                  Usuario apoderadoUsuario = new Usuario()
                  {
                     tipo = "A",
                     apoderadoId = apoderadoId,
                     nombres = apoderado.nombres,
                     fechaNacimiento = DateTime.Now,
                     edad = 0
                  };
                  insertUsuarioDB(apoderadoUsuario);
               }
               else
               {
                  updateApoderadoDB(apoderado, 1);
               }
            }
            else
            {
               return Ok(new { code = 3000, message = "No se pudo guardar el Apoderado" });
            }

            Apoderado apoderadoInMemory = newApoderado;
            apoderadoInMemory.id = apoderadoId;

            //string firma = "data:image/png; base64,iVBORw0KGgoAAAANSUhEUgAAAmwAAACgCAYAAACxIDDDAAAJiUlEQVR4nO3dS5IbSRkA4P8sHIQbcIA5AXdhC2sWPILXEhbsWXgBwYKJYYKZ4NEehogJYw829rTd3VazMBWdncqUSlJlVpX0fREV45GlygypXPnXn68IAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAmN5tRNxHxCY5AABYiE18CNaG427e6gAAkLqOx8Hai3mrAwBALg3WAAAuQtq9uHRDXY1XAwDOXhr4rCFj9SweTzAAADhLr + NxgLaJiPex / IxVWufbmevC6YbfEwBIDNmpUqA2 / P + T2WpXdxeP68j66dIGYJHyjFbPzMKnWbm3sR2oPe1Yn0Ok39v7mevCNNLf9GbmugDA1riwTfJar + AjrcO7eFhkdqjPVad6HCoNKO8j4st5q8ME0qyazBoAs / kitgfxb + JDoBTx0FD1CNZuoj5GbcmBWun7Y73 + FdsPLl / PWiMALlaeDcqDjHfJ3 / UYLJ93I6b1 + rxD + cfIG3W7FqzbH2P799SlDUB3N7E9Lq0UjPVc2ywNztYwRu2TKI / tM3PwePnv3ltpmRhZUgC6K41Ly6XBXK / xOqWuxKV2fabj6DTu08izvPcR8aZT2V9GfbwmAHSVN0Z5Y1gaq9OjwSo11EttKPO6vs9eeztf1VYrHavYu + uxdO0N19 + 3O9UBgAuXdy3VGsJSkPayQ / 1KY76WOkYoz6DdZK8vNcBcsnQSSe9AvXTtDeX / slMdALhgY4Og0iSDHpMJvl + o4yYiXnUo + xi1YOJNLD / IXKrStdcjUPtn1McbbiLi5x3qAMCFehvlAKgWROTv7RVs5GO + lpyV + nPszvqYWHCcUsb3PiKuG5f7h9i + 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";
            EmailHelper.EmailHelper.Send(apoderado.email, "Declaracion de Exoneracion de Responsabilidad", apoderado.nombres, apoderado.dni, "");

            return Ok(new { code = 1000, message = "Informacion Guardada", apoderadoinfo = getApoderadoInfoDB(apoderadoInMemory) });
         }
         catch (Exception e)
         {
            return BadRequest(new { code = 3000, message = "No se pudo guardar la informacion " + e.Message });
         }
      }

      [HttpPost("updateapoderado")]
      public async Task<IActionResult> updateApoderado(Apoderado apoderado)
      {
         try
         {
            apoderado.firma = string.Empty;
            updateApoderadoDB(apoderado, 0);

            return Ok(new { code = 1000, message = "Apoderado actualizado" });
         }
         catch (Exception e)
         {
            return BadRequest(new { code = 3000, message = "No se pudo actualizar el Apoderado" });
         }
      }

      [AllowAnonymous]
      [HttpPost("insertusuario")]
      public async Task<IActionResult> insertUsuario(Usuario usuario)
      {
         Apoderado validateApoderado = _context.Apoderados.SingleOrDefault(x => x.id == usuario.apoderadoId);
         if (validateApoderado == null) return BadRequest(new { code = 2000, message = "El Apoderado no existe" });

         try
         {
            usuario.tipo = "N";
            usuario.edad = DateTime.Today.Year - Convert.ToDateTime(usuario.fechaNacimiento).Year;
            insertUsuarioDB(usuario);
            return Ok(new { code = 1000, message = "Usuario Guardado" });
         }
         catch (Exception)
         {
            return Ok(new { code = 3000, message = "No se pudo guardar el Usuario" });
         }
      }      

      dynamic createBasicApoderado(Apoderado apoderado)
      {
         dynamic response = new System.Dynamic.ExpandoObject();
         response.apoderado = apoderado;
         return response;
      }

        [AllowAnonymous]
        [HttpPost("insertapoderadobasic")]
        public async Task<IActionResult> insertApoderado_Basic(Apoderado apoderado)
        {
            try
            {
                Apoderado validateApoderado = _context.Apoderados.SingleOrDefault(x => x.dni == apoderado.dni);
                if (validateApoderado != null) return Ok(new { code = 2000, message = "El Apoderado con DNI " + apoderado.dni + " ya se encuentra registrado" });

                List<Apoderado> apoderadosByEmail = _context.Apoderados.Where(x => x.email == apoderado.email).ToList();
                if (apoderadosByEmail.Count > 0) return Ok(new { code = 5000, message = "El Apoderado con email " + apoderado.email + " ya se encuentra registrado" });

                Apoderado newApoderado = new Apoderado()
                {
                    nombres = apoderado.nombres,
                    dni = apoderado.dni,
                    email = apoderado.email
                };

                long apoderadoId = insertApoderado_BasicDB(newApoderado);
                if (apoderadoId != 0)
                {
                    newApoderado.id = apoderadoId;

                    Usuario apoderadoUsuario = new Usuario()
                    {
                        tipo = "A",
                        apoderadoId = apoderadoId,
                        nombres = apoderado.nombres,
                        fechaNacimiento = DateTime.Now,
                        edad = 0
                    };
                    insertUsuarioDB(apoderadoUsuario);
                }
                else
                {
                    return Ok(new { code = 3000, message = "No se pudo guardar el Apoderado" });
                }

                return Ok(new { code = 1000, message = "Apoderado Guardardo", apoderado = createBasicApoderado(newApoderado) });
            }
            catch (Exception e)
            {
                return Ok(new { code = 4000, message = "No se pudo guardar el Apoderado " + e.Message });
            }
        }

      long insertApoderadoDB(Apoderado apoderado)
      {
         var dni = new SqlParameter("@dni", apoderado.dni);
         var nombres = new SqlParameter("@nombres", apoderado.nombres);
         var pais = new SqlParameter("@pais", apoderado.pais);
         var celular = new SqlParameter("@celular", apoderado.celular);
         var email = new SqlParameter("@email", apoderado.email);
         var firma = new SqlParameter("@firma", apoderado.firma);
         var apoderadoOut = new SqlParameter("@apoderadoId", SqlDbType.BigInt); apoderadoOut.Direction = ParameterDirection.Output;

         var sql = "EXEC dbo.insertApoderado @dni,@nombres,@pais,@celular,@email, @firma, @apoderadoId OUTPUT";
         var data = _context.Database.ExecuteSqlCommand(sql, dni, nombres, pais, celular, email, firma, apoderadoOut);

         return (long)apoderadoOut.Value;
      }

        long insertApoderado_BasicDB(Apoderado apoderado)
        {
            var dni = new SqlParameter("@dni", apoderado.dni);
            var nombres = new SqlParameter("@nombres", apoderado.nombres);
            var email = new SqlParameter("@email", apoderado.email);
            var apoderadoOut = new SqlParameter("@apoderadoId", SqlDbType.BigInt); apoderadoOut.Direction = ParameterDirection.Output;

            var sql = "EXEC dbo.insertApoderado_Basic @dni,@nombres,@email,@apoderadoId OUTPUT";
            var data = _context.Database.ExecuteSqlCommand(sql, dni, nombres, email, apoderadoOut);

            return (long)apoderadoOut.Value;
        }

      void insertUsuarioDB(Usuario usuario)
      {
         var tipo = new SqlParameter("@tipo", usuario.tipo);
         var apoderdoId = new SqlParameter("@apoderdoId", usuario.apoderadoId);
         var nombres = new SqlParameter("@nombres", usuario.nombres);
         var edad = new SqlParameter("@edad", usuario.edad);
         var fechaNacimiento = new SqlParameter("@fechaNacimiento", usuario.fechaNacimiento);

         var sql = "EXEC dbo.insertUsuario @tipo,@apoderdoId,@nombres,@edad,@fechaNacimiento";
         _context.Database.ExecuteSqlCommand(sql, tipo, apoderdoId, nombres, edad, fechaNacimiento);
      }

      void updateApoderadoDB(Apoderado apoderado, int updateFirma)
      {
         var id = new SqlParameter("@id", apoderado.id);
         var dni = new SqlParameter("@dni", apoderado.dni);
         var nombres = new SqlParameter("@nombres", apoderado.nombres);
         var celular = new SqlParameter("@celular", apoderado.celular);
         var email = new SqlParameter("@email", apoderado.email);
         var _updateFirma = new SqlParameter("@updateFirma", updateFirma);
         var firma = new SqlParameter("@firma", apoderado.firma);

         var sql = "EXEC dbo.updateApoderado @id,@dni,@nombres,@celular,@email,@updateFirma,@firma";
         _context.Database.ExecuteSqlCommand(sql, id, dni, nombres, celular, email, _updateFirma, firma);
      }
   }
}