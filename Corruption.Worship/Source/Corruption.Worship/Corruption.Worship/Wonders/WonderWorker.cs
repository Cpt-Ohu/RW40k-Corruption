using Corruption.Core.Gods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corruption.Worship.Wonders
{
   public class WonderWorker
    {
        public WonderDef Def;
        
        public virtual bool TryExecuteWonder(GodDef god, int worshipPoints) { return false; }

    }
}
