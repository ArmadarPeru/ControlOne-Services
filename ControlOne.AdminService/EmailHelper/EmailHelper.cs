using ControlOne.AdminService.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmailHelper
{
    public class EmailHelper
    {
        public static void Send(string from, string fromAlias, string fromPassword, string to, string toAlias, string subject, string body, bool inAnotherThread = false)
        {
            var fromAddress = new MailAddress(from, fromAlias);
            var toAddress = new MailAddress(to, toAlias);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            if (inAnotherThread == false)
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml=true
                    
                })
                {
                    smtp.Send(message);
                }
            }
            else
            {
                Thread t = new Thread(delegate ()
                {
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    })
                    {
                        smtp.Send(message);
                    }
                });
                t.Start();
            }
        }
        public static void Send(string from, string fromAlias, string fromPassword, List<Tuple<string, string>> to, string subject, string body, bool inAnotherThread = false)
        {
            var fromAddress = new MailAddress(from, fromAlias);

            var smtp = new SmtpClient
            {
                Host = "mail.experienciasxtreme.com",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            if (inAnotherThread == false)
            {
                using (var message = new MailMessage()
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = body
                })
                {
                    Utility.MakeBcc(message, to);
                    smtp.Send(message);
                }
            }
            else
            {
                Thread t = new Thread(delegate ()
                {
                    using (var message = new MailMessage()
                    {
                        From = fromAddress,
                        Subject = subject,
                        Body = body
                    })
                    {
                        Utility.MakeBcc(message, to);
                        smtp.Send(message);
                    }
                });
                t.Start();
            }
        }
        public static void Send(string to, string subject,string apoderado, string dni,string firma, bool inAnotherThread = false)
        {
            try
            {
                if (inAnotherThread == false)
                {
                    string from = "contratos@experienciasxtreme.com";
                    string fromPass = "Xtr_contr_210709";
                    MailMessage m = new MailMessage();
                    SmtpClient sc = new SmtpClient();

                    m.From = new MailAddress(from, "Experiencias Xtreme Contrato");
                    m.To.Add(to);
                    m.Bcc.Add("contratosbk@experienciasxtreme.com");

                    m.Subject = subject;
                    m.Body = bodytHtml(apoderado, dni, firma);
                    m.IsBodyHtml = true;
                    sc.Host = "mail.experienciasxtreme.com";

                    sc.Port = 25;
                    sc.UseDefaultCredentials = false;
                    sc.Credentials = new System.Net.NetworkCredential(from, fromPass);
                    sc.EnableSsl = false;

                    sc.Send(m);
                }
                else
                {
                    Thread t = new Thread(delegate ()
                    {
                        string from = "contratos@experienciasxtreme.com";
                        string fromPass = "Xtr_contr_210709";
                        MailMessage m = new MailMessage();
                        SmtpClient sc = new SmtpClient();

                        m.From = new MailAddress(from, "Experiencias Xtreme Contrato");
                        m.To.Add(to);
                        m.Bcc.Add("contratosbk@experienciasxtreme.com");

                        m.Subject = subject;
                        m.Body = bodytHtml(apoderado, dni, firma);
                        m.IsBodyHtml = true;
                        sc.Host = "mail.experienciasxtreme.com";

                        sc.Port = 25;
                        sc.UseDefaultCredentials = false;
                        sc.Credentials = new System.Net.NetworkCredential(from, fromPass);
                        sc.EnableSsl = false;

                        sc.Send(m);
                    });
                    t.Start();
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        static string bodytHtml(string apoderado,string dni, string firma)
        {
            return "" +
                "<div style='display:flex;align-items:center;height:100%;width:100%;flex-direction:column;'>" +
                    "<div style='padding-left:10px;padding-right:10px;padding-top:10px;margin-bottom:40px;font-size:13px;text-align:justify;background-color:#f8f9f694;max-width:800px;border:1px solid #c1bbbb;'>" +
                        "<div style='text-align:center;margin-bottom:10px;'><b>DECLARACIÓN DE EXONERACIÓN DE RESPONSABILIDAD</b></div>" +
                        "<span>Yo <b>" + apoderado + "</b>, identificado con DNI <b>" + dni + "</b></span>" +
                        "<span> responsable de los menores de edad, inscritos al final del documento, los cuales están bajo mi tutela o a mi cargo, elijo voluntariamente que usen las instalaciones de </span><span><b>XTREME PLAZA S.A.C.,</b> con <b>RUC 20505875843.</b> (en adelante, el establecimiento)</span></BR>En consideración a que se nos permita usar dichas instalaciones o juegos de entretenimiento y cualquier otro servicio proporcionado por el establecimiento, en dicha ubicación en Lima y/o provincias, declaro, reconozco y acepto lo siguiente:</span>                     </BR></BR>                     <p>1. Reconozco y acepto que el uso de los juegos de entretenimiento de propiedad del <b>Establecimiento</b>, pueden conllevar riesgos y tener consecuencias imprevisibles, los cuales incluyen lesiones físicas o emocionales graves, daños a mí mismo, a mis tutelados y/o a terceros. Entiendo que tales riesgos no pueden eliminarse, ya que forman parte inherente de las cualidades esenciales de la actividad recreativa, lo cual acepto y reconozco completamente de forma voluntaria. Soy el único responsable de mi salud y de mis tutelados en caso de cualquier accidente o deficiencia que nos pudiéramos causar como consecuencia de las actividades realizadas en el establecimiento.</p>                     </BR>                     <p>2. Reconozco y acepto que, si bien los empleados del establecimiento supervisan generalmente la zona de los juegos de entretenimiento, no es factible que dichos empleados supervisen las actividades y acciones de todos los clientes en todo momento o en forma simultánea. He sido informado por el establecimiento de las normas de seguridad que buscan prevenir accidentes e incidentes por lo que realizo las mismas.</p>                     </BR>                     <p>3. Me comprometo (incluyendo a mis hijos y/o tutelados) a cumplir estrictamente las normas de seguridad establecidas por el establecimiento. Con esto, de acuerdo con el literal a) del art. 74 del Código de los Niños y Adolescentes, es responsabilidad de los padres, velar por su desarrollo integral, reconociendo que el cuidado de mis menores hijos es de exclusiva responsabilidad mía, haciéndome responsable de todos los costos y/o gastos que se produzcan en la eventualidad que sucediera cualquier tipo de accidente que los involucre.</p>                     </BR>                     <p>4. Me hago responsable de todos los posibles riesgos, peligros y daños personales que pudiera sufrir (incluyendo la de mis hijos y/o tutelados) al participar de las actividades del establecimiento.</p>                     </BR>                     <p>Alguno de los riesgos incluye:</p>                     </BR>                     <p>Los participantes pueden llegar a sufrir cortes, raspones, golpes, contusiones a través del uso del juego en el establecimiento o el contacto con otros participantes o superficies con las que han contactado. Los participantes pueden torcerse, jalar, romperse o lesionarse gravemente externa o internamente la cabeza, cara, cuello, torso, columna vertebral, brazos, muñecas, manos, piernas, tobillos, pies u otras partes de cuerpo como resultado de caerse de los juegos de entretenimiento o ponerse en contacto con otros participantes.</p>                     </BR>                     <p>5. El participante se encuentra en las condiciones físicas idóneas, no presenta lesiones ni ninguna condición preexistente que le impida realizar dichas actividades.</p>                     </BR>                     <p>6. Reconozco que en ningún caso el Establecimiento, presta asesoramiento en cuestiones de salud, y que estas deberán ser consultadas previamente a la realización de las actividades con un profesional de la salud.</p>                     </BR>                     <p>7. Libero de cualquier responsabilidad al personal o representantes legales del Establecimiento, en caso de presentarse algunas de las situaciones mencionadas en los puntos que anteceden.</p>                     </BR>                     <p>8. Renuncio expresamente a iniciar cualquier tipo de acción judicial en contra del Establecimiento que busque obtener indemnización con respecto a las actividades realizadas en el mismo y a todos sus empleados, agentes y cualquier tercero relacionado al Establecimiento.</p>                     </BR>                     <p>9. Reconozco y acepto que el Establecimiento no se hace responsable de las pertenecías dejadas dentro del evento ni de los posibles hurtos o robos que pudieran sufrir dentro del Evento/Establecimiento.</p>                     </BR>                                   <p><b>Foto/video/renuncia a medios sociales.</b></p>                     <p>En relación con el uso de las instalaciones del Establecimiento, a las que yo y los menores de edad podríamos tener acceso, como tutor de los menores, doy mi consentimiento libre, previo, expreso e inequívoco para el tratamiento de mis datos personales y los de los menores. Autorizo a perpetuidad a XTREME PLAZA S.A.C. para la grabación de los menores de edad y mi imagen física, a través de medios fotográficos, digitales, para los siguientes propósitos: publicidad, promoción o publicitar cualquiera de sus instalaciones en Lima y/o provincias.</p>                     </BR>                     <p><b>Reglamento y prohibiciones.</b></p>                     <p>Reconozco haber sido informado sobre los tiempos de cada atracción, por lo que en caso de que dichos tiempos sean excedidos, me comprometo a pagar los cargos que se puedan generar por tal incumplimiento.                         Reconozco y declaro que antes de hacer uso de las instalaciones de Xtreme Plaza Sac, he leído el reglamento, por lo que manifiesto haber sido debidamente informado con relación a las reglas que rigen para mi y para los menores a mi cargo. Asimismo, manifiesto que entiendo y he explicado las referidas reglas a los menores a mi cargo, en caso no se sigan las reglas establecidas por Xtreme Plaza Sac, puedo ser expulsado y/o los menores a mi cargo, sin tener q esperar la devolución de mi dinero.                         Declaro así mismo, haber visualizado en la puerta de ingreso al juego, un panel que contiene las recomendaciones y advertencias sobre su uso. Declaro haber leído y entendido los términos de este documento.                     </p>                                        </BR></BR>" +
                        "<div style='justify-content:center;display:flex;align-items: center;'><a href =https://www.experienciasxtreme.com/pages/declaracion.html?dni=" + dni + ">Ver Declaracion completa</a></div>" +
                "</div>" +
                "";
        }

        public static void Send(string to, string subject, string efectivoCajaInicial, string ventaEfectivo, string ventaOtros, string ventaPOS, string ventaTotal,List<Concepto> gastos,string gastosTotal,string efectivoCaja, List<Concepto> abonos,string abonosTotal,string efectivoFinalCaja,string faltanteVenta,string sobranteVenta,string cierreCaja)
        {
            try
            {
                string from = "postmaster@experienciasxtreme.com";
                string fromPass = "XtremPlaza2021*";

                MailMessage m = new MailMessage();
                SmtpClient sc = new SmtpClient();

                m.From = new MailAddress(from, "Experiencias Xtreme Postmaster");
                m.To.Add(to);                

                m.Subject = subject;

                m.Body = bodyCierreCaja(efectivoCajaInicial, ventaEfectivo, ventaOtros, ventaPOS, ventaTotal, gastos, gastosTotal, efectivoCaja, abonos, abonosTotal, efectivoFinalCaja, faltanteVenta, sobranteVenta, cierreCaja);
                m.IsBodyHtml = true;
                sc.Host = "mail.experienciasxtreme.com";

                sc.Port = 25;
                sc.UseDefaultCredentials = false;
                sc.Credentials = new System.Net.NetworkCredential(from, fromPass);
                sc.EnableSsl = false;

                sc.Send(m);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static string bodyCierreCaja(string efectivoCajaInicial, string ventaEfectivo, string ventaOtros, string ventaPOS, string ventaTotal, List<Concepto> gastos, string gastosTotal, string efectivoCaja, List<Concepto> abonos, string abonosTotal, string efectivoFinalCaja, string faltanteVenta, string sobranteVenta, string cierreCaja)
        {
            return
                @"<table style ='width:400px;font-size:14;'>" +
               "<tr style='background-color:gainsboro;'>" +
                 "<th style='width:200px;'>Concepto</th>" +
                 "<th style='width:100px;text-align: right;'>Importe</th>" +
                 "<th style='width:100px;text-align: right;'>Totales</th>" +
               "</tr>" +

               "<tr>" +
                 "<td style='font-weight:700;'>Efectivo Inical Caja</td>" +
                 "<td style='text-align:right;'></td>" +
                 "<td style='font-weight:700;text-align:right;color:#89c402;'>" + efectivoCajaInicial + "</td>" +
               "</tr>" +

               "<tr>" +
                 "<td>Venta Efectivo</td>" +
                 "<td style='text-align:right;'>" + ventaEfectivo + "</td>" +
                 "<td style='text-align:right;'>" + ventaEfectivo + "</td>" +
               "</tr>" +

               "<tr>" +
                 "<td>Venta Otros</td>" +
                 "<td style='text-align:right;'>" + ventaOtros + "</td>" +
                 "<td style='text-align:right;'></td>" +
               "</tr>" +

               "<tr>" +
                 "<td>Venta POS</td>" +
                 "<td style='text-align:right;'>" + ventaPOS + "</td>" +
                 "<td style='text-align:right;'></td>" +
               "</tr>" +

               "<tr>" +
                 "<td style='font-weight:700;'>Venta Total</td>" +
                 "<td style='text-align:right;font-weight:700;'>" + ventaTotal + "</td>" +
                 "<td style='text-align:right;'></td>" +
               "</tr>" +

                line() +

               "<tr>" +
                 "<td style='font-weight:700;'>Gastos</td>" +
                 "<td></td>" +
                 "<td></td>" +
               "</tr>" +

            getListaConcepto(gastos) +

               "<tr>" +
                 "<td>Total</td>" +
                 "<td style='text-align:right;'>" + gastosTotal + "</td>" +
                 "<td style='text-align:right;font-weight:700;'>" + gastosTotal + "</td>" +
               "</tr>" +

               line() +

               "<tr>" +
                 "<td style='font-weight:700;'>Efectivo Caja</td>" +
                 "<td></td>" +
                 "<td style='font-weight:700;text-align:right;'>" + efectivoCaja + "</td>" +
               "</tr>" +

               line() +

               "<tr>" +
                 "<td style='font-weight:700;'>Abonos/Entregas</td>" +
                 "<td></td>" +
                 "<td></td>" +
               "</tr>" +

               getListaConcepto(abonos) +

               "<tr>" +
                 "<td>Total</td>" +
                 "<td style='text-align:right;'>" + abonosTotal + "</td>" +
                 "<td style='text-align:right;font-weight:700;'>" + abonosTotal + "</td>" +
               "</tr>" +

               line() +

               "<tr>" +
                 "<td style='font-weight:700;'>Efectivo Final Caja</td>" +
                 "<td></td>" +
                 "<td style='font-weight:700;text-align:right;'>" + efectivoFinalCaja + "</td>" +
               "</tr>" +

                line() +

               "<tr>" +
                 "<td>Faltante Venta</td>" +
                 "<td></td>" +
                 "<td style='text-align:right;'>" + faltanteVenta + "</td>" +
               "</tr>" +

               "<tr>" +
                 "<td>Sobrante Venta</td>" +
                 "<td></td>" +
                 "<td style='text-align:right;'>" + sobranteVenta + "</td>" +
               "</tr>" +

               line() +

               "<tr>" +
                 "<td style='font-weight:700;'>Cierre Caja</td>" +
                 "<td></td>" +
                 "<td style='font-weight:700;text-align:right;color:#89c402;'>" + cierreCaja + "</td>" +
               "</tr>" +
             "</table>";
        }

        static string getListaConcepto(List<Concepto> conceptos)
        {
            StringBuilder result = new StringBuilder();
            foreach (Concepto concepto in conceptos)
            {
                result.Append(gastoAbono(concepto.concepto, concepto.importe.ToString()));
            }
            return result.ToString();
        }
        static string line()
        {
            return
               @"<tr>" +
                 "<td style='border-top: 1px solid rgb(172, 170, 170);'></td>" +
                 "<td style='border-top: 1px solid rgb(172, 170, 170);'></td>" +
                 "<td style='border-top: 1px solid rgb(172, 170, 170);'></td>" +
               "</tr>";
        }

        static string gastoAbono(string concepto, string monto)
        {
            return
                @"<tr>" +
                "<td>" + concepto + "</td>" +
                "<td style='text-align:right;'>" + monto + "</td>" +
                "<td style='text-align:right;'></td>" +
                "</tr>";
        }

        public static void Send(string from, string fromAlias, string fromPassword, List<string> to, string subject, string body, bool inAnotherThread = false)
        {
            var fromAddress = new MailAddress(from, fromAlias);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            if (inAnotherThread == false)
            {
                using (var message = new MailMessage()
                {
                    From = fromAddress,
                    Subject = subject
                })
                {
                    Utility.MakeBcc(message, to);
                    smtp.Send(message);
                }
            }
            else
            {
                Thread t = new Thread(delegate ()
                {
                    using (var message = new MailMessage()
                    {
                        From = fromAddress,
                        Subject = subject
                    })
                    {
                        Utility.MakeBcc(message, to);
                        smtp.Send(message);
                    }
                });
                t.Start();
            }
        }
    }
}