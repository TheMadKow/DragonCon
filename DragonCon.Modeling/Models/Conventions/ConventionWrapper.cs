using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConventionWrapper : Wrapper<Convention>
    {
        public ConventionWrapper()
        {
            Inner = new Convention();
        }

        public ConventionWrapper(Convention inner) : base(inner) { }

        public List<Hall> Halls { get; set; } = new List<Hall>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<Day> Days { get; set; } = new List<Day>();
   
        public T? GetById<T>(string key)
        where T : class
        {
            var def = Activator.CreateInstance(typeof(T));
            return def switch
            {
                Ticket _ => (Tickets.FirstOrDefault(x => x.Id == key) as T),
                Day _ => (Days.FirstOrDefault(x => x.Id == key) as T),
                Hall _ => (Halls.FirstOrDefault(x => x.Id == key) as T),
                _ => null
            };
        }
    }
}
