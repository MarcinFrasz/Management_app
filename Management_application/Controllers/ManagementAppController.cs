using Management_application.Models;
using Management_application.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Rotativa.MVC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Management_application.Controllers
{
    public class ManagementAppController : Controller
    {
        private readonly Repository repository;
        private readonly ControllerServices services;
        public ManagementAppController(Repository Repository)
        {
            this.repository = Repository;
            services = new ControllerServices(Repository);
        }

        #region AssignRoleView [GET]
        public ActionResult AssignRoleView(AssignRoleView classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("admin"))
                {
                    var role_list = repository.Select_Roles();
                    ViewBag.role_list = role_list;
                    classes.Users_list = repository.Select_Users_with_Roles();
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region AssignRoleView [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRoleView(AssignRoleView classes, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("admin"))
                {
                    var role_list = repository.Select_Roles();
                    ViewBag.role_list = role_list;
                    switch (submit)
                    {
                        case "Edytuj":
                            classes.Users_list = repository.Select_Users_with_Roles();
                            break;
                        case "Szukaj":
                            classes.Users_list = repository.Select_Users_with_Roles();
                            switch (classes.Sort_column)
                            {
                                case "Imię":
                                    classes.Users_list = classes.Users_list.Where(i => i.Name == classes.Search_text).ToList();
                                    break;
                                case "Nazwisko":
                                    classes.Users_list = classes.Users_list.Where(i => i.Surname == classes.Search_text).ToList();
                                    break;
                                case "Email":
                                    classes.Users_list = classes.Users_list.Where(i => i.Email == classes.Search_text).ToList();
                                    break;
                                case "Indeks":
                                    classes.Users_list = classes.Users_list.Where(i => i.Index_number == classes.Search_text).ToList();
                                    break;
                                case "Rola":
                                    classes.Users_list = classes.Users_list.Where(i => i.Role_name == classes.Search_text).ToList();
                                    break;
                            }
                            break;
                        case "Sortuj":
                            classes.Users_list = repository.Select_Users_with_Roles();
                            switch (classes.Sort_column)
                            {
                                case "Imię":
                                    if (classes.Asc_Desc == "ASC")
                                    {
                                        classes.Users_list = classes.Users_list.OrderBy(o => o.Name).ToList();
                                    }
                                    else
                                    {
                                        classes.Users_list = classes.Users_list.OrderByDescending(o => o.Name).ToList();
                                    }
                                    break;
                                case "Nazwisko":
                                    if (classes.Asc_Desc == "ASC")
                                    {
                                        classes.Users_list = classes.Users_list.OrderBy(o => o.Surname).ToList();
                                    }
                                    else
                                    {
                                        classes.Users_list = classes.Users_list.OrderByDescending(o => o.Surname).ToList();
                                    }
                                    break;
                                case "Email":
                                    if (classes.Asc_Desc == "ASC")
                                    {
                                        classes.Users_list = classes.Users_list.OrderBy(o => o.Email).ToList();
                                    }
                                    else
                                    {
                                        classes.Users_list = classes.Users_list.OrderByDescending(o => o.Email).ToList();
                                    }
                                    break;
                                case "Numer indeksu":
                                    if (classes.Asc_Desc == "ASC")
                                    {
                                        classes.Users_list = classes.Users_list.OrderBy(o => o.Index_number).ToList();
                                    }
                                    else
                                    {
                                        classes.Users_list = classes.Users_list.OrderByDescending(o => o.Index_number).ToList();
                                    }
                                    break;
                                case "Rola":
                                    if (classes.Asc_Desc == "ASC")
                                    {
                                        classes.Users_list = classes.Users_list.OrderBy(o => o.Role_name).ToList();
                                    }
                                    else
                                    {
                                        classes.Users_list = classes.Users_list.OrderByDescending(o => o.Role_name).ToList();
                                    }
                                    break;


                            }

                            break;
                        case "Zapisz zmiany":
                            try
                            {
                                switch (classes.Role_name)
                                {
                                    case "admin":
                                        classes.Role_id = "0";
                                        break;
                                    case "leader":
                                        classes.Role_id = "1";
                                        break;
                                    case "student":
                                        classes.Role_id = "2";
                                        break;
                                }
                                if (!String.IsNullOrEmpty(classes.Id) && !String.IsNullOrEmpty(classes.Role_id))
                                {
                                    repository.Update_User_role(classes.Id, classes.Role_id);
                                }
                                classes.Users_list = repository.Select_Users_with_Roles();
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            break;
                    }
                    ModelState.Clear();
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region AddClassView [GET]
        public ActionResult AddClassView(Classes classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    var user_id = User.Identity.GetUserId();
                    var classtype_list = repository.Select_classtype();
                    ViewBag.classtype_dropdown = classtype_list;
                    var class_list = repository.Select_User_Classes(user_id);

                    classes = class_list;
                    ModelState.Clear();
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region AddClassView [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClassView(Classes classes, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {

                if (User.IsInRole("leader"))
                {
                    var user_id = User.Identity.GetUserId();
                    var classtype_list = repository.Select_classtype();
                    ViewBag.classtype_dropdown = classtype_list;
                    var class_list = repository.Select_User_Classes(user_id);

                    switch (submit)
                    {
                        case "Dodaj":
                            if (!ModelState.IsValid)
                            {
                                classes = class_list;
                                return View(classes);
                            }
                            classes = services.AddClass_createfolder(classes, user_id);
                            string ViewBagMsg = classes.ViewBagMsg;
                            class_list = repository.Select_User_Classes(user_id);
                            classes = class_list;
                            classes.ViewBagMsg = ViewBagMsg;
                            ModelState.Clear();
                            return View(classes);
                        case "Zarządzaj":
                            Session["ManageClassSession"] = classes;
                            return RedirectToAction("ManageClassView", "ManagementApp");
                    }

                    classes = class_list;
                    ModelState.Clear();
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region ManageClassView [GET]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult ManageClassView(Manage_class_view classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    var classtype_list = repository.Select_classtype();
                    ViewBag.classtype_dropdown = classtype_list;
                    Classes data = (Classes)Session["ManageClassSession"];
                    Session.Clear();
                    classes = services.ManageClass_GET(classes, data);
                    if (classes.Class_id == null)
                    {
                        return RedirectToAction("AddClassView", "ManagementApp");
                    }
                    ModelState.Clear();
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region ManageClassView [POST]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageClassView(Manage_class_view classes, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    if (classes.Class_id != null)
                    {
                        var classtype_list = repository.Select_classtype();
                        ViewBag.classtype_dropdown = classtype_list;
                        if (submit == "Zapisz zmiany")
                        {
                            if (!ModelState.IsValid)
                            {
                                return View(classes);
                            }
                        }
                        classes = services.ManageClass_POST(classes, submit);
                        ModelState.Clear();
                        if (classes.Class_closed == true)
                        {
                            return RedirectToAction("AddClassView", "ManagementApp");
                        }
                        return View(classes);
                    }
                    else
                    {
                        return RedirectToAction("AddClassView", "ManagementApp");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region AddGradesView [GET]
        public ActionResult AddGradesView(Add_Grades_View classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    var Class_names_list = repository.Select_User_Classes_names_list(User.Identity.GetUserId());
                    ViewBag.class_names_list = Class_names_list;
                    var grade_type_list = repository.Select_grade_types();
                    ViewBag.grade_dropdown = grade_type_list;
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region AddGradesView [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGradesView(Add_Grades_View classes, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    var Class_names_list = repository.Select_User_Classes_names_list(User.Identity.GetUserId());
                    ViewBag.class_names_list = Class_names_list;
                    var grade_type_list = repository.Select_grade_types();
                    ViewBag.grade_dropdown = grade_type_list;

                    switch (submit)
                    {
                        case "Zatwierdz":
                            foreach (var student in classes.Students_list)
                                repository.Update_Grades(classes.Class_id, classes.Type, student.Id, student.Grades_list);

                            break;
                        case "Zamknij przedmiot":
                            if (classes.Class_id != null && classes.Name != null && classes.Type != null && classes.Year != null && classes.Semester != null)
                            {
                                repository.Update_Class(classes.Class_id, classes.Name, classes.Type, classes.Year, classes.Semester, true);
                            }
                            break;
                    }
                    ModelState.Clear();
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region StudentView [GET]
        public ActionResult StudentView(Student_View classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("student"))
                {
                    classes = repository.Select_classes_grades(User.Identity.GetUserId());
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region StudentView [POST]
        [HttpPost]
        public ActionResult StudentView(Student_View classes, string submit)
        {
            if (System.Web.HttpContext.Current.User == null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("student"))
                {
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region TeacherFileView [GET]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult TeacherFileView(File_View classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    classes.Check_student_teacher = "teacher";
                    var Class_names_list = repository.Select_User_Classes_names_list(User.Identity.GetUserId());
                    ViewBag.class_names_list = Class_names_list;
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region TeacherFileView [POST]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherFileView(File_View classes, HttpPostedFileBase file, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    classes.Check_student_teacher = "teacher";
                    var Class_names_list = repository.Select_User_Classes_names_list(User.Identity.GetUserId());
                    ViewBag.class_names_list = Class_names_list;
                    if (classes.Folder_name != null && classes.Directory_name != null)
                    {

                        var leader_namesurname = repository.Select_User_NameSurname(User.Identity.GetUserId());
                        var fileName = classes.File_name;
                        var directory_name = classes.Directory_name;
                        var folder_name = classes.Folder_name.Replace("|", " ");
                        folder_name = folder_name.Replace("/", "-");
                        switch (submit)
                        {
                            case "Pobierz":
                                try
                                {
                                    if (classes.File_name != "")
                                    {
                                        var location_ = folder_name + " " + leader_namesurname + "/" + directory_name + "/" + fileName;
                                        var ftpStream = services.FTP_download(location_);
                                        return File(ftpStream, System.Net.Mime.MediaTypeNames.Application.Octet, classes.File_name);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                break;
                            case "Usuń":
                                try
                                {
                                    if (classes.File_name != "")
                                    {
                                        var location_ = folder_name + " " + leader_namesurname + "/" + directory_name + "/" + fileName;
                                        services.FTP_delete(location_);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                break;
                            case "Prześlij":
                                if (file != null)
                                {
                                    string filename = Path.GetFileName(file.FileName);
                                    var location = folder_name + " " + leader_namesurname + "/" + directory_name + "/" + filename;
                                    services.FTP_upload(location, file);
                                }
                                break;
                        }
                        ModelState.Clear();
                        if (classes.Folder_name != "" && classes.Folder_name != null)
                        {
                            var location = folder_name + " " + leader_namesurname + "/" + directory_name;
                            var location1 = folder_name + " " + leader_namesurname;
                            try
                            {
                                classes.Materials_list = services.FTP_GET_filenames(location);
                                foreach (var item in classes.Materials_list)
                                {
                                    classes.Directory_list_timestamps.Add(services.FTP_GET_timestamp(location + "/" + item));
                                }
                            }
                            catch (Exception)
                            {
                                return new HttpStatusCodeResult(550, "Wystąpił problem z dostępem do folderu");
                            }
                            try
                            {
                                classes.Directory_list = services.FTP_GET_filenames(location1);
                            }
                            catch
                            {
                                return new HttpStatusCodeResult(550, "Wystąpił problem z dostępem do folderu");
                            }
                        }
                    }
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region StudentFileView [GET]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public ActionResult StudentFileView(File_View classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("student"))
                {
                    classes.Check_student_teacher = "student";
                    var Class_names_list = repository.Select_Student_Classes_names_list(User.Identity.GetUserId());
                    ViewBag.class_names_list = Class_names_list;
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion
        #region StudentFileView [POST]
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentFileView(File_View classes, HttpPostedFileBase file, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("student"))
                {
                    var Class_names_list = repository.Select_Student_Classes_names_list(User.Identity.GetUserId());
                    ViewBag.class_names_list = Class_names_list;
                    if (classes.Folder_name != null)
                    {
                        classes.Check_student_teacher = "student";
                        var fileName = classes.File_name;
                        var folder_name = classes.Folder_name.Replace("|", " ");
                        folder_name = folder_name.Replace("/", "-");
                        switch (submit)
                        {
                            case "Pobierz":
                                try
                                {
                                    if (classes.File_name != "")
                                    {
                                        classes.Folder_name = "/Materiały/";
                                        var location_ = folder_name + classes.Folder_name + fileName;
                                        var ftpStream = services.FTP_download(location_);
                                        return File(ftpStream, System.Net.Mime.MediaTypeNames.Application.Octet, classes.File_name);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                break;
                            case "Pobierz ":
                                try
                                {
                                    if (classes.File_name != "")
                                    {
                                        classes.Folder_name = repository.Select_User_FolderName(User.Identity.GetUserId());
                                        var location_ = folder_name + "/" + classes.Folder_name + "/" + fileName;
                                        var ftpStream = services.FTP_download(location_);
                                        return File(ftpStream, System.Net.Mime.MediaTypeNames.Application.Octet, classes.File_name);
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                break;
                            case "Prześlij":
                                if (file != null)
                                {
                                    classes.Folder_name = repository.Select_User_FolderName(User.Identity.GetUserId());
                                    string filename = Path.GetFileName(file.FileName);
                                    var location = folder_name + "/" + classes.Folder_name + "/";// + filename;
                                    var file_list = services.FTP_GET_filenames(location);
                                    if (file_list.Contains(filename) == false)
                                    {
                                        location = folder_name + "/" + classes.Folder_name + "/" + filename;
                                        services.FTP_upload(location, file);
                                    }
                                    else
                                    {
                                        classes.ViewBagMsg = "Ten plik już istnieje";
                                    }
                                }
                                break;
                        }
                        if (classes.Folder_name != "" && classes.Folder_name != null)
                        {
                            var location = folder_name + "/" + classes.Folder_name;
                            var location1 = folder_name + "/Materiały";

                            classes.Materials_list = services.FTP_GET_filenames(location1);
                            classes.Student_files_list = services.FTP_GET_filenames(location);
                            foreach (var item in classes.Student_files_list)
                            {
                                classes.Directory_list_timestamps.Add(services.FTP_GET_timestamp(location + "/" + item));
                            }
                        }
                    }
                    return View(classes);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }

        #endregion

        #region ReportView [GET]
        public ActionResult ReportView(Report classes)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    return View(classes);
                }
            }
            return View(classes);
        }
        #endregion
        #region ReportView [POST]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportView(Report classes, string submit)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    try
                    {
                        switch (submit)
                        {
                            case "Generuj raport":
                                if (classes.Year_start != null && classes.Year_end != null)
                                {
                                    classes = repository.Select_for_Report(User.Identity.GetUserId(), classes.Year_start, classes.Year_end);
                                    return View(classes);
                                }
                                else
                                {
                                    return View(classes);
                                }
                            case "Pobierz pdf":
                                if (classes.Year_start != null && classes.Year_end != null)
                                {
                                    classes = repository.Select_for_Report(User.Identity.GetUserId(), classes.Year_start, classes.Year_end);
                                }
                                var test = classes.Classes_list.Count;
                                var report = new PartialViewAsPdf("partialReportView", classes);
                                return report;
                            default:
                                return View(classes);
                        }

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        #endregion

        #region Index [GET]
        public ActionResult Index()
        {
            return View();
        }
        #endregion
        #region Index [POST]
        [HttpPost]
        public ActionResult Index(string submit)
        {
            return View();
        }
        #endregion
        #region AccountActivation [GET]
        public ActionResult AccountActivation()
        {
            if (User.IsInRole("leader") ||  User.IsInRole("student") || User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "ManagementApp");
            }
                return View();
        }
        #endregion
        #region AccountActivation [POST]
        [HttpPost]
        public ActionResult AccountActivation(string submit)
        {
            return View();
        }
        #endregion
        #region script_actions
        [HttpPost]
        public ActionResult Sort_students(Manage_class_view model, string sort, string sort_type)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    model = services.Sort_Students_action(model, sort, sort_type);
                    return PartialView("partialManageClassView", model);
                }
                else
                { return RedirectToAction("Index", "ManagementApp"); }
            }
            else
            { return RedirectToAction("Index", "ManagementApp"); }
        }
        [HttpPost]
        public ActionResult Search_student(Manage_class_view model, string search_column, string search)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    model = services.Search_Students_action(model, search, search_column);
                    return PartialView("partialManageClassView", model);
                }
                else
                { return RedirectToAction("Index", "ManagementApp"); }
            }
            else
            { return RedirectToAction("Index", "ManagementApp"); }
        }
        [HttpPost]
        public ActionResult Select_class_AddGrades(string class_name, string search_column, string search, string sort, string sort_type)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    var grade_type_list = repository.Select_grade_types();
                    ViewBag.grade_dropdown = grade_type_list;
                    var model = services.Select_class_AddGrades_action(class_name, search_column, search, sort, sort_type, User.Identity.GetUserId());
                    return PartialView("partialAddGradesView", model);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        [HttpPost]
        public ActionResult partial_StudentView(string search_column, string search, string sort, string sort_type)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("student"))
                {
                    var model = services.Partial_Student_View_action(search_column, search, sort, sort_type, User.Identity.GetUserId());
                    return PartialView("partialStudentView", model);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        [HttpPost]
        public ActionResult Select_ClassFiles_Teacher(string class_name, string directory_name)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("leader"))
                {
                    var model = services.Select_ClassFiles_Teacher_action(class_name, directory_name, User.Identity.GetUserId());
                    return PartialView("partialFileView", model);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        [HttpPost]
        public ActionResult Select_ClassFiles_Student(string class_name, string search)
        {
            if (System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("student"))
                {
                    var model = services.Select_ClassFiles_Student_action(class_name, search, User.Identity.GetUserId());
                    return PartialView("partialFileView", model);
                }
                else
                {
                    return RedirectToAction("Index", "ManagementApp");
                }
            }
            else
            {
                return RedirectToAction("Index", "ManagementApp");
            }
        }
        [HttpPost]
        public JsonResult Select_ClassDir_Teacher(string class_name)
        {
            var split = class_name.Split('|');
            var id = "";

            if (split.Length > 1)
            {
                var class_id = Int32.Parse(split[0]);
                id = repository.Select_Class_leader_id(class_id);
            }
            var folder_name = class_name.Replace("|", " ");
            var name_surname = repository.Select_User_NameSurname(id);
            folder_name = folder_name.Replace("/", "-");
            var list = new List<string>();
            if (folder_name != "" && name_surname != "")
            {
                list = services.FTP_GET_filenames(folder_name + " " + name_surname);
            }
            else
            {
                list.Add("Brak zapisanych studentów");
            }
            return Json(list);
        }
        #endregion
    }
}
