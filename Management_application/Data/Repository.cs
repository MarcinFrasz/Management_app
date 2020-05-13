using Management_application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Management_application
{

    public interface Repository
    {
        #region SELECTS
        List<AssignRoleView> Select_Users_with_Roles();
        List<string> Select_Roles();
        Report Select_for_Report(string leader_id, string year_start, string year_finish);
        string Select_User_NameSurname(string id);
        string Select_User_FolderName(string id);
        string Select_User_NameSurname(string name, string type, string year, string semester);
        List<string> Select_classtype();
        List<string> Select_grade_types();
        Classes Select_User_Classes(string leader_id);
        Classes Select_Class(int? class_id);
        string Select_Class_leader_id(int? class_id);
        List<string> Select_Student_Classes_names_list(string student_id);
        //List<Classes> Select_User_Classes_list(string leader_id);
        List<string> Select_User_Classes_names_list(string leader_id);
        Users Select_Students();
        List<string> Select_attending_Students(int? class_id, string type);
        List<Attending_students> Select_attending_Students_grades(int? class_id, string type);
        Student_View Select_classes_grades(string student_id);
        bool Select_User_Type(string leader_id);
        string Select_leader(string Leader_id);

        #endregion

        #region INSERTS
        Classes Insert_Class(Classes classes,string leader_id);
        void Insert_Student_Role(string user_id);
        void Insert_Student(int? class_id, string user_id, string type);
        #endregion

        #region DELETE
        void Delete_Student(int? class_id, string user_id, string type);
        void Delete_Class_Student(int? class_id, string id, string type);
        void Delete_Class(int? class_id);
        #endregion

        #region UPDATE
        void Update_User_role(string user_id, string role_id);
        void Update_Class(int? class_id, string name, string type, string year, string semester,bool closed);
        void Update_Grades(int? class_id, string type, string user_id, Grades grades);
        #endregion
    }


}