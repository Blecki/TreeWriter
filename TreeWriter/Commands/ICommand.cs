using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeWriterWF
{
    public interface ICommand
    {
        void Execute(Model Model, Main View);
        bool Succeeded { get; }
    }
}
