using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public class StillPingEvent : Event
    {
        public StillPingEvent(Guid stillId) : base(stillId)
        {
        }

        public StillPingEvent()
        {
        }
    }
}
