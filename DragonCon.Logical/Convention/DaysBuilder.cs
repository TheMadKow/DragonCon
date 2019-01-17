using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
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
                if (_convention.Days.ContainsKey(key))
                    return _convention.Days[key];
                return null;
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

            var existingDay = _convention.Days[date];

            existingDay.StartTime = newRequest.StartTime;
            existingDay.EndTime = newRequest.EndTime;

            return _builder;
        }

        public ConventionBuilder RemoveDay(LocalDate localDate)
        {
            ThrowsIfDateNotExists(localDate);
            var existingDay = _convention.Days[localDate];
            _convention.Days.Remove(existingDay.Date);
            return _builder;
        }

        public ConventionBuilder AddDay(LocalDate date, LocalTime from, LocalTime to)
        {
            var day = new ConDay(date, from, to);
            ThrowsInvalidHours(day);
            ThrowsIfDateExists(day.Date);
            _convention.Days.Add(day.Date, new ConDayWrapper(day));

            return _builder;
        }

        public ConventionBuilder SetTimeSlotStrategy(LocalDate localDate, TimeSlotStrategy strategy)
        {
            ThrowsIfDateNotExists(localDate);
            var existingDay = _convention.Days[localDate];
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

        public bool IsDaysExists(LocalDate day) => _convention.Days.ContainsKey(day);
        public List<ConDayWrapper> AllDays => _convention.Days.Values.ToList();
    }
}