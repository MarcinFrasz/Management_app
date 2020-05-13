using Management_application.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace Management_application.Services
{
    public class ControllerServices
    {
        public ControllerServices(Repository Repository)
        {
            this.repository = Repository;
            FTP_login = WebConfigurationManager.AppSettings["FTP_login"];
            FTP_password = WebConfigurationManager.AppSettings["FTP_password"];
            FTP_link = WebConfigurationManager.AppSettings["FTP_link"];
        }
        private readonly Repository repository;
        private readonly string FTP_login = WebConfigurationManager.AppSettings["FTP_login"];
        private readonly string FTP_password = WebConfigurationManager.AppSettings["FTP_password"];
        private readonly string FTP_link = WebConfigurationManager.AppSettings["FTP_link"];
        public Classes AddClass_createfolder(Classes classes, string user_id)
        {
            if (classes.Name != null && classes.Type != null && classes.Year != null && classes.Semester != null)
            {
                try
                {
                    classes = repository.Insert_Class(classes, user_id);
                    var leader = repository.Select_leader(user_id);
                    var year = classes.Year.Replace("/", "-");
                    string location = classes.Class_id.ToString() + " " + classes.Name + " " + classes.Type + " " + year + " " + classes.Semester + " " + leader;

                    if (FTP_create_directory(location) == true)
                    {
                        FTP_create_directory(location + "/Materiały");
                    }
                    else
                    {
                        repository.Delete_Class(classes.Class_id);
                        classes.ViewBagMsg = "Tworzenie zajęć nie powiodło się. Wystąpił problem podczas tworzenia folderu zajęć.";
                    }
                    return classes;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return classes;
            }
        }
        public Manage_class_view ManageClass_GET(Manage_class_view classes, Classes data)
        {
            if (data != null)
            {
                classes.Class_id = data.Class_id;
                classes.Type = data.Type;
                classes.Type_change_check = data.Type;
                classes.Name = data.Name;
                classes.Year = data.Year;
                classes.Leader_id = data.Leader_id;
                classes.Semester = data.Semester;
            }
            if(classes.Class_id==null)
            {
                return classes;
            }
            var students_list = repository.Select_Students();
            var attending_students_id_list = repository.Select_attending_Students(classes.Class_id, classes.Type);
            if (classes.Not_attending_students_list.Count != 0)
            {
                classes.Not_attending_students_list.Clear();
            }
            if (classes.Attending_students_list.Count != 0)
            {
                classes.Attending_students_list.Clear();
            }
            foreach (var student in students_list.Users_list)
            {
                bool check = false;
                foreach (var student_id in attending_students_id_list)
                {
                    if (student.Id == student_id)
                    {
                        classes.Attending_students_list.Add(student);
                        attending_students_id_list.Remove(student_id);
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    classes.Not_attending_students_list.Add(student);
                }
            }
            return classes;
        }
        public Manage_class_view ManageClass_POST(Manage_class_view classes, string submit)
        {
            switch (submit)
            {
                case "Dodaj studentów":
                    for (int i = 0; i < classes.Not_attending_students_list.Count; i++)
                    {
                        if (classes.Not_attending_students_list[i].Addremove_student_check == true)
                        {
                            var leader_ = repository.Select_leader(classes.Leader_id);
                            var year_ = classes.Year.Replace("/", "-");

                            var folder_name_ = classes.Class_id.ToString() + " " + classes.Name + " " + classes.Type + " " + year_ + " " + classes.Semester + " " + leader_;
                            var location_ = folder_name_ + "/" + classes.Not_attending_students_list[i].Index_number + " " + classes.Not_attending_students_list[i].Name + " " + classes.Not_attending_students_list[i].Surname;
                            if (FTP_create_directory(location_) == true)
                            {
                                repository.Insert_Student(classes.Class_id, classes.Not_attending_students_list[i].Id, classes.Type);
                                classes.Not_attending_students_list[i].Addremove_student_check = false;
                            }
                            else
                            {
                                var list = FTP_GET_filenames(folder_name_ + "/");
                                if (list.Count != 0)
                                {
                                    var check = classes.Not_attending_students_list[i].Index_number + " " + classes.Not_attending_students_list[i].Name + " " + classes.Not_attending_students_list[i].Surname;
                                    if(list.Contains(check))
                                    {
                                        repository.Insert_Student(classes.Class_id, classes.Not_attending_students_list[i].Id, classes.Type);
                                        classes.Not_attending_students_list[i].Addremove_student_check = false;
                                    }
                                }
                                else
                                {
                                    classes.ViewBagMsg = " " + classes.Not_attending_students_list[i].Index_number + " " + classes.Not_attending_students_list[i].Name + " " + classes.Not_attending_students_list[i].Surname;
                                }
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(classes.ViewBagMsg))
                    {
                        classes.ViewBagMsg = "Nie udało się dodać" + classes.ViewBagMsg + ". Nie udało się utworzyć dla nich folderu.";
                    }
                    break;
                case "Usuń studentów":
                    for (int i = 0; i < classes.Attending_students_list.Count; i++)
                    {
                        if (classes.Attending_students_list[i].Addremove_student_check == true)
                        {
                            repository.Delete_Student(classes.Class_id, classes.Attending_students_list[i].Id, classes.Type);
                            classes.Attending_students_list[i].Addremove_student_check = false;
                            var leader_ = repository.Select_leader(classes.Leader_id);
                            var year_ = classes.Year.Replace("/", "-");
                            var folder_name_ = classes.Class_id.ToString() + " " + classes.Name + " " + classes.Type + " " + year_ + " " + classes.Semester + " " + leader_;
                            var location_ = folder_name_ + "/" + classes.Attending_students_list[i].Index_number + " " + classes.Attending_students_list[i].Name + " " + classes.Attending_students_list[i].Surname;
                            FTP_delete_directory(location_);
                        }
                    }
                    break;
                case "Zapisz zmiany":
                    var leader = repository.Select_leader(classes.Leader_id);
                    var old_class = repository.Select_Class(classes.Class_id);
                    var year = classes.Year.Replace("/", "-");
                    var old_year = old_class.Year.Replace("/", "-");
                    var folder_name = old_class.Class_id.ToString() + " " + old_class.Name + " " + old_class.Type + " " + old_year + " " + old_class.Semester + " " + leader;
                    var rename = classes.Class_id.ToString() + " " + classes.Name + " " + classes.Type + " " + year + " " + classes.Semester + " " + leader;
                    if(FTP_rename_directory(folder_name, rename))
                    {
                        if (classes.Type != classes.Type_change_check)
                        {
                            for (int i = 0; i < classes.Attending_students_list.Count; i++)
                            {
                                repository.Delete_Class_Student(classes.Class_id, classes.Attending_students_list[i].Id, classes.Type_change_check);
                                repository.Insert_Student(classes.Class_id, classes.Attending_students_list[i].Id, classes.Type);
                            }
                        }
                        repository.Update_Class(classes.Class_id, classes.Name, classes.Type, classes.Year, classes.Semester,classes.Class_closed);
                    }
                    else
                    {
                        classes.ViewBagMsg = "Wystąpił problem podczas modyfikacji folderu zajęć. Modyfikacja zajęć nie powiodła się.";
                    }
                   
                    break;
            }
            var attending_students_id_list = repository.Select_attending_Students(classes.Class_id, classes.Type);
            var students_list = repository.Select_Students();
            if (classes.Not_attending_students_list.Count != 0)
            {
                classes.Not_attending_students_list.Clear();
            }
            if (classes.Attending_students_list.Count != 0)
            {
                classes.Attending_students_list.Clear();
            }
            foreach (var student in students_list.Users_list)
            {
                bool check = false;
                foreach (var student_id in attending_students_id_list)
                {
                    if (student.Id == student_id)
                    {
                        classes.Attending_students_list.Add(student);
                        attending_students_id_list.Remove(student_id);
                        check = true;
                        break;
                    }
                }
                if (check == false)
                {
                    classes.Not_attending_students_list.Add(student);
                }
            }
            return classes;
        }
        public Stream FTP_download(string location)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(FTP_link + location);
                ftpRequest.Credentials = new NetworkCredential(FTP_login, FTP_password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                var ftpStream = ftpResponse.GetResponseStream();
                return ftpStream;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool FTP_create_directory(string location)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(FTP_link + location);
                ftpRequest.Credentials = new NetworkCredential(FTP_login, FTP_password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void FTP_delete(string location)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(FTP_link + location);
                ftpRequest.Credentials = new NetworkCredential(FTP_login, FTP_password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool FTP_rename_directory(string location, string rename)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(FTP_link + location);
                ftpRequest.Credentials = new NetworkCredential(FTP_login, FTP_password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.RenameTo = rename;
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool FTP_delete_directory(string location)
        {
            try
            {
                var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(FTP_link + location);
                ftpRequest.Credentials = new NetworkCredential(FTP_login, FTP_password);
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                var ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void FTP_upload(string location, HttpPostedFileBase file)
        {
            byte[] fileBytes = null;
            using (BinaryReader fileStream = new BinaryReader(file.InputStream))
            {
                fileBytes = fileStream.ReadBytes(file.ContentLength);
                fileStream.Close();
            }
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_link + location);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(FTP_login, FTP_password);
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.ServicePoint.ConnectionLimit = fileBytes.Length;
                request.EnableSsl = false;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> FTP_GET_filenames(string location)
        {
            try
            {
                List<string> list = new List<string>();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_link + location);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(FTP_login, FTP_password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();
                reader.Close();
                response.Close();
                list = names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return list;
            }
            catch(Exception)
            {
                throw;
            }
        }
        public DateTime FTP_GET_timestamp(string location)
        {
            try
            {
                List<string> list = new List<string>();
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTP_link + location);
                request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                request.Credentials = new NetworkCredential(FTP_login, FTP_password);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                DateTime timestamp=response.LastModified;
                response.Close();
                return timestamp;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public File_View StudentFile_POST(File_View classes, string location, string location_2)
        {
            classes.Check_student_teacher = "student";

            if (classes.Folder_name != "" && classes.Folder_name != null)
            {
                try
                {
                    classes.Materials_list = FTP_GET_filenames(location);
                    classes.Student_files_list = FTP_GET_filenames(location_2);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return classes;
        }
        public Manage_class_view Sort_Students_action(Manage_class_view model, string sort, string sort_type)
        {
            switch (sort)
            {
                case "Imię":
                    model.Attending_students_list.Sort((x, y) => x.Name.CompareTo(y.Name));
                    model.Not_attending_students_list.Sort((x, y) => x.Name.CompareTo(y.Name));
                    if (sort_type == "DESC")
                    {
                        model.Attending_students_list.Reverse();
                        model.Not_attending_students_list.Reverse();
                    }
                    break;
                case "Nazwisko":
                    model.Attending_students_list.Sort((x, y) => x.Surname.CompareTo(y.Surname));
                    model.Not_attending_students_list.Sort((x, y) => x.Surname.CompareTo(y.Surname));
                    if (sort_type == "DESC")
                    {
                        model.Attending_students_list.Reverse();
                        model.Not_attending_students_list.Reverse();
                    }
                    break;
                case "Email":
                    model.Attending_students_list.Sort((x, y) => x.Email.CompareTo(y.Email));
                    model.Not_attending_students_list.Sort((x, y) => x.Email.CompareTo(y.Email));
                    if (sort_type == "DESC")
                    {
                        model.Attending_students_list.Reverse();
                        model.Not_attending_students_list.Reverse();
                    }
                    break;
                case "Numer indeksu":
                    model.Attending_students_list.Sort((x, y) => x.Index_number.CompareTo(y.Index_number));
                    model.Not_attending_students_list.Sort((x, y) => x.Index_number.CompareTo(y.Index_number));
                    if (sort_type == "DESC")
                    {
                        model.Attending_students_list.Reverse();
                        model.Not_attending_students_list.Reverse();
                    }
                    break;
                default:
                    break;
            }
            return model;
        }
        public Manage_class_view Search_Students_action(Manage_class_view model, string search, string search_column)
        {
            var possitive_search_list_attending = new List<Users>();
            var possitive_search_list_notattending = new List<Users>();
            switch (search_column)
            {
                case "Imię":
                    foreach (var student in model.Attending_students_list)
                    {
                        if (student.Name.Contains(search) == true)
                        {
                            possitive_search_list_attending.Add(student);
                        }
                    }
                    foreach (var student in model.Not_attending_students_list)
                    {
                        if (student.Name.Contains(search) == true)
                        {
                            possitive_search_list_notattending.Add(student);
                        }
                    }
                    break;
                case "Nazwisko":
                    foreach (var student in model.Attending_students_list)
                    {
                        if (student.Surname.Contains(search) == true)
                        {
                            possitive_search_list_attending.Add(student);
                        }
                    }
                    foreach (var student in model.Not_attending_students_list)
                    {
                        if (student.Surname.Contains(search) == true)
                        {
                            possitive_search_list_notattending.Add(student);
                        }
                    }
                    break;
                case "Email":
                    foreach (var student in model.Attending_students_list)
                    {
                        if (student.Email.Contains(search) == true)
                        {
                            possitive_search_list_attending.Add(student);
                        }
                    }
                    foreach (var student in model.Not_attending_students_list)
                    {
                        if (student.Email.Contains(search) == true)
                        {
                            possitive_search_list_notattending.Add(student);
                        }
                    }
                    break;
                case "Numer indeksu":
                    foreach (var student in model.Attending_students_list)
                    {
                        if (student.Index_number.Contains(search) == true)
                        {
                            possitive_search_list_attending.Add(student);
                        }
                    }
                    foreach (var student in model.Not_attending_students_list)
                    {
                        if (student.Index_number.Contains(search) == true)
                        {
                            possitive_search_list_notattending.Add(student);
                        }
                    }
                    break;
                default:
                    break;
            }
            model.Attending_students_list = possitive_search_list_attending;
            model.Not_attending_students_list = possitive_search_list_notattending;
            return model;
        }
        public Add_Grades_View Select_class_AddGrades_action(string class_name, string search_column, string search, string sort, string sort_type, string user_id)
        {
            var split = class_name.Split('|');
            var model = new Add_Grades_View();
            if (split.Count() == 3)
            {
                model.Class_id = Int32.Parse(split[0]);
                model.Name = split[1];
                var split1 = split[2].Split(' ');
                if (split1.Count() == 3)
                {
                    model.Type = split1[0];
                    model.Year = split1[1];
                    model.Semester = split1[2];

                    model.Students_list = repository.Select_attending_Students_grades(model.Class_id, model.Type);
                    var list = new List<Attending_students>();
                    switch (search_column)
                    {
                        case "Imię":
                            foreach (var student in model.Students_list)
                            {
                                if (student.Name.Contains(search) == true)
                                {
                                    list.Add(student);
                                }
                            }
                            model.Students_list = list;
                            break;
                        case "Nazwisko":
                            foreach (var student in model.Students_list)
                            {
                                if (student.Surname.Contains(search) == true)
                                {
                                    list.Add(student);
                                }
                            }
                            model.Students_list = list;
                            break;
                        case "Numer indeksu":
                            foreach (var student in model.Students_list)
                            {
                                if (student.Index_number.Contains(search) == true)
                                {
                                    list.Add(student);
                                }
                            }
                            model.Students_list = list;
                            break;
                    }
                    switch (sort)
                    {
                        case "Imię":
                            model.Students_list.Sort((x, y) => x.Name.CompareTo(y.Name));
                            if (sort_type == "DESC")
                            {
                                model.Students_list.Reverse();
                            }
                            break;
                        case "Nazwisko":
                            model.Students_list.Sort((x, y) => x.Surname.CompareTo(y.Surname));
                            if (sort_type == "DESC")
                            {
                                model.Students_list.Reverse();
                            }
                            break;
                        case "Numer indeksu":
                            model.Students_list.Sort((x, y) => x.Index_number.CompareTo(y.Index_number));
                            if (sort_type == "DESC")
                            {
                                model.Students_list.Reverse();
                            }
                            break;
                    }
                }
            }
            return model;


        }
        public Student_View Partial_Student_View_action(string search_column, string search, string sort, string sort_type, string user_id)
        {
            var model = repository.Select_classes_grades(user_id);
            var list = new List<Student_View>();
            switch (search_column)
            {
                case "Nazwa":
                    foreach (var item in model.Classes_list)
                    {
                        if (item.Name.Contains(search) == true)
                        {
                            list.Add(item);
                        }
                    }
                    model.Classes_list = list;
                    break;
                case "Typ":
                    foreach (var item in model.Classes_list)
                    {
                        if (item.Type.Contains(search) == true)
                        {
                            list.Add(item);
                        }
                    }
                    model.Classes_list = list;
                    break;
                case "Rok":
                    foreach (var item in model.Classes_list)
                    {
                        if (item.Year.Contains(search) == true)
                        {
                            list.Add(item);
                        }
                    }
                    model.Classes_list = list;
                    break;
                case "Semestr":
                    foreach (var item in model.Classes_list)
                    {
                        if (item.Semester.Contains(search) == true)
                        {
                            list.Add(item);
                        }
                    }
                    model.Classes_list = list;
                    break;
                case "Prowadzący":
                    foreach (var item in model.Classes_list)
                    {
                        if (item.Leader_name_surname.Contains(search) == true)
                        {
                            list.Add(item);
                        }
                    }
                    model.Classes_list = list;
                    break;
            }
            switch (sort)
            {
                case "Nazwa":
                    model.Classes_list.Sort((x, y) => x.Name.CompareTo(y.Name));
                    if (sort_type == "DESC")
                    {
                        model.Classes_list.Reverse();
                    }
                    break;
                case "Typ":
                    model.Classes_list.Sort((x, y) => x.Type.CompareTo(y.Type));
                    if (sort_type == "DESC")
                    {
                        model.Classes_list.Reverse();
                    }
                    break;
                case "Rok":
                    model.Classes_list.Sort((x, y) => x.Year.CompareTo(y.Year));
                    if (sort_type == "DESC")
                    {
                        model.Classes_list.Reverse();
                    }
                    break;
                case "Semestr":
                    model.Classes_list.Sort((x, y) => x.Semester.CompareTo(y.Semester));
                    if (sort_type == "DESC")
                    {
                        model.Classes_list.Reverse();
                    }
                    break;
                case "Prowadzący":
                    model.Classes_list.Sort((x, y) => x.Leader_name_surname.CompareTo(y.Leader_name_surname));
                    if (sort_type == "DESC")
                    {
                        model.Classes_list.Reverse();
                    }
                    break;
            }
            return model;
        }
        public File_View Select_ClassFiles_Teacher_action(string class_name, string directory_name, string user_id)
        {
            var model = new File_View();
            model.Check_student_teacher = "teacher";
            var leader_namesurname = repository.Select_User_NameSurname(user_id);
            model.Folder_name = class_name;
            model.Directory_name = directory_name;
            var folder_name = class_name.Replace("|", " ");
            folder_name = folder_name.Replace("/", "-");
            var location = folder_name + " " + leader_namesurname + "/" + directory_name;
            var location1 = folder_name + " " + leader_namesurname;
            if (class_name != "" && class_name != null && directory_name!="Wybierz" && directory_name!="")
            {
                model.Materials_list = FTP_GET_filenames(location);
                model.Directory_list = FTP_GET_filenames(location1);
                foreach(var item in model.Materials_list)
                {
                    model.Directory_list_timestamps.Add(FTP_GET_timestamp(location +"/"+ item));
                }
            }
            return model;
        }
        public File_View Select_ClassFiles_Student_action(string class_name, string search, string user_id)
        {
            var model = new File_View();
            model.Check_student_teacher = "student";
            model.Folder_name = class_name;
            var folder_name = class_name.Replace("|", " ");
            folder_name = folder_name.Replace("/", "-");
            var student_folder_name = folder_name;
            folder_name = folder_name + "/Materiały";
            student_folder_name = student_folder_name + "/" + repository.Select_User_FolderName(user_id);
            if (class_name != "" && class_name != null)
            {
                model.Materials_list = FTP_GET_filenames(folder_name);
                model.Student_files_list = FTP_GET_filenames(student_folder_name);
                foreach (var item in model.Student_files_list)
                {
                    model.Directory_list_timestamps.Add(FTP_GET_timestamp(student_folder_name+"/"+ item));
                }
            }
            return model;
        }
    }
}
