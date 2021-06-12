namespace MLP_Constructor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkedPerceptronAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkedPerceptrons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WorkedPerceptrons");
        }
    }
}
