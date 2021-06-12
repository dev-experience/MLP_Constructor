using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public class OutputParameters : PerceptronParameter,IDbColumn
    {
        public string Name { get; set; }
        public List<string> AlternativeNames { get; set; }
        public OutputParameters()
        {
            AlternativeNames = new List<string>();
        }
        public OutputParameters(string name) : this()
        {

            Name = name;
        }
        public void AddNameRange(params string[] names)
        {
            foreach (var item in names)
            {
                AddName(item);
            }
        }
        public void ClearAlternatives()
        {
            if (AlternativeNames.Contains(Name))
            {
                AlternativeNames.Remove(Name);
            }
            List<string> toRemoveList = new List<string>();
            for (int i = 0; i < AlternativeNames.Count - 1; i++)
            {
                for (int j = i + 1; j < AlternativeNames.Count; j++)
                {
                    if (AlternativeNames[i] == AlternativeNames[j])
                    {
                        toRemoveList.Add(AlternativeNames[i]);
                    }
                }
            }
            foreach (var item in toRemoveList)
            {
                AlternativeNames.Remove(item);
            }
        }
        public void AddName(string newName)
        {
            if (!AlternativeNames.Contains(newName))
            {
                AlternativeNames.Add(newName);
            }
        }
        public string CreateToolTip()
        {

            return AlternativeNames.Count > 0 ?
                "Альтернативные названия: " + string.Join(";", AlternativeNames) :
                "Нет альтернативных названий";
        }
        protected override bool CheckCorrect()
        {
            return !(Name is null);
        }
        public string DbType => "DECIMAL(18,6)";
        public string DbName => $"[{Name}]";
    }
}
