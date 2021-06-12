using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLP_Constructor.Model.MLPParameters
{
    public interface IDbColumn
    {
        string DbType { get; }
        string DbName { get; }
    }
}
