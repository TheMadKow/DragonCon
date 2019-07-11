using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;
using NodaTime;

namespace DragonCon.Logical.Convention
{
    public class DaysBuilder 
    {
        private readonly ConventionBuilder _builder;
        private readonly ConventionWrapper _convention;

        public ConDayWrapper this[LocalDate key]
        {
            get
            {
                return _convention.Days.SingleOrDefault(x => x.Date == key);
            }
        }

        public ConDayWrapper this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;

                return _convention.Days.SingleOrDefault(x => x.Id == key);
            }
        }



        public DaysBuilder(ConventionBuilder builder, ConventionWrapper convention)
        {
            this._convention = convention;
            this._builder = builder;
        }
        
        public ConventionBuilder UpdateDay(LocalDate date, LocalTime from, LocalTime to)
        {
            var newRequest = new ConDay(date, from, to);
            ThrowsInvalidHours(newRequest);
            ThrowsIfDateNotExists(newRequest.Date);

            var existingDay = this[date];

            existingDay.StartTime = newRequest.StartTime;
            existingDay.EndTime = newRequest.EndTime;

            return _builder;
        }

        public ConventionBuilder RemoveDay(LocalDate localDate)
        {
            ThrowsIfDateNotExists(localDate);
            var existingDay = this[localDate];
            _convention.Days.Remove(existingDay);
            _builder.DeletedEntityIds.Add(existingDay.Id);
            return _builder;
        }

        public ConventionBuilder AddDay(LocalDate date, LocalTime from, LocalTime to)
        {
            var day = new ConDay(date, from, to);
            ThrowsInvalidHours(day);
            ThrowsIfDateExists(day.Date);
            _convention.Days.Add(new ConDayWrapper(day));

            return _builder;
        }

        public ConventionBuilder SetTimeSlotStrategy(LocalDate localDate, TimeSlotStrategy strategy)
        {
            ThrowsIfDateNotExists(localDate);
            var existingDay = this[localDate];
            existingDay.TimeSlotStrategy = strategy;
            return _builder;
        }

        private void ThrowsIfDateExists(LocalDate day)
        {
            if (IsDaysExists(day))
            {
                throw new Exception("Convention already contains date");
            }
        }

        private void ThrowsIfDateNotExists(LocalDate day)
        {
            if (!IsDaysExists(day))
            {
                throw new Exception("Convention doesn't include date");
            }
        }


        private static void ThrowsInvalidHours(ConDay day)
        {
            if (day.StartTime >= day.EndTime)
            {
                throw new Exception("Start Time cannot be greater than End Time");
            }
        }

        public bool IsDaysExists(LocalDate day) => _builder.Days[day] != null;
        public List<ConDayWrapper> AllDays => _convention.Days.ToList();
    }
}