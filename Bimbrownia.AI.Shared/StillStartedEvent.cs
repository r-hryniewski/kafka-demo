using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public class StillStartedEvent : Event
    {
        public StillStartedEvent (Guid stillId) : base(stillId)
        {
        }

        public StillStartedEvent()
        {
        }
    }
}
