using System;
using System.Collections.Generic;
using System.Linq;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using NodaTime;

namespace DragonCon.Logical.Convention
{
    public class DaysBuilder  : BuilderBase<Day>
    {
        public Day this[LocalDate key]
        {
            get
            {
                return Convention.Days.SingleOrDefault(x => x.Date == key);
            }
        }

        public DaysBuilder(ConventionBuilder builder, ConventionWrapper convention) : base(builder, convention)
        {
        }
        
        public ConventionBuilder UpdateDay(LocalDate date, LocalTime from, LocalTime to)
        {
            var newRequest = new Day(date, from, to);
            ThrowsInvalidHours(newRequest);
            ThrowsIfDateNotExists(newRequest.Date);

            var existingDay = this[date];

            existingDay.StartTime = newRequest.StartTime;
            existingDay.EndTime = newRequest.EndTime;

            return Parent;
        }

        public ConventionBuilder RemoveDay(LocalDate localDate)
        {
            ThrowsIfDateNotExists(localDate);
            var existingDay = this[localDate];
            Convention.Days.Remove(existingDay);
            Parent.DeletedEntityIds.Add(existingDay.Id);
            return Parent;
        }

        public ConventionBuilder AddDay(LocalDate date, LocalTime from, LocalTime to)
        {
            var day = new Day(date, from, to);
            ThrowsInvalidHours(day);
            ThrowsIfDateExists(day.Date);
            Convention.Days.Add(day);

            return Parent;
        }

        public ConventionBuilder SetTimeSlotStrategy(LocalDate localDate, TimeSlotStrategy strategy)
        {
            ThrowsIfDateNotExists(localDate);
            var existingDay = this[localDate];
            existingDay.TimeSlotStrategy = strategy;
            return Parent;
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


        private static void ThrowsInvalidHours(Day day)
        {
            if (day.StartTime >= day.EndTime)
            {
                throw new Exception("Start Time cannot be greater than End Time");
            }
        }

        public bool IsDaysExists(LocalDate day) => Parent.Days[day] != null;
        public List<Day> AllDays => Convention.Days.ToList();
    }
}