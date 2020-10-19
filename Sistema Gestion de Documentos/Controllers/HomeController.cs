using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Sistema_Gestion_de_Documentos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Gestion_de_Documentos.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext DB = new ApplicationDbContext();

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string usuario, string clave)
        {
            try

            {
                var ListaUsuario = DB.usuarios.Where(x => x.usuario == usuario && x.clave == clave).ToList();

                if (ListaUsuario.Count() > 0)
                {
                    Session["Login"] = "true"; Session["query"] = ""; Session["query2"] = ""; Session["Criterio"] = "";
                    Session["NombreUsuario"] = ListaUsuario[0].nombre_completo;
                    Session["IdUsuario"] = ListaUsuario[0].idusuario.ToString();
                    Session["Nivel"] = ListaUsuario[0].nivel.ToString();

                    Session["Message"] = ""; Session["MessageConsulta"] = ""; Session["MessageRemi"] = ""; Session["MessageSubserie"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    Session["Login"] = "false";
                    return RedirectToAction("Login", "Home");
                }

            }

            catch (Exception ex)
            {
                ViewBag.Error = "ERROR: " + ex.Message;
                return View();
            }

        }

        public ActionResult Index()
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    Session["Login"] = "false";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    List<departamentos> ListaDep = new List<departamentos>();
                    List<Registros> ListaRegistros = new List<Registros>();
                    List<serie> ListaSeries = new List<serie>();
                    string StrSql = "";

                   Session["MessageConsulta"] = ""; Session["MessageRemi"] = ""; Session["MessageSubserie"] = "";
                   Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["Message"] as string))
                    {
                        ViewBag.msg = Session["Message"] as string;
                    }
                    else
                    {
                        ViewBag.msg = null;
                    }

                    StrSql = "SELECT TOP 25 r.reg_cor_id, r.tipo_correspondencia, r.numero_correspondencia,"
                    + "r.fecha_correspondencia, i.nombre_institucional as institucion, p.nombre as persona, r.numero_remision,"
                    + "r.titulo_asunto FROM registro_correspondencia r"
                    + " LEFT JOIN persona p ON r.persona = p.per_id"
                    + " LEFT JOIN institucional i ON r.institucion = i.cre_id"
                    + " order by r.reg_cor_id desc";
                    ListaRegistros = DB.Database.SqlQuery<Registros>(StrSql).ToList();
                    ViewBag.documentos = ListaRegistros;

                    StrSql = "SELECT ser_id, titulo, siglas FROM SERIE ORDER BY TITULO ASC";
                    ListaSeries = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.series = ListaSeries;

                    ListaDep = DB.departamentos.ToList();
                    ViewBag.departamentos = ListaDep;

                    ViewBag.nivel = Session["Nivel"] as string;
                    ViewBag.seleccion = null;

                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(string tipo, string no_doc, string select_remitente, string no_rem,
            string select_institucion, string select_serie, string select_sub_serie, string fecha_ing,
            string select_departamento, string asunto, HttpPostedFileBase archivo_pdf)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = "";

                    DateTime fecha = DateTime.Parse(fecha_ing);                   
                    string Anio = fecha.Year.ToString();

                    string FechaInicial = Anio + "-01-01";
                    string FechaFinal = Anio + "-12-31" ;

                    int IdReg = DB.Database.SqlQuery<int>("Select REG_COR_ID from registro_correspondencia" +
                    " where tipo_correspondencia = @t and numero_correspondencia = @n" +
                    " and fecha_correspondencia >= @f1 and fecha_correspondencia <= @f2",
                    new SqlParameter("@t", tipo),
                    new SqlParameter("@n", no_doc),
                    new SqlParameter("@f1", FechaInicial),
                    new SqlParameter("@f2", FechaFinal)).FirstOrDefault();
                    if (IdReg > 0)
                    {
                        Session["Message"] = "Error: El No. de archivo " + no_doc +  " del tipo " + tipo +
                            " ya esta registrado en el año " + Anio + " con el Id = " + IdReg;
                        return RedirectToAction("Index", "Home");
                    }

                   
                    var RegDoc = new registro_correspondencia();
                    string NombreArchivo = null;

                    if (archivo_pdf != null)
                    {
                        NombreArchivo = archivo_pdf.FileName;
                    }

                    var idUsuario = Session["IdUsuario"] as string;
                    RegDoc.tipo_correspondencia = tipo;
                    RegDoc.numero_correspondencia = no_doc;
                    RegDoc.numero_remision = no_rem;
                    RegDoc.persona = int.Parse(select_remitente);
                    RegDoc.institucion = int.Parse(select_institucion);
                    RegDoc.codigo_acp_serie = int.Parse(select_serie);
                    if (!string.IsNullOrEmpty(select_sub_serie))
                    {
                        RegDoc.numero_acp_subserie = int.Parse(select_sub_serie);
                    }
                    RegDoc.fecha_correspondencia = DateTime.Parse(fecha_ing);
                    RegDoc.titulo_asunto = asunto;
                    RegDoc.ruta_archivo = NombreArchivo;
                    RegDoc.id_departamento = int.Parse(select_departamento);
                    RegDoc.id_usuario = int.Parse(idUsuario);

                    DB.registro_correspondencia.Add(RegDoc);
                    DB.SaveChanges();

                    if (archivo_pdf != null)
                    {
                        string path = Server.MapPath("~/archivos_pdf/" + archivo_pdf.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        archivo_pdf.SaveAs(path);
                    }

                    Session["Message"] = "Registro Grabado";
                    return RedirectToAction("Index", "Home");

                };

            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = ""; Session["MessageConsulta"] = ""; Session["MessageRemi"] = ""; Session["MessageSubserie"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    List<departamentos> ListaDep = new List<departamentos>();
                    List<Registros> ListaRegistros = new List<Registros>();
                    List<serie> ListaSeries = new List<serie>();
                    List<subserie> ListaSubSeries = new List<subserie>();
                    string StrSql = "";

                    StrSql = "SELECT TOP 25 r.reg_cor_id, r.tipo_correspondencia, r.numero_correspondencia,"
                    + "r.fecha_correspondencia, p.per_id, p.nombre as remitente, i.cre_id, i.nombre_institucional as institucion,"
                    + "r.numero_remision, r.titulo_asunto, r.codigo_acp_serie, s.titulo as nombre_serie, r.numero_acp_subserie,"
                    + "ss.titulo as nombre_subserie, r.id_departamento, d.nombre as nombre_dep, r.ruta_archivo, r.id_usuario"
                    + " FROM registro_correspondencia r"
                    + " LEFT JOIN persona p ON r.PERSONA = p.PER_ID"
                    + " LEFT JOIN institucional i ON r.INSTITUCION = i.CRE_ID"
                    + " LEFT JOIN serie s ON r.CODIGO_ACP_SERIE = s.SER_ID"
                    + " JOIN departamentos d ON r.ID_DEPARTAMENTO = d.id"
                    + " LEFT JOIN subserie ss on r.NUMERO_ACP_SUBSERIE = ss.SUB_ID"
                    + " WHERE r.reg_cor_id =" + id;
                    var ListaSeleccion = DB.Database.SqlQuery<Seleccion>(StrSql).ToList();
                    ViewBag.seleccion = ListaSeleccion;

                    StrSql = "SELECT TOP 15 r.reg_cor_id, r.tipo_correspondencia, r.numero_correspondencia,"
                    + "r.fecha_correspondencia, i.nombre_institucional as institucion, p.nombre as persona, r.numero_remision,"
                    + "r.titulo_asunto FROM registro_correspondencia r"
                    + " LEFT JOIN persona p ON r.persona = p.per_id"
                    + " LEFT JOIN institucional i ON r.institucion = i.cre_id"
                    + " order by r.reg_cor_id desc";
                    ListaRegistros = DB.Database.SqlQuery<Registros>(StrSql).ToList();
                    ViewBag.documentos = ListaRegistros;

                    StrSql = "SELECT ser_id, titulo, siglas FROM SERIE ORDER BY TITULO ASC";
                    ListaSeries = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.series = ListaSeries;

                    if (ListaSeleccion[0].numero_acp_subserie != null)
                    {
                        StrSql = "SELECT * FROM SUBSERIE WHERE SERIE =" + ListaSeleccion[0].codigo_acp_serie + " ORDER BY TITULO ASC";
                        ListaSubSeries = DB.Database.SqlQuery<subserie>(StrSql).ToList();
                        ViewBag.SubSeries = ListaSubSeries;
                    }
                    else
                    {
                        ViewBag.SubSeries = null;
                    }

                    ViewBag.nivel = Session["Nivel"] as string;
                    ListaDep = DB.departamentos.ToList();
                    ViewBag.departamentos = ListaDep;

                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string tipo, string no_doc, string select_remitente, string no_rem,
            string select_institucion, string select_serie, string select_sub_serie, string fecha_ing,
            string select_departamento, string asunto, HttpPostedFileBase archivo_pdf, string id_documento, string archivo_anterior)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = "";

                    string NombreArchivo = null;
                    if (archivo_pdf != null)
                    {
                        NombreArchivo = archivo_pdf.FileName;
                    }

                    int idReg = int.Parse(id_documento);

                    var RegCorresp = DB.registro_correspondencia
                    .Where(b => b.REG_COR_ID == idReg)
                    .FirstOrDefault();

                    RegCorresp.tipo_correspondencia = tipo;
                    RegCorresp.numero_correspondencia = no_doc;
                    RegCorresp.numero_remision = no_rem;
                    RegCorresp.persona = int.Parse(select_remitente);
                    RegCorresp.institucion = int.Parse(select_institucion);
                    RegCorresp.codigo_acp_serie = int.Parse(select_serie);
                    if (!string.IsNullOrEmpty(select_sub_serie))
                    {
                        RegCorresp.numero_acp_subserie = int.Parse(select_sub_serie);
                    }
                    RegCorresp.fecha_correspondencia = DateTime.Parse(fecha_ing);
                    RegCorresp.titulo_asunto = asunto;
                    RegCorresp.ruta_archivo = NombreArchivo;
                    RegCorresp.id_departamento = int.Parse(select_departamento);

                    DB.SaveChanges();
                    Session["Message"] = "Registro actualizado correctamente";

                    if (archivo_pdf != null)
                    {
                        string path = "";
                        if (archivo_anterior != null)
                        {
                            path = Server.MapPath("~/archivos_pdf/" + archivo_anterior);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                        }

                        path = Server.MapPath("~/archivos_pdf/" + archivo_pdf.FileName);
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        archivo_pdf.SaveAs(path);
                    }

                    return RedirectToAction("Index", "Home");
                };

            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = ""; Session["MessageConsulta"] = ""; Session["MessageRemi"] = ""; Session["MessageSubserie"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM registro_correspondencia WHERE REG_COR_ID =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["Message"] = "Registro eliminado";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Session["Message"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        //**************************************************************************************************************
        //Para llenar el select de remitentes con AJAX
        [HttpPost]
        public ActionResult Remitente(string valor)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = "";
                    valor = valor + "%";
                    var remitentes = DB.Database.SqlQuery<Remitentes>("SELECT TOP 50 per_id, nombre FROM PERSONA" +
                    " WHERE NOMBRE LIKE @p1 order by nombre asc", new SqlParameter("@p1", valor)).ToList();
                    if (remitentes.Count() > 0)
                    {
                        return Json(remitentes, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return Json("Missing data!", JsonRequestBehavior.AllowGet);
            }
        }

        //Para llenar el select de instituciones con AJAX
        [HttpPost]
        public ActionResult Institucion(string valor)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = "";
                    valor = valor + "%";
                    var instituciones = DB.Database.SqlQuery<Instituciones>("SELECT TOP 250  cre_id, nombre_institucional FROM institucional" +
                    " WHERE nombre_institucional LIKE @p1 order by nombre_institucional asc", new SqlParameter("@p1", valor)).ToList();
                    if (instituciones.Count() > 0)
                    {
                        return Json(instituciones, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("", JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return Json("Missing data!", JsonRequestBehavior.AllowGet);
            }

        }

        //Para llenar el select de subseries con AJAX
        [HttpPost]
        public ActionResult Serie(string valor)
        {
            try
            {
                Session["Message"] = "";
                var series = DB.Database.SqlQuery<subserie>("SELECT * FROM SUBSERIE" +
                " WHERE serie = @p1 order by titulo asc", new SqlParameter("@p1", valor)).ToList();
                if (series.Count() > 0)
                {
                    return Json(series, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(valor, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return Json("Missing data!", JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult grabar_remitente_modal(string remitente, string rol)
        {
            try
            {
                var pers = new persona();
                Session["Message"] = "";

                pers.nombre = remitente;
                pers.rol = rol;

                DB.persona.Add(pers);
                DB.SaveChanges();
                return Json("Ok", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return Json("Missing data!", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult grabar_institucion_modal(string institucion, string siglas, string direccion, string email, string telefono)
        {
            try
            {
                Session["Message"] = "";
                var inst = new institucional();

                inst.nombre_institucional = institucion;
                inst.sigla = siglas;
                inst.direccion = direccion;
                inst.correo = email;
                inst.telefono = telefono;

                DB.institucional.Add(inst);
                DB.SaveChanges();
                return Json("Ok", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Session["Message"] = "Error: " + ex.Message;
                return Json("Missing data!", JsonRequestBehavior.AllowGet);
            }
        }

        //**************************************************************************************************************

        public ActionResult Salir()
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["Message"] = ""; Session["MessageConsulta"] = ""; Session["MessageRemi"] = ""; Session["MessageSubserie"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";
                    Session["query"] = "";

                    Session["Login"] = "false";
                    return RedirectToAction("Login", "Home");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "ERROR: " + ex.Message;
                return RedirectToAction("Login", "Home");
            }
        }

        //Reporte de Comunicaciones******************************************************************************************************

        public ActionResult ReporteComunicaciones()
        {                                 
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }

                if (Session["query2"] as string == "")
                {
                    Session["MessageConsulta"] = "Error: No hay comunicaciones para mostrar en el reporte (Haga una consulta que traiga registros primero)";
                    return RedirectToAction("Consulta", "Query");
                }

                string Criterio = Session["Criterio"] as string;

                ReportDocument Rpt = new ReportDocument();
               
                string StrSql = Session["query2"] as string;
                var ListaSeleccion = DB.Database.SqlQuery<Seleccion2>(StrSql).ToList();

                Rpt.Load(Path.Combine(Server.MapPath("/Reportes"), "Reporte_Comunicaciones.rpt"));

                //Envio de datos a las formulas del reporte *****************************
                Rpt.DataDefinition.FormulaFields["criterio"].Text = "'" + Criterio + "'";

                Rpt.SetDataSource(ListaSeleccion);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = Rpt.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Reporte_Comunicaciones.pdf");
                
                //Para generar el reporte en excel**********************************************
                //    Stream stream = Rpt.ExportToStream(ExportFormatType.Excel);
                //    stream.Seek(0, SeekOrigin.Begin);
                //    return File(stream, "application / excel", "ReporteDocumentos.xls");
                //*******************************************************************************               
            }
            catch (Exception ex)
            {
                Session["MessageConsulta"] = "Error: " + ex.Message;
                return RedirectToAction("Consulta", "Query");
            }

        }

        //***********************************************************************************************************************************

        [HttpPost]
        public ActionResult MantenerSeccionActiva(string valor)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return Json("LoginFalse", JsonRequestBehavior.AllowGet);
                }

                return Json("Ok", JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ViewBag.Error = "ERROR: " + ex.Message;
                return Json("Missing data!", JsonRequestBehavior.AllowGet);
            }
        }

    }
}