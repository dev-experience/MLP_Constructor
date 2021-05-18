using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;

namespace MLP_Constructor.Model.EntityDataModel
{
    public class MLPContext : DbContext
    {
        private const string local = "data source=(LocalDb)\\MSSQLLocalDB;initial catalog=MLP_Constructor.Model.EntityDataModel.Multilayer perceptrons;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        private const string connection = "data source=.\\SQLEXPRESS;initial catalog=Multilayer perceptrons;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        public MLPContext()
            : base(connection)
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MLP>()
                .Property(x => x._CreatorData).HasColumnName("Data");
        }
        public override int SaveChanges()
        {
           
            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if(changedEntity.Entity is IEntitySave savedEntity)
                {
                    savedEntity.Save();
                }
            }

            return base.SaveChanges();
        }
        public DbSet<MLP> Perceptrons { get; set; }
    
    }


}