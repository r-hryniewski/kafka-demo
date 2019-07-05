using System;
using System.Collections.Generic;
using System.Text;

namespace Bimbrownia.AI.Shared
{
    public abstract class Event
    {
        public Guid Id { get; set; }
        public DateTime Occured { get; set; }
        public Guid StillId { get; set; }

        public Event(Guid stillId)
        {
            Id = Guid.NewGuid();
            Occured = DateTime.Now;
            StillId = stillId;
        }
        public Event()
        {

        }
    }
}
