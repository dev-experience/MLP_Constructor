using MLP_Constructor.Model.DBContext;
using MLP_Constructor.Model.EntityDataModel;
using MLP_Constructor.Model.MLPParameters;
using MultyLayerPerceptron.CalculatingGraph.Network;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model
{
    public static class DataBase
    {
        public static void UploadCreator(PerceptronCreator creator)
        {
            using (var context = new MLPContext())
            {
                var mlp = context.Perceptrons.Find(creator.Id);
                if (mlp is null)
                {
                    mlp = new MLP();
                    context.Perceptrons.Add(mlp);
                }
                mlp.Creator = creator;
                context.SaveChanges();
            }
        }
        public static IEnumerable<PerceptronCreator> LoadSortedPerceptronCreators()
        {
            foreach (var item in GetPerceptrons().OrderByDescending(x => x.LastUpdate.Ticks))
            {
                yield return item.Creator;
            }
        }
        public static IEnumerable<PerceptronCreator> LoadPerceptronCreators()
        {
            foreach (var item in GetPerceptrons())
            {
                yield return item.Creator;
            }
        }
        private static IEnumerable<MLP> GetPerceptrons()
        {
            using (var context = new MLPContext())
            {
                foreach (var item in context.Perceptrons)
                {
                    item.Load();
                    yield return item;
                }
            }
        }
        public static void UpdatePerceptron(PerceptronCreator creator)
        {
            WorkedPerceptron perceptron = new WorkedPerceptron();
            perceptron.Perceptron = creator.Perceptron;
            perceptron.Name = creator.Name;
            perceptron.Id = creator.Id;
            using (var context = new MLPContext())
            {
                var findedPerceptron = context.WorkedPerceptrons
                    .FirstOrDefault(x => x.Id == perceptron.Id);
                if (findedPerceptron is null)
                {
                    context.WorkedPerceptrons.Add(perceptron);
                }
                else
                {
                    findedPerceptron = perceptron;
                }
                context.SaveChanges();
            }
        }
        public static DataBaseContext Context(string connectionString)
        {
            return new DataBaseContext(connectionString);
        }
    }

}
