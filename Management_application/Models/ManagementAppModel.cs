using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Management_application.Models
{
    public class ManagementAppModel
    {
    }
    public class AssignRoleView
    {
        public AssignRoleView()
        {
            Users_list = new List<AssignRoleView>();
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Index_number { get; set; }
        public bool Type { get; set; }
        public string Role_id { get; set; }
        public string Role_name { get; set; }
        public string Search_text { get; set; }
        public string Sort_column { get; set; }
        public string Asc_Desc { get; set; }
        public List<AssignRoleView> Users_list{get;set;}
    }
    public class AddClass
    {
        public AddClass()
        {
            AddClass_list = new List<AddClass>();
        }
        public int? Class_id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string User_id { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public List<AddClass> AddClass_list { get; set; }
    }
    public class Classes
    {
        public Classes()
        {
            Classes_list = new List<Classes>();
        }
        public int? Class_id { get; set; }
        [Required]
        [RegularExpression(@"^[^\/:*?<>|!@#$%^&"".]*$", ErrorMessage= @"Nazwa nie może zawierać znaków \/:*?<>|!@#$%^&*'"".")]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Leader_id { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public string ViewBagMsg { get; set; }

        public List<Classes> Classes_list { get; set; }
    }
    public class Users
    {
        public Users()
        {
            Users_list = new List<Users>();
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Index_number { get; set; }
        public bool Type { get; set; }
        public bool Addremove_student_check { get; set; }
        public List<Users> Users_list { get; set; }
    }
    public class Manage_class_view
    {
        public Manage_class_view()
        {
            Attending_students_list = new List<Users>();
            Not_attending_students_list = new List<Users>();
        }
        public int? Class_id { get; set; }
        [Required]
        [RegularExpression(@"^[^\/:*?<>|!@#$%^&"".]*$", ErrorMessage = @"Nazwa nie może zawierać znaków \/:*?<>|!@#$%^&*'"".")]
        public string Name { get; set; }
        public string Type { get; set; }
        public string Type_change_check { get; set; }
        public string Leader_id { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public string User_id { get; set; }
        public bool Class_closed { get; set; }
        public string ViewBagMsg { get; set; }
        public List<Users> Attending_students_list { get; set; }
        public List<Users> Not_attending_students_list { get; set; }
    }
    public class Add_Grades_View
    {
        public Add_Grades_View()
        {
            Students_list = new List<Attending_students>();
        }
        public int? Class_id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Leader_id { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public List<Attending_students> Students_list { get; set; }
    }
    public class Student_View
    {
        public Student_View()
        {
            Classes_list=new List<Student_View>();
            Classes_list_closed = new List<Student_View>();
        }
        public int? Class_id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Leader_id { get; set; }
        public string Leader_name_surname { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public Grades Grades_list { get; set; }
        public List<Student_View> Classes_list { get; set; }
        public List<Student_View> Classes_list_closed { get; set; }
    }
    public class File_View
    {
        public File_View()
        {
            Folder_name = "";
            File_name = "";
            Check_student_teacher = "";
            Class_name = "";
            Materials_list = new List<string>();
            Directory_list = new List<string>();
            Student_files_list = new List<string>();
            Directory_list_timestamps = new List<DateTime>();
        }
        public string Folder_name { get; set; }
        public string Class_name { get; set; }
        public string Check_student_teacher{get;set;}
        public string File_name { get; set; }
        public string Directory_name { get; set; }
        public string ViewBagMsg { get; set; }
        public List<string> Materials_list { get; set; }
        public List<string> Student_files_list { get; set; }
        public List<string> Directory_list { get; set; }
        public List<DateTime> Directory_list_timestamps { get; set; }
    }
    public class Grades
    {
        public string Grade_final { get; set; }
        public string Grade_1 { get; set; }
        public string Grade_2 { get; set; }
        public string Grade_3 { get; set; }
        public string Grade_4 { get; set; }
        public string Grade_5 { get; set; }
        public string Grade_6 { get; set; }
        public string Grade_7 { get; set; }
        public string Grade_8 { get; set; }
        public string Grade_9 { get; set; }
        public string Grade_10 { get; set; }
        public string Grade_11 { get; set; }
        public string Grade_12 { get; set; }
        public string Grade_13 { get; set; }
        public string Grade_14 { get; set; }
        public string Grade_15 { get; set; }
    }
    public class Attending_students
    {
        public Attending_students()
        {
            Students_list = new List<Attending_students>();
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Index_number { get; set; }
        public bool Type { get; set; }
        public Grades Grades_list { get; set; }
        public List<Attending_students> Students_list { get; set; }
    }
    public class Report
    {
        public Report()
        {
            Classes_list = new List<Classes_students_grades>();
        }
        public string Leader_id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Year_start { get; set; }
        public string Year_end { get; set; }
        public List<Classes_students_grades> Classes_list { get; set; }
    }
    public class Classes_students_grades
    {
        public Classes_students_grades()
        {
            Students_list = new List<Students_with_grades>();
        }
        public int? Class_id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Leader_id { get; set; }
        public string Year { get; set; }
        public string Semester { get; set; }
        public List<Students_with_grades> Students_list { get; set; }
    }
    public class Students_with_grades
    {
        public Students_with_grades()
        {
            Grades_list = new Grades();
        }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Index_number { get; set; }
        public Grades Grades_list { get; set; }
    }
}
