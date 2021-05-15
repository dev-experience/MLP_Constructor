namespace MLP_Constructor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PerceptronLastUpdateAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Perceptrons", "LastUpdate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Perceptrons", "LastUpdate");
        }
    }
}
