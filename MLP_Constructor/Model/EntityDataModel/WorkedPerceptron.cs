using MultyLayerPerceptron.CalculatingGraph.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.EntityDataModel
{
   public class WorkedPerceptron
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Column("Информация по перцептрону")]
        internal string _Perceptron { get; set; }
        [NotMapped]
        public Perceptron Perceptron
        {
            get
            {
                return JsonConvert.DeserializeObject<Perceptron>(_Perceptron);
            }
            set
            {
                _Perceptron = JsonConvert.SerializeObject(_Perceptron);
            }
        }
    }
}
