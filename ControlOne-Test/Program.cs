using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ControlOne_Test
{
    class Program
    {

      static string createUniqueId(long id)
      {

         DateTime moment = DateTime.Now;

         var result = new StringBuilder();
         result.Append(String.Format("{0:000000000}", id));
         result.Append("-");
         result.Append(moment.Year.ToString());
         result.Append(String.Format("{0:00}", moment.Month));
         result.Append(String.Format("{0:00}", moment.Day));
         result.Append(String.Format("{0:00}", moment.Hour));
         result.Append(String.Format("{0:00}", moment.Minute));
         result.Append(String.Format("{0:00}", moment.Second));

         return result.ToString();
      }

      static void Main(string[] args)
      {
         Console.WriteLine(createUniqueId(160331));

         int v = 2;
         v += 2;
         string firma = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASwAAACWCAYAAABkW7XSAAAAAXNSR0IArs4c6QAAFgZJREFUeF7tXXnodkUVfgotS82M9iyLJMiUVjekzCIhy0wk27EyouUPKyojIysosyC1IhNCE9QWJXMpLP+wrKBs+UptI8n2BSUrl8oW49E7eb755r537jJzZ+59Bn78vu/3zp0588zc5z3nzJkzd4OKEBACQqASBO5WiZwSUwgIASEAEZYWgRAQAtUgIMKqZqokqBAQAiIsrQEhIASqQUCEVc1USVAhIAREWFoDQkAIVIOACKuaqZKgQkAIiLC0BoSAEKgGARFWNVMlQYWAEBBhaQ0IASFQDQIirGqmSoIKASEgwtIaEAJCoBoERFjVTJUEFQJCQIQ1zRp4etPM7c3vvwL4wTRNqxUhIAQcAiKsYWvhCQAeD+D5AJ4FYMdAM78E8CkA7xnWhZ4SAkLAR0CEFbcm7gvg8IagqE3x/7GFpPXK2MqqJwSEQDsCIqzNq+ORAE4A8IqRi4iEReJSEQJCYAQCSyYspxWRdFhoot0A3JED7JIIzN4I4OSIerFVjgDwhdjKqicEhMC2CCyRsOhfolZE/1JMoXP8L6biDgD2ArBTxMM/BPBVj4jYf4jotgB4UkSbqiIEhEALAksjrKm1Ih+2XzXkRJLijyU6W5cm5Jnew38E8BCtRCEgBIYjsBTCovlHbecRw6HofPLfALbvrHVXBZp/dNS7ciWA/Xo8r6pCQAh4CCyFsHxyiJloFytF3xZ/nGn4GQAP8ojm743jnfViC03SC0xlhje8O/Zh1RMCQmBbBJZAWCHziyMlIdFEpOnGQi1st+bvX9+wGBi2wDZZnzt7Qx3lfP5G08+FPfxqWqtCQAgEEKidsLgDSGe2jYu6FcBbAZy7wceUazFYze9rAFxEfK7+1Y8QWBQCtRMWNaCjvRk52GhVc0+WNQvpoN91boE29E/S5xcA5fRNXxLt7s3n3AX1A2edOe2e5W8StIoQmBSB2gmLL8YuBpGzJgjynBJg3yx0ePOlp0OeZuKcZw5p+h7UmKqh6P2bI8M7QpjxXCU3Qqhlcoz8dx8f4JTzoLYWgkDNhMVv/cu9eaAG0xZqMNeU2QPRJIW3APhgE8D6ewAPyywYtShqpSQrF1SbSwTODQmMXyzOt5irb/WzAARqJiy+bNd5c1DaeKhJ0cfG8k0AP/c0QG4M9DmXOGbJvRfAm0ZoTGP6Dj1LwqI8c2qYU49J7SVGoLQXvO9wnfbinivJf0WZ7A5myLxiRD6JJHU5BcCxGzpxppurQhLdv9HAGH92rXmWZp2vmdGsHFp0ZGkocit8rnbC+pYXjJmLAGKXylea9DOh+t8D8JTYhkbUayMrF7XPz6fyLZHoqFXu0YSQHA9guw7ZFe4xYnLX9mjthMXo8X3MpDGkgT6hOf1YLsMDdwg3mXs5tMHQLuovAByZyRSjVhnKFWbfMxHW2lhnxHhrJ6yPAXiDN376RJ44ApMxjzKSnVpeV8nhuwqdq5yDHLgxcl7zxULNiyTOw+Wu5CDurvnQ55UgUDthUZshQdnQBkJ/ahPlnmsaKAeP4dAcChVqfFbbyhF+4Yd8kKzoU5tT+3TYPKqJ6/IzZeSaL/VTKQK1ExZh98/suanI5cxtOxpEORg8ST/bcWZ9ULtyAZqplo2PSelR9gxRoeOev68GwKNT1MpUhMBWCCyBsDigNtKgaZhy27zt0DUd2i5LqR8rlsME8n1X1GimcqxP/Qr9GsDDA41yDPRRnjZ1h2qvXgSWQlicgZCDmX/nLhgzJUxpCtH0owkYCrykuUf/ET8jWVlTMJepSnLiURqn5ZV6hvFtAE7qeH1uAvA8BZrWSzJTSr4kwiIxUJtyL6rFiWRF4iJhjCUumltMzufvANLU42cMiHRkZQmNsU5tPq4p55Rt2fi0XCQ5ZAwxhMV2c/j8hsivZzIjsCTCInQkERIGr+AKFQZBfhbAOweaSG0xTfQRkaycc52alSUnmojUcnKYZYcC+KIZfGmxaf68MAVPV7Q/caNZq7JyBJZGWI60aJLxx989tNNNE5LaR4yPi2TEun5GU2pVJDGbmO8bAA701lVOH5JvGj+tcWKXvNRJql3JDXNiWDJWq5ZtiYTlJpTf2iQaElebxuXqOs2IgadXAfhpow3RpNsUAOrvRIac8Ll2K+1YHFFTs8t9wHnMC3U6gNe0NKCr0sYgu5Bnl0xYdoponn0CwN4A7j3B3NEfRRPP+sNCO5VnADhmgv5im/DDGXi4mBpgTeUAAOcDeKgntPxYNc1iIlnXQlhW66LGRXIJOee7YP5vE1v1DK8iCdHfEczpZHfi+D62Ws2oUOqg7wDYt2uC9PmyEVgbYdnZpDbCF8M5x58M4J6Bm3Hop6Kp5378FUHT03ey8xm2m8PJbuXhhoPLnFCbOejj+m2PoOb4Alj221/h6NZMWG3T5TIOuM+7Es2F4r/m8rfYcIbSo9u7Xhduhljfo3YKuxBbwecirHGTHDpgPKevZUmE9TMAjzHTQ42LObpUVoyACGv45Ntsoq6VkDN+eA/9n6wlwj1mZH5qGvrnuImgsmIERFjDJ983Wei3ok8sJq5reK+bn7Q+rJpNKP/yDo5aF9GmWjUVtSvCGjZZIb9VCSEE/i5hrfNLLD/sTU2OQ+PDVoOeyoZArQs6G0CBjkLpbOb0W1kR/QSCJd4iFDN3oUSIWqsxyC28jhZBvwkO3TTN8AH6s8Yequ4nSbj2iwB82nzE//PsZG3Fz9V/NoCX1zYIyTs9AiKsfpiGjt6kzrnVR0L/6rOSMzVsGpd/ILrWANg+c6e6EQiIsCJAaqqEjt6U4LfyR2A3A6j10Sysqfike0tBdynWhOMiZRVhxU1r6NLWUgMzfWLNffg6DtH2Wr7D/TIAh4xtVM8vAwERVtw82nABPjHX0ZsYaf2QAMrOHbZaym+bq9qcvC8DcE4twkvOtAiIsLrxDUWzl661+GEXpcvrZiFkdte609m9slSjNwIirM2QhXYFa3Bk+9kO6MuiljVnUGvX4qTMF3v+KpmDXait7HMR1uYJ93cF5z5602d5+mYsSYuaVtdh7j59TFW37dajuQ6RTzUutTMxAiKsdkBDL1FJIQxdSyF0ySxJi05tmoyllJDJ7fyEXbneSxmD5MiEgAgrDDRflC1eeuESQxi6lgkDWqlR+bntzwXw0q6HM3zeRlbsWmcHM0xAbV2IsMIz5p/JKzWEIWa9UdMiafkZVnk4mkdgeKwod+EXwocAvLql4xy3Y+ces/qbAAER1rYg+g7rkkMYYpcACeJSAPsFHiCZUXvM4ZAntkc3vrRNNxrJdxU7syurJ8LadsJtTil+WktIQMzS5UUcLwGws1eZvi1uMNC3RW1y6kJ/4LGRF8kqUd/U6C+oPRHW1pPpZwkoJQvDlEuO2hbJyeV+b2v7BgAXAfjoAO2LfTC9MTNbkKzanOc8drOjJ8BzAHxpygGrreUgIMK6ay79m2+WfukByYS+upjbg6iB+SbjHgC2ay7a2KEhpdg7EC9s/Gone6/SEr8glsMWBYxEhHXXJNhDw0vwW8UsL2o+1ID403XZbEx7XXVISCRJmt3XeZqXyKoLPX0OEdadi8DfXl+j05fk5a49OwrAgyfK9MB8YTRBHVERbz8gt+ZdWNFIRgREWHde5c5ve1dqOHqTa4kQG5qO/OG/rfn4TwA3AbgewG2NQC6JoTMhSUy+Kel/OSiEIddsLqAfEdadMUrOAa1v+rSLmlqcbwouaRc2LXpqffUmoT1+o2/69C+En0VC2mx6zBfVw5o1LJo4V5vsALqVJe3S9gNyS8qFn3bkan0yBNZMWAxQ3LdBUufWJltSwYZCZzNlCqbFfJGtr5WwrCn4EwB7LnJ2yxmUdgXLmYuqJVkjYdkAUZkl6Zevf3pAvsL0mC+2hzUSlt0VlFmSdmn7OcVIVgyRKDGJYFok1PokCKyNsOy3vSKrJ1lCrY0w3upEADy248oaA3LToryy1tdEWDZAVGZJ2oVOJ/sfRFZpQV5j62siLGsKKoQh7Wq3muztAF5VWFrmtKNX68kQWAth2eMgMgWTLac7GvZvGlLISFq8V9X6GgjLvkAyBdMvbz+a/VFNdob0PauHxSOwBsKyMUDaFUy7pP1bp6VdpcV7da0vnbDscRAdbE6/vP0wBt3anB7zVfWwdMKy+dllmqRf2labZVZRxlypCIHJEFgyYdmdKmUFmGzJbGyIO4KuyBzMg/mqelkqYdm8S3K051nShwL4ounqzQD8nO15JFEvi0VgqYRltasab2yuccH5/qvDAFxS40Akc7kILJGwrHbFw82xN7mUO0t1SOYfcpbDvY55q0rKJRKWjQPS2bV8y9GeJKAZ3nYXYT6J1NPiEFgaYdnzgtKu8i5XXjzhrp/XDmFe7FfT29IIS9rVPEuXOca2mK7lN5xnHhbf65IIy2pXS7+1ubSF6fuvnjjgevvSxiR5CkRgSYRlgxaVjSHvYpP/Ki/eq+1tKYT1VABXNLOoIzj5l7MNGBX++fFfTY9LISxe17VXM2vSrvIuX//mbEW458V/Vb0tgbD4wtBndR8A1wDYe1UzOP9gfYe7Qknmn5PFSrAEwrIOXzl751mqzoelW4jmwX81vdZOWPaCTsVdzbtsOReMxVIRAskQqJ2w7Pk1xf4kWyZqWAiUgUDthPUDAI8HoIwMZawnSSEEkiJQM2HZbKK6WCLpMlHjQqAMBGomLHsMR872MtaTpBACSRGolbBs7I8CFZMuETUuBMpBoFbCstqVbsIpZz1JEiGQFIFaCculMlEoQ9LlocaFQFkI1EhY1tmuYyBlrSdJIwSSIlAjYdnIdhFW0uWhxoVAWQjUSFhKI1PWGpI0QiAbAjUSls29pMtRsy0VdSQE5kegRsLibcJvBEDionmoIgSEwEoQqJGwVjI1GqYQEAI+AiIsrQkhIASqQUCEVc1USVAhIAREWFoDQkAIVIOACKuaqZKgQkAIiLC0BoSAEKgGARFWNVMlQReIALOO7A6AF3kwxfQvAVyoVNPtMy3CWuBbMMOQ+OLxZVPZGgES0S4NGfHfLDwLS3Jy//cx061DG1aRCEuvWBsCJKHTAHwrUMFpBPbFYwYN5tVn6p+pi3vx2S7lYg60nATJ/tmfvWSDl/fyark9GgKiXPxxpDQUA91NIMIaunaqfW6KG2z+DmCHnggwxz6zv44pPMlweKOBtGkhbP9SAM8e01FDMCTlnwJ4bnOvJUlpp4aI3G/XzbUAtjPENLL7rR7nvQQ8J8vTGznJeMoxJG9LGlZyiJN04DQbXhFPE4OFL7f7lneENUbjuQ3A9j2l54W2m0imrTmO4WgAJCvK3qecA+CTzVGtPs+x7hBS7tvHpvrUFHnEjERFslfpQECEVdYS8c0wZ3pRyk1+j7ZRjNF42PeJAL4daNwSI28tYqGGwDOeMSahI9kDALyj0WjGzsQNAB7Qs5EhpNyzi/9XJz6cDxKU+xna1mqfE2GVNfVTf+Pnysga63S3ucxSIE9zbtceDTtSJk7PBPC75othZwD3a8w/moWuUKPd9M6wHefrIjlRHv7m32Tm9ZiYtqoirAlAnLCJKb/x+Y1OgjhlQvnGNPUNAAdGNkDT0jq4/ReevjX6sFiorR3fkMstE2lrvpj/AXD35o/unaHTnTuANw80RyOhUDWLgAirrPXgvvG/7Pk0+PL6u1SU3N58/Q8Aj22GQ42HZkefwmeeBODP5iGSwe+b/78YwMF9GjR16Zu6wHv2bwA4Tsr8cwAfGamJXD5Cvq5hKQdbF0KZPhdhZQI6UTckMQYesvSN36FPjLtxJBMXG9QlZl+Ty7UXIix+Rp/XqV2dFvC5stwWMAkUQYRVyEQMEMNextHHV0VN6oRGO+vb7RiTy2optl+SAcnWmoB95Updn2b1sU0n1DL7aq+p5VtN+yKseqfaEkBssCE1nTN7hA5cDOA3AOhTGmMSOpQ3Od25u0htq8TtfSt3LNb1rqyCJRdhFTw5G0SzN1+zGnfGujQUqyV0jfoiAO9vCWnoerbrc+t3C9WlmUtZ/wTgVgD0dVGbPBsAAzfnKLqpaQ7UA32KsAqZiJ5iWPI5K8K8u64jOpsalIsNusYjhhcC2AfATQC+B+CSnrKGqnNzgaag87/FNnkegI/PYJJZH5auloudrQT1RFgJQM3QpLv5ml0d0bz8bd1+DsALWj4k2bUdBaFW83rvWZLa0J3CkAjUtth/H+KixsUzfAyTOCxCs5xiOqz5LcKaAtGBbYiwBgI342O+SdVmDu7Y+KtCZMUjISSKkPN4z+az0HPXA3hggrHTt8ZxcdeyT8lFHgwYdeV0AK/tI6TqToeACGs6LHO1RKe0Ow6zaXdwv0CmBcY7vQHAZS3C/ggACaut0Bzj86kKQy1IXPSf3QvAr5sA0UMB7BbolJom76bs8t+Nkdf3F7KtviEkY/rXswYBEVZdy8GGMlDyTf4rf0eO/p+jNgz3Rm/3kAGj9H39q0nnkvv8G8fqNECSKGVnOIZfuLPIeK5Uxcec/TDJHrVClcwIiLAyAz6yO6tdsalNW+z+rmDXi80jJjQjWWhq5SaoWGgY/sDMDrakvAE8FIrRhWXsWFSvJwIirJ6AzVidO2tberyo5zaxU+4RmoGHdMjP4y00d0o+qBsy0WJ2SodOXSgcRLFYQ9Ec+ZwIaySAGR/3ne1d0e3va1K35NJEMkJxR5yWizxnv0OPDMXIbEMaXH1Fu8cgl6COCCsBqImapEOafiZXuswSaiI0IZlRwJWpwxISDbWzWXdhgx1bKhIJBbqm6qtz4GuvIMKqawWQhOjspcnGb/6uEjJn6AOiSZNyZ61Lrik+988mpjTTmErmCiN0zMmCKcaoNjwERFjLXhIhLcuZUCQuOtdrIy6XlZXZRZ9spi91TJaNxdJ7M9N7I+BnAj5jtzFZPl3qXpIXk+O9GsD9AXy3iSh3pObSNLvffI5O+j4HlhlfxdxX3EBgXFff4rKyMtzC5pwXYfVFssL6IqwKJ22AyKFQgAHNBB/pu0NnNZUzABzTU5C2rKzf9zSuns12VpeG1QlR+goirPQYl9IDAyDp03JR8lPJ1YewfG1vyMURbVkn6GM6aKpBBdoRYSUEN7ZpEVYsUsup9y4AxwG49wRDYpYH7qLFmoSPDqSI6btzGYo8d0NJuXsnwppgwYxtQoQ1FsF6n6fzmr4o/mZQqrt6y15Fxb/bq8bcLTBjMm5+vskwYZHr43+yAbQ0D+9hGurTTt+Zc6Yoc+fTD6cyAwIirBlAX3mXoUj1PlkgbCAnHfCWPFISlrsg5HWFnwRY9PISYS16eoseHEmKO5GuXBXhXwuRnR3k/omypBYN5JqEE2GtabbLGuv5AI70RHocgB9vEPPKJvtpqMqXADynrCFKmqkREGFNjajai0WAphwzRLgLSt1zTCPDVDi20NfGew2dny3UB/N0DYnripVX9QpAQIRVwCSsWIQPNDuWPgTMLEHHPq8VI1nxOBJ9SH5hWAST/TFvF+vXFrW/4qkfNnQR1jDc9NR0CNhwgb6tngTg7X0fUv16EfgfK6qtxL6haqQAAAAASUVORK5CYII=";
         Send("jesusalvinoperu@gmail.com", "Declaracion de Exoneracion de Responsabilidad", "Jesus Alviño", "42238827", firma);
         Console.WriteLine(v);
         Console.ReadKey();
      }        

        static string bodytHtml(string apoderado, string dni, string firma)
        {
            return "" +
                "<div style='display:flex;align-items:center;height:100%;width:100%;flex-direction:column;'>" +
                    "<div style='padding-left:10px;padding-right:10px;padding-top:10px;margin-bottom:40px;font-size:13px;text-align:justify;background-color:#f8f9f694;max-width:800px;border:1px solid #c1bbbb;'>" +
                        "<div style='text-align:center;margin-bottom:10px;'><b>DECLARACIÓN DE EXONERACIÓN DE RESPONSABILIDAD</b></div>" +
                        "<span>Yo <b>" + apoderado + "</b>, identificado con DNI <b>" + dni + "</b></span>" +
                        "<span> responsable de los menores de edad, inscritos al final del documento, los cuales están bajo mi tutela o a mi cargo, elijo voluntariamente que usen las instalaciones de </span><span><b>XTREME PLAZA S.A.C.,</b> con <b>RUC 20505875843.</b> (en adelante, el establecimiento)</span></BR>En consideración a que se nos permita usar dichas instalaciones o juegos de entretenimiento y cualquier otro servicio proporcionado por el establecimiento, en dicha ubicación en Lima y/o provincias, declaro, reconozco y acepto lo siguiente:</span>                     </BR></BR>                     <p>1. Reconozco y acepto que el uso de los juegos de entretenimiento de propiedad del <b>Establecimiento</b>, pueden conllevar riesgos y tener consecuencias imprevisibles, los cuales incluyen lesiones físicas o emocionales graves, daños a mí mismo, a mis tutelados y/o a terceros. Entiendo que tales riesgos no pueden eliminarse, ya que forman parte inherente de las cualidades esenciales de la actividad recreativa, lo cual acepto y reconozco completamente de forma voluntaria. Soy el único responsable de mi salud y de mis tutelados en caso de cualquier accidente o deficiencia que nos pudiéramos causar como consecuencia de las actividades realizadas en el establecimiento.</p>                     </BR>                     <p>2. Reconozco y acepto que, si bien los empleados del establecimiento supervisan generalmente la zona de los juegos de entretenimiento, no es factible que dichos empleados supervisen las actividades y acciones de todos los clientes en todo momento o en forma simultánea. He sido informado por el establecimiento de las normas de seguridad que buscan prevenir accidentes e incidentes por lo que realizo las mismas.</p>                     </BR>                     <p>3. Me comprometo (incluyendo a mis hijos y/o tutelados) a cumplir estrictamente las normas de seguridad establecidas por el establecimiento. Con esto, de acuerdo con el literal a) del art. 74 del Código de los Niños y Adolescentes, es responsabilidad de los padres, velar por su desarrollo integral, reconociendo que el cuidado de mis menores hijos es de exclusiva responsabilidad mía, haciéndome responsable de todos los costos y/o gastos que se produzcan en la eventualidad que sucediera cualquier tipo de accidente que los involucre.</p>                     </BR>                     <p>4. Me hago responsable de todos los posibles riesgos, peligros y daños personales que pudiera sufrir (incluyendo la de mis hijos y/o tutelados) al participar de las actividades del establecimiento.</p>                     </BR>                     <p>Alguno de los riesgos incluye:</p>                     </BR>                     <p>Los participantes pueden llegar a sufrir cortes, raspones, golpes, contusiones a través del uso del juego en el establecimiento o el contacto con otros participantes o superficies con las que han contactado. Los participantes pueden torcerse, jalar, romperse o lesionarse gravemente externa o internamente la cabeza, cara, cuello, torso, columna vertebral, brazos, muñecas, manos, piernas, tobillos, pies u otras partes de cuerpo como resultado de caerse de los juegos de entretenimiento o ponerse en contacto con otros participantes.</p>                     </BR>                     <p>5. El participante se encuentra en las condiciones físicas idóneas, no presenta lesiones ni ninguna condición preexistente que le impida realizar dichas actividades.</p>                     </BR>                     <p>6. Reconozco que en ningún caso el Establecimiento, presta asesoramiento en cuestiones de salud, y que estas deberán ser consultadas previamente a la realización de las actividades con un profesional de la salud.</p>                     </BR>                     <p>7. Libero de cualquier responsabilidad al personal o representantes legales del Establecimiento, en caso de presentarse algunas de las situaciones mencionadas en los puntos que anteceden.</p>                     </BR>                     <p>8. Renuncio expresamente a iniciar cualquier tipo de acción judicial en contra del Establecimiento que busque obtener indemnización con respecto a las actividades realizadas en el mismo y a todos sus empleados, agentes y cualquier tercero relacionado al Establecimiento.</p>                     </BR>                     <p>9. Reconozco y acepto que el Establecimiento no se hace responsable de las pertenecías dejadas dentro del evento ni de los posibles hurtos o robos que pudieran sufrir dentro del Evento/Establecimiento.</p>                     </BR>                                   <p><b>Foto/video/renuncia a medios sociales.</b></p>                     <p>En relación con el uso de las instalaciones del Establecimiento, a las que yo y los menores de edad podríamos tener acceso, como tutor de los menores, doy mi consentimiento libre, previo, expreso e inequívoco para el tratamiento de mis datos personales y los de los menores. Autorizo a perpetuidad a XTREME PLAZA S.A.C. para la grabación de los menores de edad y mi imagen física, a través de medios fotográficos, digitales, para los siguientes propósitos: publicidad, promoción o publicitar cualquiera de sus instalaciones en Lima y/o provincias.</p>                     </BR>                     <p><b>Reglamento y prohibiciones.</b></p>                     <p>Reconozco haber sido informado sobre los tiempos de cada atracción, por lo que en caso de que dichos tiempos sean excedidos, me comprometo a pagar los cargos que se puedan generar por tal incumplimiento.                         Reconozco y declaro que antes de hacer uso de las instalaciones de Xtreme Plaza Sac, he leído el reglamento, por lo que manifiesto haber sido debidamente informado con relación a las reglas que rigen para mi y para los menores a mi cargo. Asimismo, manifiesto que entiendo y he explicado las referidas reglas a los menores a mi cargo, en caso no se sigan las reglas establecidas por Xtreme Plaza Sac, puedo ser expulsado y/o los menores a mi cargo, sin tener q esperar la devolución de mi dinero.                         Declaro así mismo, haber visualizado en la puerta de ingreso al juego, un panel que contiene las recomendaciones y advertencias sobre su uso. Declaro haber leído y entendido los términos de este documento.                     </p>                                        </BR></BR>" +
                        "<div style='justify-content:center;display:flex;align-items: center;'><a href =https://www.experienciasxtreme.com/pages/declaracion.html?dni=" + dni +">Ver Declaracion completa</a></div>" +
                "</div>" +
                "";
        }

        public static void Send(string to, string subject, string apoderado, string dni, string firma, bool inAnotherThread = false)
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
    }
}
