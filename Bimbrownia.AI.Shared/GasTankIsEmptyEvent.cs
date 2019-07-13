using System;

namespace Bimbrownia.AI.Shared
{
    public class GasTankIsEmptyEvent : Event
    {
        public GasTankIsEmptyEvent(Guid stillId) : base(stillId)
        {
        }
        public GasTankIsEmptyEvent()
        {

        }
    }
}