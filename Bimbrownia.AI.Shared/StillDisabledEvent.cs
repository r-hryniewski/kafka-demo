using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public class StillDisabledEvent : Event
    {
        public StillDisabledEvent(Guid stillId) : base(stillId)
        {
        }

        public StillDisabledEvent()
        {
        }
    }
}
