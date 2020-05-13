namespace Management_application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNameSurnameIndexNuber : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Name", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Surname", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "Index_number", c => c.String(nullable: false));
            DropColumn("dbo.AspNetUsers", "FullName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "FullName", c => c.String(nullable: false));
            DropColumn("dbo.AspNetUsers", "Index_number");
            DropColumn("dbo.AspNetUsers", "Surname");
            DropColumn("dbo.AspNetUsers", "Name");
        }
    }
}
