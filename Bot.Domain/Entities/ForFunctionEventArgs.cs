using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bot.Domain.Interfaces;
namespace Bot.Domain.Entities
{
    public class ForFunctionEventArgs : EventArgs
    {
        public CoreUpdate update { get;}

    
        public ForFunctionEventArgs(CoreUpdate update)
        {
            this.update=update;
        }
    }
}