using MLP_Constructor.Model.MLPParameters;
using MultyLayerPerceptron.CalculatingGraph.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.EntityDataModel
{
    [Table("Perceptrons")]
    public class MLP : IEntitySave
    {
        [Key]
        public int Id { get; set; }

        private PerceptronCreator creator;
        [NotMapped]
        public PerceptronCreator Creator
        {
            get
            {
                if(creator is null)
                {
                creator = JsonConvert.DeserializeObject<PerceptronCreator>(_CreatorData);
                }
                return creator;
            }
            set
            {
                creator = value;
                _CreatorData = JsonConvert.SerializeObject(value);
            }
        }
        [Required]
        public DateTime LastUpdate { get; set; }


        [Required]
        internal string _CreatorData { get; set; }

        public void Load()
        {
            Creator.Id = Id; 
            return;
        }

        public void Save()
        {
            LastUpdate = DateTime.Now;
        }
        public MLP()
        {
        }
    }
}
