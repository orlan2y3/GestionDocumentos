using Sistema_Gestion_de_Documentos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sistema_Gestion_de_Documentos.Controllers
{
    public class UpkeepController : Controller
    {

        ApplicationDbContext DB = new ApplicationDbContext();

        public ActionResult Remitente()
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {

                    Session["Message"] = ""; Session["MessageConsulta"] = ""; Session["MessageSubserie"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    string StrSql = "SELECT * FROM PERSONA ORDER BY NOMBRE ASC";
                    var ListaRemitentes = DB.Database.SqlQuery<persona>(StrSql).ToList();
                    ViewBag.registros = ListaRemitentes;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageRemi"] as string))
                    {
                        ViewBag.msgRemi = Session["MessageRemi"] as string;
                    }
                    else
                    {
                        ViewBag.msgRemi = null;
                    }

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageRemi"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRemitente(string remitente, string rol, string telefono)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageRemi"] = "";

                    int idReg = DB.Database.SqlQuery<int>("Select per_id from persona" +
                    " where nombre = @p1", new SqlParameter("@p1", remitente)).FirstOrDefault();
                    if (idReg > 0)
                    {
                        Session["MessageRemi"] = "Error: Ese Remitente ya esta registrado";
                        return RedirectToAction("Remitente", "Upkeep");
                    }

                    var RegRemitente = new persona();

                    RegRemitente.nombre = remitente;
                    RegRemitente.rol = rol;
                    RegRemitente.telefono = telefono;

                    DB.persona.Add(RegRemitente);
                    DB.SaveChanges();

                    Session["MessageRemi"] = "Remitente Grabado";
                    return RedirectToAction("Remitente", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageRemi"] = "Error: " + ex.Message;
                return RedirectToAction("Remitente", "Upkeep");
            }
        }


        public ActionResult EditRemitente(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = ""; Session["MessageRemi"] = "";

                    StrSql = "SELECT * FROM PERSONA WHERE PER_ID =" + id;
                    var Registro = DB.Database.SqlQuery<persona>(StrSql).ToList();
                    ViewBag.seleccion = Registro;

                    StrSql = "SELECT * FROM PERSONA ORDER BY NOMBRE ASC";
                    var ListaRemitentes = DB.Database.SqlQuery<persona>(StrSql).ToList();
                    ViewBag.registros = ListaRemitentes;

                    return View("Remitente");
                }

            }
            catch (Exception ex)
            {
                Session["MessageRemi"] = "Error: " + ex.Message;
                return RedirectToAction("Remitente", "Upkeep");
            }
        }


        public ActionResult UpdateRemitente(string remitente, string rol, string telefono, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageRemi"] = "";
                    int idReg = int.Parse(id_registro);

                    var Reg = DB.persona
                    .Where(b => b.per_id == idReg)
                    .FirstOrDefault();

                    Reg.nombre = remitente;
                    Reg.rol = rol;
                    Reg.telefono = telefono;

                    DB.SaveChanges();
                    Session["MessageRemi"] = "Registro actualizado correctamente";

                    return RedirectToAction("Remitente", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageRemi"] = "Error: " + ex.Message;
                return RedirectToAction("Remitente", "Upkeep");
            }
        }

        public ActionResult DeleteRemitente(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageRemi"] = "";

                    int Reg = DB.Database.SqlQuery<int>("SELECT reg_cor_id FROM REGISTRO_CORRESPONDENCIA WHERE persona =@p1",
                        new SqlParameter("@p1", id)).FirstOrDefault();

                    if(Reg > 0)
                    {
                        Session["MessageRemi"] = "Error: Hay correspondencias registradas con ese remitente (No se puede eliminar)";
                        return RedirectToAction("Remitente", "Upkeep");
                    }

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM persona WHERE per_id =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageRemi"] = "Registro eliminado";
                        return RedirectToAction("Remitente", "Upkeep");
                    }
                    else
                    {
                        Session["MessageRemi"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Remitente", "Upkeep");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MessageRemi"] = "Error: " + ex.Message;
                return RedirectToAction("Remitente", "Upkeep");

            }
        }

        //**************************************************************************************************************************
        //Actions Institucion *******************************************************************************************************

        public ActionResult Institucion()
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
                    Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    string StrSql = "SELECT * FROM INSTITUCIONAL ORDER BY NOMBRE_INSTITUCIONAL ASC";
                    var ListaInstituciones = DB.Database.SqlQuery<institucional>(StrSql).ToList();
                    ViewBag.registros = ListaInstituciones;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageInst"] as string))
                    {
                        ViewBag.msgInst = Session["MessageInst"] as string;
                    }
                    else
                    {
                        ViewBag.msgInst = null;
                    }

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageInst"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_Institucion(string nombre, string siglas, string direccion, string email, string telefono, string fax, string celular)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageInst"] = "";

                    int idReg = DB.Database.SqlQuery<int>("Select cre_id from institucional" +
                    " where nombre_institucional = @p1", new SqlParameter("@p1", nombre)).FirstOrDefault();
                    if (idReg > 0)
                    {
                        Session["MessageInst"] = "Error: Esa Institución ya esta registrada";
                        return RedirectToAction("Institucion", "Upkeep");
                    }

                    var RegInstitucion = new institucional();

                    RegInstitucion.nombre_institucional = nombre;
                    RegInstitucion.sigla = siglas;
                    RegInstitucion.direccion = direccion;
                    RegInstitucion.correo = email;
                    RegInstitucion.telefono = telefono;
                    RegInstitucion.fax = fax;
                    RegInstitucion.movil = celular;

                    DB.institucional.Add(RegInstitucion);
                    DB.SaveChanges();

                    Session["MessageInst"] = "Institución Grabada";
                    return RedirectToAction("Institucion", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageInst"] = "Error: " + ex.Message;
                return RedirectToAction("Institucion", "Upkeep");
            }
        }


        public ActionResult Edit_Institucion(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = ""; Session["MessageInst"] = "";

                    StrSql = "SELECT * FROM INSTITUCIONAL WHERE CRE_ID =" + id;
                    var Registro = DB.Database.SqlQuery<institucional>(StrSql).ToList();
                    ViewBag.seleccion = Registro;

                    StrSql = "SELECT * FROM INSTITUCIONAL ORDER BY NOMBRE_INSTITUCIONAL ASC";
                    var ListaRemitentes = DB.Database.SqlQuery<institucional>(StrSql).ToList();
                    ViewBag.registros = ListaRemitentes;

                    return View("Institucion");
                }

            }
            catch (Exception ex)
            {
                Session["MessageInst"] = "Error: " + ex.Message;
                return RedirectToAction("Institucion", "Upkeep");
            }
        }


        public ActionResult Update_Institucion(string nombre, string siglas, string direccion, string email, string telefono, string fax, string celular, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageInst"] = "";
                    int idReg = int.Parse(id_registro);

                    var Reg = DB.institucional
                    .Where(b => b.cre_id == idReg)
                    .FirstOrDefault();

                    Reg.nombre_institucional = nombre;
                    Reg.sigla = siglas;
                    Reg.direccion = direccion;
                    Reg.correo = email;
                    Reg.telefono = telefono;
                    Reg.fax = fax;
                    Reg.movil = celular;

                    DB.SaveChanges();
                    Session["MessageInst"] = "Registro actualizado correctamente";

                    return RedirectToAction("Institucion", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageInst"] = "Error: " + ex.Message;
                return RedirectToAction("Institucion", "Upkeep");
            }
        }

        public ActionResult Delete_Institucion(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageInst"] = "";

                    int Reg = DB.Database.SqlQuery<int>("SELECT reg_cor_id FROM REGISTRO_CORRESPONDENCIA WHERE institucion =@p1",
                        new SqlParameter("@p1", id)).FirstOrDefault();

                    if (Reg > 0)
                    {
                        Session["MessageInst"] = "Error: Hay correspondencias registradas con esa institución (No se puede eliminar)";
                        return RedirectToAction("Institucion", "Upkeep");
                    }

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM institucional WHERE cre_id =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageInst"] = "Registro eliminado";
                        return RedirectToAction("Institucion", "Upkeep");
                    }
                    else
                    {
                        Session["MessageInst"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Institucion", "Upkeep");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MessageInst"] = "Error: " + ex.Message;
                return RedirectToAction("Institucion", "Upkeep");

            }
        }

        //**************************************************************************************************************************

        //Actions Departamentos**************************************************************************************************

        public ActionResult Departamento()
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
                    Session["MessageInst"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    string StrSql = "SELECT * FROM DEPARTAMENTOS ORDER BY NOMBRE ASC";
                    var ListaDepartamentos = DB.Database.SqlQuery<departamentos>(StrSql).ToList();
                    ViewBag.registros = ListaDepartamentos;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageDep"] as string))
                    {
                        ViewBag.msgDep = Session["MessageDep"] as string;
                    }
                    else
                    {
                        ViewBag.msgDep = null;
                    }

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageDep"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_Departamento(string nombre)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageDep"] = "";

                    int idReg = DB.Database.SqlQuery<int>("Select id from departamentos" +
                    " where nombre = @p1", new SqlParameter("@p1", nombre)).FirstOrDefault();
                    if (idReg > 0)
                    {
                        Session["MessageDep"] = "Error: Ese Departamento ya esta registrado";
                        return RedirectToAction("Departamento", "Upkeep");
                    }

                    var RegDep = new departamentos();

                    RegDep.nombre = nombre;
                    
                    DB.departamentos.Add(RegDep);
                    DB.SaveChanges();

                    Session["MessageDep"] = "Departamento Grabado";
                    return RedirectToAction("Departamento", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageDep"] = "Error: " + ex.Message;
                return RedirectToAction("Departamento", "Upkeep");
            }
        }


        public ActionResult Edit_Departamento(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = ""; Session["MessageDep"] = "";

                    StrSql = "SELECT * FROM DEPARTAMENTOS WHERE ID =" + id;
                    var Registro = DB.Database.SqlQuery<departamentos>(StrSql).ToList();
                    ViewBag.seleccion = Registro;

                    StrSql = "SELECT * FROM DEPARTAMENTOS ORDER BY NOMBRE ASC";
                    var ListaDep = DB.Database.SqlQuery<departamentos>(StrSql).ToList();
                    ViewBag.registros = ListaDep;

                    return View("Departamento");
                }

            }
            catch (Exception ex)
            {
                Session["MessageDep"] = "Error: " + ex.Message;
                return RedirectToAction("Departamento", "Upkeep");
            }
        }


        public ActionResult Update_Departamento(string nombre, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageDep"] = "";
                    int idReg = int.Parse(id_registro);

                    var Reg = DB.departamentos
                    .Where(b => b.id == idReg)
                    .FirstOrDefault();

                    Reg.nombre = nombre;

                    DB.SaveChanges();
                    Session["MessageDep"] = "Registro actualizado correctamente";

                    return RedirectToAction("Departamento", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageDep"] = "Error: " + ex.Message;
                return RedirectToAction("Departamento", "Upkeep");
            }
        }

        public ActionResult Delete_Departamento(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageDep"] = "";

                    int Reg = DB.Database.SqlQuery<int>("SELECT reg_cor_id FROM REGISTRO_CORRESPONDENCIA WHERE id_departamento =@p1",
                        new SqlParameter("@p1", id)).FirstOrDefault();

                    if (Reg > 0)
                    {
                        Session["MessageDep"] = "Error: Hay correspondencias registradas con ese departamento (No se puede eliminar)";
                        return RedirectToAction("Departamento", "Upkeep");
                    }

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM DEPARTAMENTOS WHERE id =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageDep"] = "Registro eliminado";
                        return RedirectToAction("Departamento", "Upkeep");
                    }
                    else
                    {
                        Session["MessageDep"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Departamento", "Upkeep");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MessageDep"] = "Error: " + ex.Message;
                return RedirectToAction("Departamento", "Upkeep");

            }
        }

        //**************************************************************************************************************************

        //Actions Series**************************************************************************************************

        public ActionResult Serie()
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
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageUsuario"] = "";

                    string StrSql = "SELECT * FROM SERIE ORDER BY TITULO ASC";
                    var ListaSeries = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.registros = ListaSeries;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageSerie"] as string))
                    {
                        ViewBag.msgSerie = Session["MessageSerie"] as string;
                    }
                    else
                    {
                        ViewBag.msgSerie = null;
                    }

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageSerie"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_Serie(string nombre, string siglas)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageSerie"] = "";

                    int idReg = DB.Database.SqlQuery<int>("Select ser_id from serie" +
                    " where titulo = @p1", new SqlParameter("@p1", nombre)).FirstOrDefault();
                    if (idReg > 0)
                    {
                        Session["MessageSerie"] = "Error: Esa Serie ya esta registrada";
                        return RedirectToAction("Serie", "Upkeep");
                    }

                    var RegSerie = new serie();

                    RegSerie.titulo = nombre;
                    RegSerie.siglas = siglas;

                    DB.serie.Add(RegSerie);
                    DB.SaveChanges();

                    Session["MessageSerie"] = "Serie Grabada";
                    return RedirectToAction("Serie", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageSerie"] = "Error: " + ex.Message;
                return RedirectToAction("Serie", "Upkeep");
            }
        }


        public ActionResult Edit_Serie(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = ""; Session["MessageSerie"] = "";

                    StrSql = "SELECT * FROM SERIE WHERE SER_ID =" + id;
                    var Registro = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.seleccion = Registro;

                    StrSql = "SELECT * FROM SERIE ORDER BY TITULO ASC";
                    var ListaSerie = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.registros = ListaSerie;

                    return View("Serie");
                }

            }
            catch (Exception ex)
            {
                Session["MessageSerie"] = "Error: " + ex.Message;
                return RedirectToAction("Serie", "Upkeep");
            }
        }


        public ActionResult Update_Serie(string nombre, string siglas, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageSerie"] = "";
                    int idReg = int.Parse(id_registro);

                    var Reg = DB.serie
                    .Where(b => b.ser_id == idReg)
                    .FirstOrDefault();

                    Reg.titulo = nombre;
                    Reg.siglas = siglas;

                    DB.SaveChanges();
                    Session["MessageSerie"] = "Registro actualizado correctamente";

                    return RedirectToAction("Serie", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageSerie"] = "Error: " + ex.Message;
                return RedirectToAction("Serie", "Upkeep");
            }
        }

        public ActionResult Delete_Serie(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageSerie"] = "";

                    int Reg = DB.Database.SqlQuery<int>("SELECT reg_cor_id FROM REGISTRO_CORRESPONDENCIA WHERE codigo_acp_serie =@p1",
                        new SqlParameter("@p1", id)).FirstOrDefault();

                    if (Reg > 0)
                    {
                        Session["MessageSerie"] = "Error: Hay correspondencias registradas con esa Serie (No se puede eliminar)";
                        return RedirectToAction("Serie", "Upkeep");
                    }

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM SERIE WHERE ser_id =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageSerie"] = "Registro eliminado";
                        return RedirectToAction("Serie", "Upkeep");
                    }
                    else
                    {
                        Session["MessageSerie"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Serie", "Upkeep");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MessageSerie"] = "Error: " + ex.Message;
                return RedirectToAction("Serie", "Upkeep");

            }
        }

        //**************************************************************************************************************************

        //Actions Sub-Series**************************************************************************************************

        public ActionResult Subserie()
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = "";
                    Session["Message"] = ""; Session["MessageConsulta"] = ""; Session["MessageRemi"] = "";
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = ""; Session["MessageUsuario"] = "";

                    StrSql = "SELECT SS.SUB_ID, SS.SERIE, SS.TITULO, SS.SIGLA, S.TITULO AS NOMBRE_SERIE" +
                    " FROM SUBSERIE SS LEFT JOIN SERIE S ON SS.SERIE = S.SER_ID ORDER BY S.TITULO, SS.TITULO ASC";
                    var ListaSubseries = DB.Database.SqlQuery<subserie2>(StrSql).ToList();
                    ViewBag.registros = ListaSubseries;

                    StrSql = "SELECT ser_id, titulo, siglas FROM SERIE ORDER BY TITULO ASC";
                    var ListaSeries = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.series = ListaSeries;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageSubserie"] as string))
                    {
                        ViewBag.msgSubserie = Session["MessageSubserie"] as string;
                    }
                    else
                    {
                        ViewBag.msgSubserie = null;
                    }

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageSubserie"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_Subserie(string select_serie, string nombre, string siglas)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageSubserie"] = "";
                    int idSerie = int.Parse(select_serie);

                    int idReg = DB.Database.SqlQuery<int>("Select sub_id from subserie" +
                    " where titulo = @p1 and serie = @p2", new SqlParameter("@p1", nombre), new SqlParameter("@p2", idSerie)).FirstOrDefault();
                    if (idReg > 0)
                    {
                        Session["MessageSubserie"] = "Error: Esa Sub-serie ya esta registrada en esa serie";
                        return RedirectToAction("Subserie", "Upkeep");
                    }

                    var RegSubserie = new subserie();

                    RegSubserie.serie = idSerie;
                    RegSubserie.titulo = nombre;
                    RegSubserie.sigla = siglas;

                    DB.subserie.Add(RegSubserie);
                    DB.SaveChanges();

                    Session["MessageSerie"] = "Sub-serie Grabada";
                    return RedirectToAction("Subserie", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageSubserie"] = "Error: " + ex.Message;
                return RedirectToAction("Subserie", "Upkeep");
            }
        }


        public ActionResult Edit_Subserie(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = ""; Session["MessageSubserie"] = "";

                    StrSql = "SELECT * FROM SUBSERIE WHERE SUB_ID =" + id;
                    var IdSerie = DB.Database.SqlQuery<subserie>(StrSql).ToList();

                    if (IdSerie[0].serie ==null)
                    {
                        StrSql = "SELECT * FROM SUBSERIE WHERE SUB_ID =" + id;
                        var Registro = DB.Database.SqlQuery<subserie>(StrSql).ToList();
                        ViewBag.seleccion = Registro;
                    }
                    else
                    {
                        StrSql = "SELECT SS.SUB_ID, SS.SERIE, SS.TITULO, SS.SIGLA, S.TITULO AS NOMBRE_SERIE" +
                        " FROM SUBSERIE SS, SERIE S WHERE SS.SUB_ID =" + id + " AND SS.SERIE = S.SER_ID";
                        var Registro = DB.Database.SqlQuery<subserie2>(StrSql).ToList();
                        ViewBag.seleccion = Registro;
                    }

                    StrSql = "SELECT ser_id, titulo, siglas FROM SERIE ORDER BY TITULO ASC";
                    var ListaSeries = DB.Database.SqlQuery<serie>(StrSql).ToList();
                    ViewBag.series = ListaSeries;

                    StrSql = "SELECT SS.SUB_ID, SS.SERIE, SS.TITULO, SS.SIGLA, S.TITULO AS NOMBRE_SERIE" +
                    " FROM SUBSERIE SS LEFT JOIN SERIE S ON SS.SERIE = S.SER_ID ORDER BY S.TITULO, SS.TITULO ASC";
                    var ListaSubSerie = DB.Database.SqlQuery<subserie2>(StrSql).ToList();
                    ViewBag.registros = ListaSubSerie;

                    return View("Subserie");
                }

            }
            catch (Exception ex)
            {
                Session["MessageSubserie"] = "Error: " + ex.Message;
                return RedirectToAction("Subserie", "Upkeep");
            }
        }


        public ActionResult Update_Subserie(string select_serie, string nombre, string siglas, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageSubserie"] = "";

                    int idSerie = 0;

                    if (select_serie != "")
                    {
                        idSerie = int.Parse(select_serie);
                    }                    
                    
                    int idReg = int.Parse(id_registro);                    

                    var Reg = DB.subserie
                    .Where(b => b.sub_id == idReg)
                    .FirstOrDefault();

                    if (select_serie != "")
                    {
                        Reg.serie = idSerie;
                    }
                    Reg.titulo = nombre;
                    Reg.sigla = siglas;

                    DB.SaveChanges();
                    Session["MessageSubserie"] = "Registro actualizado correctamente";

                    return RedirectToAction("Subserie", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageSubserie"] = "Error: " + ex.Message;
                return RedirectToAction("Subserie", "Upkeep");
            }
        }

        public ActionResult Delete_Subserie(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageSubserie"] = "";

                    int Reg = DB.Database.SqlQuery<int>("SELECT reg_cor_id FROM REGISTRO_CORRESPONDENCIA WHERE numero_acp_subserie =@p1",
                        new SqlParameter("@p1", id)).FirstOrDefault();

                    if (Reg > 0)
                    {
                        Session["MessageSubserie"] = "Error: Hay correspondencias registradas con esa Sub-Serie (No se puede eliminar)";
                        return RedirectToAction("Subserie", "Upkeep");
                    }

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM SUBSERIE WHERE sub_id =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageSubserie"] = "Registro eliminado";
                        return RedirectToAction("Subserie", "Upkeep");
                    }
                    else
                    {
                        Session["MessageSubserie"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Subserie", "Upkeep");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MessageSubserie"] = "Error: " + ex.Message;
                return RedirectToAction("Subserie", "Upkeep");

            }
        }


        //**************************************************************************************************************************

        //Actions Usuarios**************************************************************************************************

        public ActionResult Usuario()
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
                    Session["MessageInst"] = ""; Session["MessageDep"] = ""; Session["MessageSerie"] = "";

                    string StrSql = "SELECT * FROM USUARIOS ORDER BY NOMBRE_COMPLETO ASC";
                    var ListaSeries = DB.Database.SqlQuery<usuarios>(StrSql).ToList();
                    ViewBag.registros = ListaSeries;

                    //Para presentar en la vista Index, los mensajes devueltos por los actions
                    if (!string.IsNullOrEmpty(Session["MessageUsuario"] as string))
                    {
                        ViewBag.msgUsuario = Session["MessageUsuario"] as string;
                    }
                    else
                    {
                        ViewBag.msgUsuario = null;
                    }

                    return View();
                }

            }
            catch (Exception ex)
            {
                Session["MessageUsuario"] = "Error: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save_Usuario(string usuario, string clave, string nombre, string email, int nivel, int estado)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageUsuario"] = "";

                    int idReg = DB.Database.SqlQuery<int>("Select idusuario from usuarios" +
                    " where usuario = @p1", new SqlParameter("@p1", nombre)).FirstOrDefault();
                    if (idReg > 0)
                    {
                        Session["MessageUsuario"] = "Error: Ese Usuario ya esta registrado";
                        return RedirectToAction("Usuario", "Upkeep");
                    }

                    var RegUsuario = new usuarios();

                    RegUsuario.usuario = usuario;
                    RegUsuario.clave = clave;
                    RegUsuario.nombre_completo = nombre;
                    RegUsuario.nivel = nivel;
                    RegUsuario.activo = estado;


                    DB.usuarios.Add(RegUsuario);
                    DB.SaveChanges();

                    Session["MessageUsuario"] = "Usuario Grabado";
                    return RedirectToAction("Usuario", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageUsuario"] = "Error: " + ex.Message;
                return RedirectToAction("Usuario", "Upkeep");
            }
        }


        public ActionResult Edit_Usuario(int? id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    string StrSql = ""; Session["MessageUsuario"] = "";

                    StrSql = "SELECT * FROM USUARIOS WHERE IDUSUARIO =" + id;
                    var Registro = DB.Database.SqlQuery<usuarios>(StrSql).ToList();
                    ViewBag.seleccion = Registro;

                    StrSql = "SELECT * FROM USUARIOS ORDER BY NOMBRE_COMPLETO ASC";
                    var ListaSerie = DB.Database.SqlQuery<usuarios>(StrSql).ToList();
                    ViewBag.registros = ListaSerie;

                    return View("Usuario");
                }

            }
            catch (Exception ex)
            {
                Session["MessageUsuario"] = "Error: " + ex.Message;
                return RedirectToAction("Usuario", "Upkeep");
            }
        }


        public ActionResult Update_Usuario(string usuario, string clave, string nombre, string email, int select_nivel, int select_estado, string id_registro)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageUsuario"] = "";
                    int idReg = int.Parse(id_registro);

                    var Reg = DB.usuarios
                    .Where(b => b.idusuario== idReg)
                    .FirstOrDefault();

                    Reg.usuario = usuario;
                    Reg.clave = clave;
                    Reg.nombre_completo = nombre;
                    Reg.nivel = select_nivel;
                    Reg.activo = select_estado;

                    DB.SaveChanges();
                    Session["MessageUsuario"] = "Registro actualizado correctamente";

                    return RedirectToAction("Usuario", "Upkeep");
                }

            }
            catch (Exception ex)
            {
                Session["MessageUsuario"] = "Error: " + ex.Message;
                return RedirectToAction("Usuario", "Upkeep");
            }
        }

        public ActionResult Delete_Usuario(int id)
        {
            try
            {
                if (Session["Login"] as string == null || Session["Login"] as string == "false")
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    Session["MessageUsuario"] = "";

                    int Reg = DB.Database.SqlQuery<int>("SELECT reg_cor_id FROM REGISTRO_CORRESPONDENCIA WHERE id_usuario =@p1",
                        new SqlParameter("@p1", id)).FirstOrDefault();

                    if (Reg > 0)
                    {
                        Session["MessageUsuario"] = "Error: Hay correspondencias registradas con ese usuario (No se puede eliminar)";
                        return RedirectToAction("Usuario", "Upkeep");
                    }

                    int Resul = DB.Database.ExecuteSqlCommand("DELETE FROM USUARIOS WHERE IDUSUARIO =" + id);
                    if (Resul > 0)
                    {
                        DB.SaveChanges();
                        Session["MessageUsuario"] = "Registro eliminado";
                        return RedirectToAction("Usuario", "Upkeep");
                    }
                    else
                    {
                        Session["MessageUsuario"] = "Error: No fue posible eliminar el registro solicitado";
                        return RedirectToAction("Usuario", "Upkeep");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["MessageUsuario"] = "Error: " + ex.Message;
                return RedirectToAction("Usuario", "Upkeep");

            }
        }


    }
}