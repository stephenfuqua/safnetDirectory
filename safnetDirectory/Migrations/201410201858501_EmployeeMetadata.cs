namespace safnetDirectory.FullMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeMetadata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FullName", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Title", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "Location", c => c.String(maxLength: 100));
            AddColumn("dbo.AspNetUsers", "MobilePhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MobilePhoneNumber");
            DropColumn("dbo.AspNetUsers", "Location");
            DropColumn("dbo.AspNetUsers", "Title");
            DropColumn("dbo.AspNetUsers", "EmailAddress");
        }
    }
}
