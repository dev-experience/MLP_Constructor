using MultyLayerPerceptron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class Trainer
    {
        private static Matrix[] lastW;
        private readonly PerceptronCreator creator;
        private int trainDataCount;
        private int checkDataCount;
        private int allCount;
        private int current = 0;
        private int last = 0;
        private int batchSize = 20;
        private int[] ids;
        private Random rnd = new Random();
        public Trainer(PerceptronCreator creator)
        {
            this.creator = creator;
            Initiate();
        }
        private void Initiate()
        {
            allCount = creator.DataBase.GetDataCount();
            trainDataCount = (int)(allCount * creator.DataBase.TrainDataPart);
            checkDataCount = allCount - trainDataCount;
            ids = new int[allCount];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = i + 1;
            }
            Mix(ids);

        }
        private double error;
        public double Error => error;
        public async Task<bool> TrainAsync()
        {
            return await Task<bool>.Run(Train);

        }
        public async Task<bool> CheckAsync()
        {
            return await Task<bool>.Run(Check);

        }
        public bool Train()
        {
            if (current > trainDataCount) return false;
            current += batchSize;
            if (current > trainDataCount) current = trainDataCount;
            int[] curIds = new int[current - last];
            for (int i = 0; i < curIds.Length; i++)
            {
                curIds[i] = ids[last + i];
            }
            last = current;
            var trainData = creator.DataBase.GetTrainData(curIds).ToArray();
            var a = creator.Perceptron.Train(out error, trainData);
            return true;
        }
        public bool CanCheck => current <= allCount;
        public bool CanTrain => current <= trainDataCount;
        public bool Check()
        {
            if (current > allCount) return false;
            current += batchSize;
            if (current > allCount) current = allCount;
            int[] curIds = new int[current - last];
            for (int i = 0; i < curIds.Length; i++)
            {
                curIds[i] = ids[last + i];
            }
            last = current;
            var trainData = creator.DataBase.GetTrainData(curIds);
            creator.Perceptron.Check(out error, trainData);
            return true;
        }
        private void Mix(int[] ints)
        {
            int temp;
            int rndIndex;
            int len = ints.Length;
            for (int i = 0; i < len; i++)
            {
                temp = ints[i];
                rndIndex = rnd.Next(i, len);
                ints[i] = ints[rndIndex];
                ints[rndIndex] = temp;
            }
        }

    }
}
