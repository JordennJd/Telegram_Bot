using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bot.Domain.Interfaces;
namespace Bot.Domain.Entities
{
    public class ForFunctionEventArgs : EventArgs
    {
        public IUpdate update { get;}

    
        public ForFunctionEventArgs(IUpdate update)
        {
            this.update=update;
        }
    }
}