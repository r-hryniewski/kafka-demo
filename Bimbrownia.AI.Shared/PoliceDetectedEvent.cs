using System;

namespace Bimbrownia.AI.Shared
{
    public class PoliceDetectedEvent : Event
    {
        public PoliceDetectedEvent(Guid stillId) : base(stillId)
        {
        }
        public PoliceDetectedEvent()
        {

        }
    }
}