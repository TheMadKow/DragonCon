using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.HallsTables;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;
using NotImplementedException = System.NotImplementedException;

namespace DragonCon.Modeling.Models.Conventions
{
    public class ConventionWrapper : Wrapper<Convention>
    {
        public ConventionWrapper()
        {
            Model = new Convention();
        }

        public ConventionWrapper(Convention model) : base(model) { }

        public string Id { get => Model.Id; set => Model.Id = value; }
        public string Name { get => Model.Name; set => Model.Name = value; }

        public List<Hall> Halls { get; set; } = new List<Hall>();
        public List<Ticket> Tickets { get; set; } = new List<Ticket>();
        public List<Day> Days { get; set; } = new List<Day>();
   
        public Instant CreateTimeStamp { get => Model.CreateTimeStamp; set => Model.CreateTimeStamp = value; }
        public Instant UpdateTimeStamp { get => Model.UpdateTimeStamp; set => Model.UpdateTimeStamp = value; }
        
        public string Location { get => Model.Location; set => Model.Location = value; }
        public string TagLine { get => Model.TagLine; set => Model.TagLine = value; }


        public T GetById<T>(string key)
        where T : class
        {
            var def = Activator.CreateInstance(typeof(T));
            switch (def)
            {
                case Ticket t:
                    return Tickets.FirstOrDefault(x => x.Id == key) as T;
                case Day d:
                    return Days.FirstOrDefault(x => x.Id == key) as T;
                case Hall h:
                    return Halls.FirstOrDefault(x => x.Id == key) as T;
            }
            return null;
        }
    }
}
