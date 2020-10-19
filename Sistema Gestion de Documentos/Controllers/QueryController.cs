using Sistema_Gestion_de_Documentos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Gestion_de_Documentos.Controllers
{
    public class QueryController : Controller
    {
        ApplicationDbContext DB = new ApplicationDbContext();

        public ActionResult Consulta()
        {
            try
            {
                List<departamentos> ListaDep = new List<departamentos>();

                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = ""; Session["MessageRemi"] = ""; Session["MessageSubserie"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    if (Session["query"] as string != "")
                    {
                        string StrSql = Session["query"] as string;
                        var ListaConsulta = DB.Database.SqlQuery<RegistroConsultas>(StrSql).ToList();
                        ViewBag.registros = ListaConsulta;
                    }
                    else
                    { 
                        ViewBag.registros = null;
                    }
                    
                    ListaDep = DB.departamentos.ToList();
                    ViewBag.departamentos = ListaDep;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageConsulta"] as string))
                    {
                        ViewBag.msgConsulta = Session["MessageConsulta"] as string;
                    }
                    else
                    {
                        ViewBag.msgConsulta = null;
                    }

                    ViewBag.nivel = Session["Nivel"] as string;

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageConsulta"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Consulta(string select_tipo, string fecha_inicial, string fecha_final, string asunto2,
            int? select_departamento2, string no_doc, int? select_remitente2, int? select_institucion2)
        {
            try
            {                
                List<departamentos> ListaDep = new List<departamentos>();                

                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageConsulta"] = "";

                    //Para evitar injección de código en esta consulta *******************************************
                    if (asunto2.Length > 0)
                    {                        
                        int p1 = asunto2.IndexOf(";"); int p2 = asunto2.IndexOf("-"); int p3 = asunto2.IndexOf("/");
                        int p4 = asunto2.IndexOf("="); int p5 = asunto2.IndexOf("'");

                        if (p1 > 0 || p2 > 0 || p3 > 0 || p4 > 0 || p5 > 0)
                        {
                            Session["MessageConsulta"] = "Error: hay caracteres no permitidos en el asunto";
                            return RedirectToAction("Consulta", "Query");
                        }
                    }

                    if (no_doc.Length > 0)
                    {
                        int p1 = no_doc.IndexOf(";"); int p2 = no_doc.IndexOf("-"); int p3 = no_doc.IndexOf("/");
                        int p4 = no_doc.IndexOf("="); int p5 = no_doc.IndexOf("'");

                        if (p1 > 0 || p2 > 0 || p3 > 0 || p4 > 0 || p5 > 0)
                        {
                            Session["MessageConsulta"] = "Error: hay caracteres no permitidos en No. Archivo";
                            return RedirectToAction("Consulta", "Query");
                        }
                    }
                    //********************************************************************************************

                    string F1 = ""; string F2 = "";

                    if (fecha_inicial.Length > 0 || fecha_final.Length > 0)
                    {
                        if (fecha_inicial.Length == 0 || fecha_final.Length == 0)
                        {
                            Session["MessageConsulta"] = "Error: Debe seleccionar fecha inicial y fecha final (si va a usar fechas)";
                            return RedirectToAction("Consulta", "Query");
                        }
                        else
                        {
                            DateTime FechaIni = DateTime.Parse(fecha_inicial);
                            DateTime FechaFin = DateTime.Parse(fecha_final);

                            string Dia = ""; string Mes = ""; string Anio = "";
                            Dia = FechaIni.Day.ToString(); Mes = FechaIni.Month.ToString(); Anio = FechaIni.Year.ToString();
                            if(Dia.Length == 1)
                            {
                                Dia = "0" + Dia;
                            }

                            if (Mes.Length == 1)
                            {
                                Mes = "0" + Mes;
                            }
                            
                            F1 = Dia + "/" + Mes + "/" + Anio;


                            Dia = FechaFin.Day.ToString(); Mes = FechaFin.Month.ToString(); Anio = FechaFin.Year.ToString();
                            if (Dia.Length == 1)
                            {
                                Dia = "0" + Dia;
                            }

                            if (Mes.Length == 1)
                            {
                                Mes = "0" + Mes;
                            }
                            
                            F2 = Dia + "/" + Mes + "/" + Anio;
                        }
                    }

                    string Condicion = ""; string Criterio = "";
                    string StrSql = "";

                    if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0
                        && select_departamento2 != null && select_remitente2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2
                            + " and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2
                            + " and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0
                        && select_departamento2 != null && select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2
                            + " and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2
                            + " and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0
                        && select_departamento2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2
                            + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2
                            + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0
                        && select_remitente2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2;
                        }
                    }
                    
                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0 && select_departamento2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0 && select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_remitente2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_remitente2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0 && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_institucion2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_institucion2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && select_remitente2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_remitente2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_remitente2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && select_departamento2 != null && select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.id_departamento =" + select_departamento2 + " and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.id_departamento =" + select_departamento2 + " and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && select_departamento2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.id_departamento =" + select_departamento2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.id_departamento =" + select_departamento2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && asunto2.Length > 0)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'";

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.titulo_asunto like '" + asunto2 + "%'";

                            Criterio = "Desde el " + F1 + " Hasta el " + F2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && no_doc.Length > 0)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.numero_correspondencia ='" + no_doc + "'";

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", No. archivo " + no_doc + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.numero_correspondencia ='" + no_doc + "'";

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", No. archivo " + no_doc;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && select_departamento2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.id_departamento =" + select_departamento2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.id_departamento =" + select_departamento2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", del departamento " + select_departamento2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_remitente2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.persona =" + select_remitente2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_remitente2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0 && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_institucion2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final
                            + "' and r.institucion =" + select_institucion2;

                            Criterio = "Desde el " + F1 + " Hasta el " + F2 + ", de " + select_institucion2;
                        }
                    }

                    else if (asunto2.Length > 0 && select_departamento2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2;

                            Criterio = "Comunicaciones del Departamento " + select_departamento2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.titulo_asunto like '" + asunto2 + "%'" + " and r.id_departamento =" + select_departamento2;

                            Criterio = "Comunicaciones del Departamento " + select_departamento2;
                        }
                    }
                    
                    else if (asunto2.Length > 0 && select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.persona =" + select_remitente2;

                            Criterio = "Comunicaciones de " + select_remitente2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.titulo_asunto like '" + asunto2 + "%'" + " and r.persona =" + select_remitente2;

                            Criterio = "Comunicaciones del Departamento " + select_remitente2;
                        }
                    }

                    else if (asunto2.Length > 0 && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.titulo_asunto like '" + asunto2 + "%'" + " and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones de " + select_institucion2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.titulo_asunto like '" + asunto2 + "%'" + " and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones de " + select_institucion2;
                        }
                    }
                    
                    else if (select_departamento2 != null && select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.id_departamento =" + select_departamento2 + " and r.persona =" + select_remitente2;

                            Criterio = "Comunicaciones del departamento " + select_departamento2 + " y de " + select_remitente2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.id_departamento =" + select_departamento2 + " and r.persona =" + select_remitente2;

                            Criterio = "Comunicaciones del departamento " + select_departamento2 + " y de " + select_remitente2;
                        }
                    }

                    else if (select_departamento2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.id_departamento =" + select_departamento2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones del departamento " + select_departamento2 + " y de " + select_institucion2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.id_departamento =" + select_departamento2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones del departamento " + select_departamento2 + " y de " + select_institucion2;
                        }
                    }

                    else if (select_remitente2 != null && select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones de " + select_remitente2 + " y de " + select_institucion2 + ", del Tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.persona =" + select_remitente2 + " and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones de " + select_remitente2 + " y de " + select_institucion2;
                        }
                    }

                    else if (fecha_inicial.Length > 0 && fecha_final.Length > 0)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final + "'";

                            Criterio = "Comunicaciones Desde el " + F1 + " Hasta el " + F2 + ", del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.fecha_correspondencia >='" + fecha_inicial + "' and r.fecha_correspondencia <='" + fecha_final + "'";

                            Criterio = "Comunicaciones Desde el " + F1 + " Hasta el " + F2;
                        }
                    }

                    else if (no_doc.Length > 0)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.numero_correspondencia ='" + no_doc + "'";

                            Criterio = "Comunicaciones con No. archivo " + no_doc + ", del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.numero_correspondencia ='" + no_doc + "'";

                            Criterio = "Comunicaciones con No. archivo " + no_doc;
                        }
                    }

                    else if (asunto2.Length > 0)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.titulo_asunto like '" + asunto2 + "%'";

                            Criterio = "Comunicaciones con el asunto " + asunto2 + ", del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.titulo_asunto like '" + asunto2 + "%'";

                            Criterio = "Comunicaciones con el asunto " + asunto2;
                        }
                    }

                    else if (select_departamento2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.id_departamento =" + select_departamento2;

                            Criterio = "Comunicaciones del departamento " + select_departamento2 + ", del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.id_departamento =" + select_departamento2;

                            Criterio = "Comunicaciones del departamento " + select_departamento2;
                        }
                    }

                    else if (select_remitente2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.persona =" + select_remitente2;

                            Criterio = "Comunicaciones de " + select_remitente2 + ", del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.persona =" + select_remitente2;

                            Criterio = "Comunicaciones de " + select_remitente2;
                        }
                    }

                    else if (select_institucion2 != null)
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo
                            + "' and r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones de " + select_institucion2 + ", del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = " WHERE r.institucion =" + select_institucion2;

                            Criterio = "Comunicaciones de " + select_institucion2;
                        }
                    }

                    else
                    {
                        if (select_tipo != "T")
                        {
                            Condicion = " WHERE r.tipo_correspondencia ='" + select_tipo + "'";

                            Criterio = "Comunicaciones del tipo " + select_tipo;
                        }
                        else
                        {
                            Condicion = "";

                            Criterio = "Todas las Comunicaciones";
                        }
                    }

                    //Consulta para el Reporte de comunicaciones******************************************************************
                    string StrSql2 = "SELECT r.reg_cor_id, r.tipo_correspondencia, r.numero_correspondencia,"
                        + "r.fecha_correspondencia, p.nombre as remitente, i.nombre_institucional as institucion,"
                        + "r.numero_remision, r.titulo_asunto, s.titulo as nombre_serie,"
                        + "ss.titulo as nombre_subserie, r.id_departamento, d.nombre as nombre_dep, r.ruta_archivo, r.id_usuario"
                        + " FROM registro_correspondencia r"
                        + " LEFT JOIN persona p ON r.PERSONA = p.PER_ID"
                        + " LEFT JOIN institucional i ON r.INSTITUCION = i.CRE_ID"
                        + " LEFT JOIN serie s ON r.CODIGO_ACP_SERIE = s.SER_ID"
                        + " JOIN departamentos d ON r.ID_DEPARTAMENTO = d.id"
                        + " LEFT JOIN subserie ss on r.NUMERO_ACP_SUBSERIE = ss.SUB_ID"
                        + Condicion + " order by r.reg_cor_id desc";

                    Session["query2"] = StrSql2; Session["Criterio"] = Criterio;
                    //****************************************************************************************************************

                    StrSql = "SELECT TOP 125  r.reg_cor_id, r.tipo_correspondencia, r.numero_correspondencia,"
                        + "r.numero_remision, r.fecha_correspondencia, p.nombre as persona, i.nombre_institucional as institucion,"
                        + "r.titulo_asunto, r.ruta_archivo, r.acuse"
                        + " FROM registro_correspondencia r"
                        + " LEFT JOIN persona p ON r.PERSONA = p.PER_ID"
                        + " LEFT JOIN institucional i ON r.INSTITUCION = i.CRE_ID"
                        + " LEFT JOIN serie s ON r.CODIGO_ACP_SERIE = s.SER_ID"
                        + " JOIN departamentos d ON r.ID_DEPARTAMENTO = d.id"
                        + " LEFT JOIN subserie ss on r.NUMERO_ACP_SUBSERIE = ss.SUB_ID"
                        + Condicion + " order by r.reg_cor_id desc";                    

                    var ListaConsulta = DB.Database.SqlQuery<RegistroConsultas>(StrSql).ToList();
                    ViewBag.registros = ListaConsulta;
                   
                    ListaDep = DB.departamentos.ToList();
                    ViewBag.departamentos = ListaDep;

                    Session["query"] = StrSql;

                    ViewBag.nivel = Session["Nivel"] as string;

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["query"] = "";
                Session["MessageConsulta"] = "Error: " + ex.Message;
                return RedirectToAction("Consulta", "Query");
            }
        }
       
        public ActionResult Subir_acuse(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageConsulta"] = "";

                    string StrSql = "SELECT r.reg_cor_id, r.tipo_correspondencia, r.numero_correspondencia,"
                    + "r.numero_remision, r.fecha_correspondencia, p.nombre as persona, i.nombre_institucional as institucion,"
                    + "r.titulo_asunto, r.ruta_archivo, r.acuse"
                    + " FROM registro_correspondencia r"
                    + " LEFT JOIN persona p ON r.PERSONA = p.PER_ID"
                    + " LEFT JOIN institucional i ON r.INSTITUCION = i.CRE_ID"
                    + " LEFT JOIN serie s ON r.CODIGO_ACP_SERIE = s.SER_ID"
                    + " JOIN departamentos d ON r.ID_DEPARTAMENTO = d.id"
                    + " LEFT JOIN subserie ss on r.NUMERO_ACP_SUBSERIE = ss.SUB_ID"
                    + " WHERE r.reg_cor_id =" + id + " order by r.reg_cor_id desc";

                    var ListaConsulta = DB.Database.SqlQuery<RegistroConsultas>(StrSql).ToList();
                    ViewBag.datos = ListaConsulta;                    

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["query"] = "";
                Session["MessageConsulta"] = "Error: " + ex.Message;
                return RedirectToAction("Consulta", "Query");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subir_acuse(HttpPostedFileBase acuse_pdf, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageConsulta"] = "";

                    string NombreArchivo = null;
                    if (acuse_pdf != null)
                    {
                        NombreArchivo = acuse_pdf.FileName;
                    }

                    int idReg = int.Parse(id_registro);

                    int Resul = DB.Database.ExecuteSqlCommand("UPDATE registro_correspondencia SET acuse ='" + NombreArchivo + "' WHERE reg_cor_id =" +  idReg);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageConsulta"] = "Registro eliminado";

                        if (acuse_pdf != null)
                        {
                            string path = Server.MapPath("~/acuses/" + acuse_pdf.FileName);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            acuse_pdf.SaveAs(path);
                        }

                        return RedirectToAction("Consulta", "Query");
                    }
                    else
                    {
                        Session["MessageConsulta"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Consulta", "Query");
                    }
                }

            }
            catch (Exception ex)
            {
                Session["MessageConsulta"] = "Error: " + ex.Message;
                return RedirectToAction("Consulta", "Query");
            }
        }


    }
}