namespace MLP_Constructor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredLastUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Perceptrons", "Data", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Perceptrons", "Data", c => c.String());
        }
    }
}
