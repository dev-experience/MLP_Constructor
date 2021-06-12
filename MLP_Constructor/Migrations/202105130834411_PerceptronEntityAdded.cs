namespace MLP_Constructor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PerceptronEntityAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Perceptrons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Perceptrons");
        }
    }
}
