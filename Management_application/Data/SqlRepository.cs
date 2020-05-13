using Management_application.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Management_application.Data
{
    public class SqlRepository : Repository
    {
        string connetionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region SELECTS
        public List<AssignRoleView> Select_Users_with_Roles()
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var Users_list = new List<AssignRoleView>();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT u.Id,u.Name,u.Surname,u.Email,u.Index_number,ur.RoleId,r.Name as RoleName FROM AspNetUsers as u LEFT JOIN AspNetUserRoles as ur ON u.Id=ur.UserId LEFT JOIN AspNetRoles as r ON ur.RoleId=r.Id WHERE u.EmailConfirmed='true'";
                    
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Users_list.Add(new AssignRoleView()
                        {
                            Id = reader["Id"] as string,
                            Name=reader["Name"] as string,
                            Surname=reader["Surname"] as string,
                            Email = reader["Email"] as string,
                            Index_number=reader["Index_number"] as string,
                            Role_id=reader["RoleId"] as string,
                            Role_name=reader["RoleName"] as string,
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return Users_list;
            }
        }
        public Report Select_for_Report(string leader_id, string year_start, string year_finish)
        {
            int start = Int32.Parse(year_start);
            int finish = Int32.Parse(year_finish);
            List<string> year_list = new List<string>();
            while (start < finish)
            {
                year_list.Add((start.ToString()) + "/" + ((start + 1).ToString()));
                start = start + 1;
            }
            var model = new Report();
            using (var sqlConn = new SqlConnection(connetionString))
            {
                for (int i = 0; i < year_list.Count; i++)
                {
                    sqlConn.Open();
                    using (var cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM classes LEFT JOIN AspNetUsers ON classes.leader_id=AspNetUsers.Id WHERE leader_id=@id AND year=@year AND closed='true'";
                        cmd.Parameters.AddWithValue("@id", leader_id);
                        cmd.Parameters.AddWithValue("@year", year_list[i]);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            model.Name =reader["Name"] as string;
                            model.Surname =reader["Surname"] as string;
                            model.Classes_list.Add(new Classes_students_grades()
                            {
                                Class_id = reader["class_id"] as int?,
                                Name = reader["name"] as string,
                                Type = reader["type"] as string,
                                Leader_id = reader["leader_id"] as string,
                                Year = reader["year"] as string,
                                Semester = reader["semester"] as string,
                            });
                        }
                        reader.Close();
                    }
                    sqlConn.Close();
                }
                for (int i = 0; i < model.Classes_list.Count; i++)
                {
                    switch (model.Classes_list[i].Type)
                    {
                        case "Laboratorium":
                            sqlConn.Open();
                            using (var cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = "SELECT l.user_id,l.mark_1,l.mark_2,l.mark_3,l.mark_4,l.mark_5,l.mark_6,l.mark_7,l.mark_8,l.mark_9,l.mark_10,l.mark_11,l.mark_12,l.mark_13,l.mark_14,l.mark_15,l.final_mark,u.Name,u.Surname,u.Email,u.Index_number  FROM laboratories as l INNER JOIN AspNetUsers as u ON l.user_id=u.Id WHERE class_id=@id";
                                cmd.Parameters.AddWithValue("@id", model.Classes_list[i].Class_id);
                                var reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    model.Classes_list[i].Students_list.Add(new Students_with_grades()
                                    {
                                        Id = reader["user_id"] as string,
                                        Name = reader["Name"] as string,
                                        Surname = reader["Surname"] as string,
                                        Email = reader["Email"] as string,
                                        Index_number = reader["Index_number"] as string,
                                        Grades_list = new Grades()
                                        {
                                            Grade_1 = reader["mark_1"] as string,
                                            Grade_2 = reader["mark_2"] as string,
                                            Grade_3 = reader["mark_3"] as string,
                                            Grade_4 = reader["mark_4"] as string,
                                            Grade_5 = reader["mark_5"] as string,
                                            Grade_6 = reader["mark_6"] as string,
                                            Grade_7 = reader["mark_7"] as string,
                                            Grade_8 = reader["mark_8"] as string,
                                            Grade_9 = reader["mark_9"] as string,
                                            Grade_10 = reader["mark_10"] as string,
                                            Grade_11 = reader["mark_11"] as string,
                                            Grade_12 = reader["mark_12"] as string,
                                            Grade_13 = reader["mark_13"] as string,
                                            Grade_14 = reader["mark_14"] as string,
                                            Grade_15 = reader["mark_15"] as string,
                                            Grade_final = reader["final_mark"] as string,
                                        }
                                    });
                                }
                                reader.Close();
                            }
                            sqlConn.Close();
                            break;
                        case "Projekt":
                            sqlConn.Open();
                            using (var cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = "SELECT p.user_id,p.mark,u.Name,u.Surname,u.Email,u.Index_number FROM projects as p INNER JOIN AspNetUsers as u ON p.user_id=u.Id WHERE class_id=@id";
                                cmd.Parameters.AddWithValue("@id", model.Classes_list[i].Class_id);
                                var reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    model.Classes_list[i].Students_list.Add(new Students_with_grades()
                                    {
                                        Id = reader["user_id"] as string,
                                        Name = reader["Name"] as string,
                                        Surname = reader["Surname"] as string,
                                        Email = reader["Email"] as string,
                                        Index_number = reader["Index_number"] as string,
                                        Grades_list = new Grades()
                                        {
                                            Grade_final = reader["mark"] as string,
                                        }
                                    });
                                }
                                reader.Close();
                            }
                            sqlConn.Close();
                            break;
                        case "Wykład":
                            sqlConn.Open();
                            using (var cmd = sqlConn.CreateCommand())
                            {
                                cmd.CommandText = "SELECT l.user_id,l.mark,u.Name,u.Surname,u.Email,u.Index_number FROM lectures as l INNER JOIN AspNetUsers as u ON l.user_id=u.Id WHERE class_id=@id";
                                cmd.Parameters.AddWithValue("@id", model.Classes_list[i].Class_id);
                                var reader = cmd.ExecuteReader();
                                while (reader.Read())
                                {
                                    model.Classes_list[i].Students_list.Add(new Students_with_grades()
                                    {
                                        Id = reader["user_id"] as string,
                                        Name = reader["Name"] as string,
                                        Surname = reader["Surname"] as string,
                                        Email = reader["Email"] as string,
                                        Index_number = reader["Index_number"] as string,
                                        Grades_list = new Grades()
                                        {
                                            Grade_final = reader["mark"] as string,
                                        }
                                    });
                                }
                                reader.Close();
                            }
                            sqlConn.Close();
                            break;
                    }
                }
            }
            return model;
        }
        public string Select_User_NameSurname(string id)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                string name_surname = "";

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name,Surname FROM AspNetUsers WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        name_surname = reader["Name"] as string + " " + reader["Surname"] as string;
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return name_surname;
            }
        }
        public List<string> Select_Roles()
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                List<string> role = new List<string>();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM AspNetRoles";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        role.Add(reader["Name"] as string);
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return role;
            }
        }
        public string Select_User_FolderName(string id)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                string folder_name = "";

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name,Surname,Index_number FROM AspNetUsers WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        folder_name = reader["Index_number"] as string + " " + reader["Name"] as string + " " + reader["Surname"] as string;
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return folder_name;
            }
        }
        public string Select_User_NameSurname(string name, string type, string year, string semester)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                string name_surname = "";
                string id = "";
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT leader_id FROM classes WHERE name=@name AND type=@type AND year=@year AND semester=@semester";
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        id = reader["leader_id"] as string;
                    }
                    reader.Close();
                    sqlConn.Close();
                }
                using (var cmd = sqlConn.CreateCommand())
                {
                    sqlConn.Open();
                    cmd.CommandText = "SELECT Name,Surname FROM AspNetUsers WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        name_surname = reader["Name"] as string + " " + reader["Surname"] as string;
                    }
                    reader.Close();
                    sqlConn.Close();
                }
                return name_surname;
            }
        }
        public List<string> Select_classtype()
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var classtype_list = new List<string>();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT type FROM class_type";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classtype_list.Add(reader["type"] as string);
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return classtype_list;
            }
        }
        public List<string> Select_grade_types()
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var grade_list = new List<string>();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT grade FROM grades";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        grade_list.Add(reader["grade"] as string);
                    }
                    for (int i = 0; i < grade_list.Count; i++)
                    {
                        grade_list[i] = grade_list[i].Trim();
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return grade_list;
            }
        }
        public Classes Select_User_Classes(string leader_id)
        {

            {
                using (var sqlConn = new SqlConnection(connetionString))
                {
                    sqlConn.Open();
                    var classes_list = new List<Classes>();
                    var classes = new Classes();

                    using (var cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM classes WHERE leader_id=@id AND (closed IS NULL OR closed='false')";
                        cmd.Parameters.AddWithValue("@id", leader_id);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            classes_list.Add(new Classes()
                            {
                                Class_id = reader["class_id"] as int?,
                                Name = reader["name"] as string,
                                Type = reader["type"] as string,
                                Leader_id = reader["leader_id"] as string,
                                Year = reader["year"] as string,
                                Semester = reader["semester"] as string,
                            });

                            classes.Classes_list = classes_list;
                        }
                        reader.Close();
                    }
                    sqlConn.Close();
                    return classes;
                }
            }
        }
        public Classes Select_Class(int? class_id)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var classes_list = new List<Classes>();
                var classes = new Classes();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM classes WHERE class_id=@id";
                    cmd.Parameters.AddWithValue("@id", class_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classes.Class_id = reader["class_id"] as int?;
                        classes.Name = reader["name"] as string;
                        classes.Type = reader["type"] as string;
                        classes.Leader_id = reader["leader_id"] as string;
                        classes.Year = reader["year"] as string;
                        classes.Semester = reader["semester"] as string;
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return classes;
            }
        }
        public string Select_Class_leader_id(int? class_id)
        {
            var Leader_id = "";
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM classes WHERE class_id=@id";
                    cmd.Parameters.AddWithValue("@id", class_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Leader_id = reader["leader_id"] as string;
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return Leader_id;
            }
        }
        public List<Classes> Select_User_Classes_list(string leader_id)
        {

            {
                using (var sqlConn = new SqlConnection(connetionString))
                {
                    sqlConn.Open();
                    var classes_list = new List<Classes>();
                    var classes = new Classes();

                    using (var cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT * FROM classes WHERE leader_id=@id";
                        cmd.Parameters.AddWithValue("@id", leader_id);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            classes_list.Add(new Classes()
                            {
                                Class_id = reader["class_id"] as int?,
                                Name = reader["name"] as string,
                                Type = reader["type"] as string,
                                Leader_id = reader["leader_id"] as string,
                                Year = reader["year"] as string,
                                Semester = reader["semester"] as string,
                            });

                            classes.Classes_list = classes_list;
                        }
                        reader.Close();
                    }
                    sqlConn.Close();
                    return classes.Classes_list;
                }
            }
        }
        public List<string> Select_User_Classes_names_list(string leader_id)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var classes_names_list = new List<string>();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM classes WHERE leader_id=@id AND (closed IS NULL OR closed='false')";
                    cmd.Parameters.AddWithValue("@id", leader_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = reader["class_id"] as int?;
                        classes_names_list.Add(id.ToString() + "|" + reader["name"] as string + "|" + reader["type"] as string + " " + reader["year"] as string + " " + reader["semester"] as string);
                    }
                    reader.Close();
                }
                sqlConn.Close();

                return classes_names_list;
            }
        }
        public List<string> Select_Student_Classes_names_list(string student_id)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var classes_names_list = new List<string>();
                var classes_id_list = new List<int?>();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT c.class_id,c.name,c.type,c.year,c.semester,u.Name as leader_name,u.Surname as leader_surname FROM classes as c INNER JOIN AspNetUsers as u ON c.leader_id=u.Id LEFT JOIN projects as p ON p.class_id=c.class_id LEFT JOIN laboratories as lab ON lab.class_id=c.class_id LEFT JOIN lectures as l ON l.class_id=c.class_id WHERE lab.user_id=@id OR l.user_id=@id OR p.user_id=@id AND (c.closed IS NULL OR c.closed='false')";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classes_names_list.Add((reader["class_id"] as int?).ToString() + "|" + reader["name"] as string + "|" + reader["type"] as string + " " + reader["year"] as string + " " + reader["semester"] as string + " " + reader["leader_name"] as string + " " + reader["leader_surname"] as string);
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return classes_names_list;
            }
        }
        public bool Select_User_Type(string leader_id)
        {
            bool type = false;
            {
                using (var sqlConn = new SqlConnection(connetionString))
                {
                    sqlConn.Open();
                    using (var cmd = sqlConn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT type FROM AspNetUsers WHERE Id=@id";
                        cmd.Parameters.AddWithValue("@id", leader_id);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            type = reader["type"] as bool? ?? false;
                        }
                        reader.Close();
                    }

                    return type;
                }
            }
        }
        public Users Select_Students()
        {
            List<string> id_list = new List<string>();
            string id_string = "";
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT UserId FROM AspNetUserRoles WHERE RoleId=2";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        id_list.Add(reader["UserId"] as string);
                    }
                    reader.Close();
                }
                sqlConn.Close();
                for (int i = 0; i < id_list.Count; i++)
                {
                    if (i == id_list.Count - 1)
                    {
                        id_string = id_string + "'" + id_list[i] + "'";
                    }
                    else
                    {
                        id_string = id_string + "'" + id_list[i] + "',";
                    }
                }
            }


            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var students_list = new List<Users>();
                var students = new Users();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id,Email,Name,Surname,Index_number FROM AspNetUsers WHERE Id IN (" + id_string + ")";
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        students_list.Add(new Users()
                        {
                            Id = reader["Id"] as string,
                            Email = reader["Email"] as string,
                            Name = reader["Name"] as string,
                            Surname = reader["Surname"] as string,
                            Index_number = reader["Index_number"] as string,
                        });

                        students.Users_list = students_list;
                    }
                    reader.Close();
                }
                sqlConn.Close();

                return students;
            }

        }
        public List<string> Select_attending_Students(int? class_id, string type)
        {
            List<string> attenting_students_list = new List<string>();
            {
                using (var sqlConn = new SqlConnection(connetionString))
                {
                    sqlConn.Open();
                    using (var cmd = sqlConn.CreateCommand())
                    {
                        switch (type)
                        {
                            case "Wykład":
                                cmd.CommandText = "SELECT user_id FROM lectures WHERE class_id=@class_id";
                                break;
                            case "Laboratorium":
                                cmd.CommandText = "SELECT user_id FROM laboratories WHERE class_id=@class_id";
                                break;
                            case "Projekt":
                                cmd.CommandText = "SELECT user_id FROM projects WHERE class_id=@class_id";
                                break;
                        }
                        cmd.Parameters.AddWithValue("@class_id", class_id);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            attenting_students_list.Add(reader["user_id"] as string);
                        }
                        reader.Close();
                    }
                    sqlConn.Close();
                    return attenting_students_list;
                }
            }
        }
        public List<Attending_students> Select_attending_Students_grades(int? class_id, string type)
        {
            List<Attending_students> attenting_students_list = new List<Attending_students>();

            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    switch (type)
                    {
                        case "Wykład":
                            cmd.CommandText = "SELECT l.user_id,l.mark,u.Name,u.Surname,u.Email,u.Index_number FROM lectures as l INNER JOIN AspNetUsers as u ON l.user_id=u.Id  WHERE class_id=@class_id";
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            var reader_1 = cmd.ExecuteReader();
                            while (reader_1.Read())
                            {
                                attenting_students_list.Add(new Attending_students()
                                {
                                    Id = reader_1["user_id"] as string,
                                    Name = reader_1["Name"] as string,
                                    Surname = reader_1["Surname"] as string,
                                    Email = reader_1["Email"] as string,
                                    Index_number = reader_1["Index_number"] as string,

                                    Grades_list = new Grades()
                                    {
                                        Grade_final = reader_1["mark"] as string,
                                    }

                                });
                            }
                            reader_1.Close();
                            sqlConn.Close();
                            break;
                        case "Laboratorium":
                            cmd.CommandText = "SELECT l.user_id,l.mark_1,l.mark_2,l.mark_3,l.mark_4,l.mark_5,l.mark_6,l.mark_7,l.mark_8,l.mark_9,l.mark_10,l.mark_11,l.mark_12,l.mark_13,l.mark_14,l.mark_15,l.final_mark,u.Name,u.Surname,u.Email,u.Index_number FROM laboratories as l INNER JOIN AspNetUsers as u ON l.user_id=u.Id WHERE class_id=@class_id";
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            var reader_2 = cmd.ExecuteReader();
                            while (reader_2.Read())
                            {
                                attenting_students_list.Add(new Attending_students()
                                {
                                    Id = reader_2["user_id"] as string,
                                    Name = reader_2["Name"] as string,
                                    Surname = reader_2["Surname"] as string,
                                    Email = reader_2["Email"] as string,
                                    Index_number = reader_2["Index_number"] as string,
                                    Grades_list = new Grades()
                                    {
                                        Grade_final = reader_2["final_mark"] as string,
                                        Grade_1 = reader_2["mark_1"] as string,
                                        Grade_2 = reader_2["mark_2"] as string,
                                        Grade_3 = reader_2["mark_3"] as string,
                                        Grade_4 = reader_2["mark_4"] as string,
                                        Grade_5 = reader_2["mark_5"] as string,
                                        Grade_6 = reader_2["mark_6"] as string,
                                        Grade_7 = reader_2["mark_7"] as string,
                                        Grade_8 = reader_2["mark_8"] as string,
                                        Grade_9 = reader_2["mark_9"] as string,
                                        Grade_10 = reader_2["mark_10"] as string,
                                        Grade_11 = reader_2["mark_11"] as string,
                                        Grade_12 = reader_2["mark_12"] as string,
                                        Grade_13 = reader_2["mark_13"] as string,
                                        Grade_14 = reader_2["mark_14"] as string,
                                        Grade_15 = reader_2["mark_15"] as string,
                                    }
                                });
                            }
                            reader_2.Close();
                            sqlConn.Close();
                            break;
                        case "Projekt":
                            cmd.CommandText = "SELECT p.user_id,p.mark,u.Name,u.Surname,u.Email,u.Index_number FROM projects as p INNER JOIN AspNetUsers as u ON p.user_id=u.Id WHERE class_id=@class_id";
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            var reader_3 = cmd.ExecuteReader();
                            while (reader_3.Read())
                            {
                                attenting_students_list.Add(new Attending_students()
                                {
                                    Id = reader_3["user_id"] as string,
                                    Name = reader_3["Name"] as string,
                                    Surname = reader_3["Surname"] as string,
                                    Email = reader_3["Email"] as string,
                                    Index_number = reader_3["Index_number"] as string,
                                    Grades_list = new Grades()
                                    {
                                        Grade_final = reader_3["mark"] as string,
                                    }

                                });
                            }
                            reader_3.Close();
                            sqlConn.Close();
                            break;
                    }
                }
                return attenting_students_list;

            }
        }
        public string Select_leader(string Leader_id)
        {
            var leader = "";
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var classtype_list = new List<string>();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name,Surname FROM AspNetUsers WHERE Id=@id";
                    cmd.Parameters.AddWithValue("@id", Leader_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        leader = reader["Name"] as string + " " + reader["Surname"] as string;
                    }
                    reader.Close();
                }
                sqlConn.Close();
                return leader;
            }
        }
        public Student_View Select_classes_grades(string student_id)
        {
            using (var sqlConn = new SqlConnection(connetionString))
            {
                sqlConn.Open();
                var classes_list = new List<Student_View>();
                var closed_classes_list = new List<Student_View>();
                var classes = new Student_View();

                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT l.class_id,l.mark,c.name,c.type,c.leader_id,c.year,c.semester,u.Name,u.Surname FROM lectures as l INNER JOIN classes as c ON l.class_id=c.class_id INNER JOIN AspNetUsers as u ON u.Id=c.leader_id WHERE l.user_id=@id AND (c.closed IS NULL OR c.closed='false')";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classes_list.Add(new Student_View()
                        {
                            Class_id = reader["class_id"] as int?,
                            Name = reader["name"] as string,
                            Type = reader["type"] as string,
                            Leader_id = reader["leader_id"] as string,
                            Year = reader["year"] as string,
                            Semester = reader["semester"] as string,
                            Leader_name_surname = reader["Name"] as string + " " + reader["Surname"] as string,
                            Grades_list = new Grades()
                            {
                                Grade_final = reader["mark"] as string,
                            }
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.class_id,p.mark,c.name,c.type,c.leader_id,c.year,c.semester,u.Name,u.Surname FROM projects as p INNER JOIN classes as c ON p.class_id=c.class_id INNER JOIN AspNetUsers as u ON u.Id=c.leader_id WHERE p.user_id=@id AND (c.closed IS NULL OR c.closed='false')";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classes_list.Add(new Student_View()
                        {
                            Class_id = reader["class_id"] as int?,
                            Name = reader["name"] as string,
                            Type = reader["type"] as string,
                            Leader_id = reader["leader_id"] as string,
                            Year = reader["year"] as string,
                            Semester = reader["semester"] as string,
                            Leader_name_surname = reader["Name"] as string + " " + reader["Surname"] as string,
                            Grades_list = new Grades()
                            {
                                Grade_final = reader["mark"] as string,
                            }
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT lab.class_id,lab.mark_1,lab.mark_2,lab.mark_3,lab.mark_4,lab.mark_5,lab.mark_6,lab.mark_7,lab.mark_8,lab.mark_9,lab.mark_10,lab.mark_11,lab.mark_12,lab.mark_13,lab.mark_14,lab.mark_15,lab.final_mark,c.name,c.type,c.leader_id,c.year,c.semester,u.Name,u.Surname FROM laboratories as lab INNER JOIN classes as c ON lab.class_id=c.class_id INNER JOIN AspNetUsers as u ON u.Id=c.leader_id WHERE lab.user_id=@id AND (c.closed IS NULL OR c.closed='false')";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        classes_list.Add(new Student_View()
                        {
                            Class_id = reader["class_id"] as int?,
                            Name = reader["name"] as string,
                            Type = reader["type"] as string,
                            Leader_id = reader["leader_id"] as string,
                            Year = reader["year"] as string,
                            Semester = reader["semester"] as string,
                            Leader_name_surname = reader["Name"] as string + " " + reader["Surname"] as string,
                            Grades_list = new Grades()
                            {
                                Grade_1 = reader["mark_1"] as string,
                                Grade_2 = reader["mark_2"] as string,
                                Grade_3 = reader["mark_3"] as string,
                                Grade_4 = reader["mark_4"] as string,
                                Grade_5 = reader["mark_5"] as string,
                                Grade_6 = reader["mark_6"] as string,
                                Grade_7 = reader["mark_7"] as string,
                                Grade_8 = reader["mark_8"] as string,
                                Grade_9 = reader["mark_9"] as string,
                                Grade_10 = reader["mark_10"] as string,
                                Grade_11 = reader["mark_11"] as string,
                                Grade_12 = reader["mark_12"] as string,
                                Grade_13 = reader["mark_13"] as string,
                                Grade_14 = reader["mark_14"] as string,
                                Grade_15 = reader["mark_15"] as string,
                                Grade_final = reader["final_mark"] as string,
                            }
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();

                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT l.class_id,l.mark,c.name,c.type,c.leader_id,c.year,c.semester,u.Name,u.Surname FROM lectures as l INNER JOIN classes as c ON l.class_id=c.class_id INNER JOIN AspNetUsers as u ON u.Id=c.leader_id WHERE l.user_id=@id AND c.closed='true'";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        closed_classes_list.Add(new Student_View()
                        {
                            Class_id = reader["class_id"] as int?,
                            Name = reader["name"] as string,
                            Type = reader["type"] as string,
                            Leader_id = reader["leader_id"] as string,
                            Year = reader["year"] as string,
                            Semester = reader["semester"] as string,
                            Leader_name_surname = reader["Name"] as string +" "+ reader["Surname"] as string,
                            Grades_list = new Grades()
                            {
                                Grade_final = reader["mark"] as string,
                            }
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.class_id,p.mark,c.name,c.type,c.leader_id,c.year,c.semester,u.Name,u.Surname FROM projects as p INNER JOIN classes as c ON p.class_id=c.class_id INNER JOIN AspNetUsers as u ON u.Id=c.leader_id WHERE p.user_id=@id AND c.closed='true'";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        closed_classes_list.Add(new Student_View()
                        {
                            Class_id = reader["class_id"] as int?,
                            Name = reader["name"] as string,
                            Type = reader["type"] as string,
                            Leader_id = reader["leader_id"] as string,
                            Year = reader["year"] as string,
                            Semester = reader["semester"] as string,
                            Leader_name_surname = reader["Name"] as string +" "+reader["Surname"] as string,
                            Grades_list = new Grades()
                            {
                                Grade_final = reader["mark"] as string,
                            }
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();
                sqlConn.Open();
                using (var cmd = sqlConn.CreateCommand())
                {
                    cmd.CommandText = "SELECT lab.class_id,lab.mark_1,lab.mark_2,lab.mark_3,lab.mark_4,lab.mark_5,lab.mark_6,lab.mark_7,lab.mark_8,lab.mark_9,lab.mark_10,lab.mark_11,lab.mark_12,lab.mark_13,lab.mark_14,lab.mark_15,lab.final_mark,c.name,c.type,c.leader_id,c.year,c.semester,u.Name,u.Surname FROM laboratories as lab INNER JOIN classes as c ON lab.class_id=c.class_id INNER JOIN AspNetUsers as u ON u.Id=c.leader_id WHERE lab.user_id=@id AND c.closed='true'";
                    cmd.Parameters.AddWithValue("@id", student_id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        closed_classes_list.Add(new Student_View()
                        {
                            Class_id = reader["class_id"] as int?,
                            Name = reader["name"] as string,
                            Type = reader["type"] as string,
                            Leader_id = reader["leader_id"] as string,
                            Year = reader["year"] as string,
                            Semester = reader["semester"] as string,
                            Leader_name_surname = reader["Name"] as string +" "+reader["Surname"] as string,
                            Grades_list = new Grades()
                            {
                                Grade_1 = reader["mark_1"] as string,
                                Grade_2 = reader["mark_2"] as string,
                                Grade_3 = reader["mark_3"] as string,
                                Grade_4 = reader["mark_4"] as string,
                                Grade_5 = reader["mark_5"] as string,
                                Grade_6 = reader["mark_6"] as string,
                                Grade_7 = reader["mark_7"] as string,
                                Grade_8 = reader["mark_8"] as string,
                                Grade_9 = reader["mark_9"] as string,
                                Grade_10 = reader["mark_10"] as string,
                                Grade_11 = reader["mark_11"] as string,
                                Grade_12 = reader["mark_12"] as string,
                                Grade_13 = reader["mark_13"] as string,
                                Grade_14 = reader["mark_14"] as string,
                                Grade_15 = reader["mark_15"] as string,
                                Grade_final = reader["final_mark"] as string,
                            }
                        });
                    }
                    reader.Close();
                }
                sqlConn.Close();

                classes.Classes_list = classes_list;
                classes.Classes_list_closed = closed_classes_list;
                return classes;
            }
        }

        #endregion
        #region INSERTS
        public Classes Insert_Class(Classes classes, string leader_id)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "INSERT INTO classes(name,leader_id,type,year,semester) VALUES(@name,@leader_id,@type,@year,@semester)";
                query += " SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();

                    cmd.Parameters.AddWithValue("@name", classes.Name);
                    cmd.Parameters.AddWithValue("@leader_id", leader_id);
                    cmd.Parameters.AddWithValue("@type", classes.Type);
                    cmd.Parameters.AddWithValue("@year", classes.Year);
                    cmd.Parameters.AddWithValue("@semester", classes.Semester);

                    classes.Class_id = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlConn.Close();
                }
            }
            return classes;
        }
        public void Insert_Student_Role(string user_id)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                var student_role = "2";
                string query = "INSERT INTO AspNetUserRoles(UserId,RoleId) VALUES(@user_id,@role_id)";
                query += " SELECT SCOPE_IDENTITY()";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();

                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@role_id", student_role);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        public void Insert_Student(int? class_id, string user_id, string type)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "";
                switch (type)
                {
                    case "Wykład":
                        query = "INSERT INTO lectures(class_id,user_id) VALUES(@class_id,@user_id)";
                        break;
                    case "Laboratorium":
                        query = "INSERT INTO laboratories(class_id,user_id) VALUES(@class_id,@user_id)";
                        break;
                    case "Projekt":
                        query = "INSERT INTO projects(class_id,user_id) VALUES(@class_id,@user_id)";
                        break;
                }

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        #endregion
        #region DELETE
        public void Delete_Class(int? class_id)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "DELETE FROM classes WHERE class_id=@class_id";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        public void Delete_Student(int? class_id, string user_id, string type)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "";
                switch (type)
                {
                    case "Wykład":
                        query = "DELETE FROM lectures WHERE class_id=@class_id AND user_id=@user_id";
                        break;
                    case "Laboratorium":
                        query = "DELETE FROM laboratories WHERE class_id=@class_id AND user_id=@user_id";
                        break;
                    case "Projekt":
                        query = "DELETE FROM projects WHERE class_id=@class_id AND user_id=@user_id";
                        break;
                }

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        public void Delete_Class_Student(int? class_id, string id, string type)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "";
                switch (type)
                {
                    case "Wykład":
                        query = "DELETE FROM lectures WHERE class_id=@class_id AND user_id=@user_id";
                        break;
                    case "Laboratorium":
                        query = "DELETE FROM laboratories WHERE class_id=@class_id AND user_id=@user_id";
                        break;
                    case "Projekt":
                        query = "DELETE FROM projects WHERE class_id=@class_id AND user_id=@user_id";
                        break;
                }

                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }

            }
        }
        #endregion
        #region UPDATE
        public void Update_Class(int? class_id, string name, string type, string year, string semester,bool closed)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "UPDATE classes SET name=@name,type=@type,year=@year,semester=@semester,closed=@closed WHERE class_id=@class_id";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();
                    cmd.Parameters.AddWithValue("@class_id", class_id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@closed", closed);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        public void Update_Grades(int? class_id, string type, string user_id, Grades grades)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "";
                switch (type)
                {
                    case "Laboratorium":
                        query = "UPDATE laboratories SET mark_1=@mark_1,mark_2=@mark_2,mark_3=@mark_3,mark_4=@mark_4,mark_5=@mark_5,mark_6=@mark_6,mark_7=@mark_7,mark_8=@mark_8,mark_9=@mark_9,mark_10=@mark_10,mark_11=@mark_11,mark_12=@mark_12,mark_13=@mark_13,mark_14=@mark_14,mark_15=@mark_15,mark_final=@mark_final WHERE class_id=@class_id AND user_id=@user_id";
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.Connection = sqlConn;
                            sqlConn.Open();
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                            if (grades.Grade_1 == null)
                            {
                                grades.Grade_1 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_1", grades.Grade_1);
                            if (grades.Grade_2 == null)
                            {
                                grades.Grade_2 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_2", grades.Grade_2);
                            if (grades.Grade_3 == null)
                            {
                                grades.Grade_3 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_3", grades.Grade_3);
                            if (grades.Grade_4 == null)
                            {
                                grades.Grade_4 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_4", grades.Grade_4);
                            if (grades.Grade_5 == null)
                            {
                                grades.Grade_5 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_5", grades.Grade_5);
                            if (grades.Grade_6 == null)
                            {
                                grades.Grade_6 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_6", grades.Grade_6);
                            if (grades.Grade_7 == null)
                            {
                                grades.Grade_7 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_7", grades.Grade_7);
                            if (grades.Grade_8 == null)
                            {
                                grades.Grade_8 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_8", grades.Grade_8);
                            if (grades.Grade_9 == null)
                            {
                                grades.Grade_9 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_9", grades.Grade_9);
                            if (grades.Grade_10 == null)
                            {
                                grades.Grade_10 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_10", grades.Grade_10);
                            if (grades.Grade_11 == null)
                            {
                                grades.Grade_11 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_11", grades.Grade_11);
                            if (grades.Grade_12 == null)
                            {
                                grades.Grade_12 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_12", grades.Grade_12);
                            if (grades.Grade_13 == null)
                            {
                                grades.Grade_13 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_13", grades.Grade_13);
                            if (grades.Grade_14 == null)
                            {
                                grades.Grade_14 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_14", grades.Grade_14);
                            if (grades.Grade_15 == null)
                            {
                                grades.Grade_15 = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_15", grades.Grade_15);
                            if (grades.Grade_final == null)
                            {
                                grades.Grade_final = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_final", grades.Grade_final);
                            cmd.ExecuteNonQuery();
                            sqlConn.Close();
                        }
                        break;
                    case "Projekt":
                        query = "UPDATE projects SET mark=@mark_final WHERE class_id=@class_id AND user_id=@user_id";
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.Connection = sqlConn;
                            sqlConn.Open();
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                            if (grades.Grade_final == null)
                            {
                                grades.Grade_final = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_final", grades.Grade_final);
                            cmd.ExecuteNonQuery();
                            sqlConn.Close();
                        }
                        break;
                    case "Wykład":
                        query = "UPDATE lectures SET mark=@mark_final WHERE class_id=@class_id AND user_id=@user_id";
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.Connection = sqlConn;
                            sqlConn.Open();
                            cmd.Parameters.AddWithValue("@class_id", class_id);
                            cmd.Parameters.AddWithValue("@user_id", user_id);
                            if (grades.Grade_final == null)
                            {
                                grades.Grade_final = "";
                            }
                            cmd.Parameters.AddWithValue("@mark_final", grades.Grade_final);
                            cmd.ExecuteNonQuery();
                            sqlConn.Close();
                        }
                        break;
                }
            }
        }
        public void Update_User_role(string user_id,string role_id)
        {
            using (SqlConnection sqlConn = new SqlConnection(connetionString))
            {
                string query = "UPDATE AspNetUserRoles SET RoleId=@role_id WHERE UserId=@user_id";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = sqlConn;
                    sqlConn.Open();
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@role_id", role_id);
                    cmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
            }
        }
        #endregion
    }
}