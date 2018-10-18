﻿using DragonCon.Logical.Convention;
using DragonCon.Modeling.Gateways;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;
using FakeItEasy;
using NodaTime;
using NUnit.Framework;

namespace DragonCon.Logical.Tests.ConventionBuilder
{
    [TestFixture]
    public class ConventionBuilderTests
    {
        private IConventionGateway GetGateway()
        {
            return A.Fake<IConventionGateway>();
        }

        [Test]
        public void ConventionBuilder_NewConvention_CorrectNameObject()
        {
            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Save();
        }

        [Test]
        public void ConventionBuilder_LoadConventionChangeName_Success()
        {
            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Save();

            builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .LoadConvention("Test Convention")
                .ChangeName("New Name");

            Assert.AreEqual(builder.ConventionName, "New Name");
        }


        [Test]
        public void ConventionBuilder_NewConventionManipulateDates_Success()
        {
            var builder = new Logical.Convention.ConventionBuilder(GetGateway());
            builder.NewConvention("Test Convention")
                .Days.AddDay(new LocalDate(2018, 7, 7), new LocalTime(9, 0), new LocalTime(23, 0))
                .Days.AddDay(new LocalDate(2018, 7, 8), new LocalTime(9, 0), new LocalTime(23, 0));
           
            Assert.AreEqual(builder.Days[new LocalDate(2018, 7, 7)].EndTime, new LocalTime(23, 0));
            Assert.AreEqual(builder.Days[new LocalDate(2018, 7, 7)].StartTime, new LocalTime(9, 0));

            builder.Days.UpdateDay(new LocalDate(2018, 7, 7), new LocalTime(11, 0), new LocalTime(22, 0))
                   .Days.RemoveDay(new LocalDate(2018, 7, 8));

            Assert.AreEqual(builder.Days[new LocalDate(2018, 7, 7)].EndTime, new LocalTime(22, 0));
            Assert.AreEqual(builder.Days[new LocalDate(2018, 7, 7)].StartTime, new LocalTime(11, 0));
            Assert.AreEqual(builder.Days[new LocalDate(2018, 7, 8)], null);

            builder.Days.SetTimeSlotStrategy(new LocalDate(2018, 7, 7), TimeSlotStrategy.Exact246Windows)
                .Save();

            Assert.AreEqual(builder.Days[new LocalDate(2018, 7, 7)].TimeSlotStrategy, TimeSlotStrategy.Exact246Windows);
        }

        [Test]
        public void ConventionBuilder_NewConventionManipulateHallsTables_Success()
        {
            var tables = HallsBuilder.RoomsFromNumericRange(1, 25);

            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Halls.AddHall("אולם השחקנים", "ללא תיאור")
                .Halls.AddHall("אולם ההרפתקנים", "ללא תיאור")
                .Halls.SetDescription("אולם השחקנים", "עם תיאור")
                .Halls.SetHallTables("אולם השחקנים", tables)
                .Halls.RenameHall("אולם השחקנים", "אולם המשחקים")
                .Halls.RemoveHall("אולם ההרפתקנים")
                .Save();

            var removedHall = builder.Halls["אולם ההרפתקנים"];
            Assert.IsNull(removedHall);
          
            var renamedHall = builder.Halls["אולם השחקנים"];
            Assert.IsNull(renamedHall);

            var hall = builder.Halls["אולם המשחקים"];
            Assert.AreEqual(hall.Name, "אולם המשחקים");
            Assert.AreEqual(hall.Description, "עם תיאור");
            Assert.AreEqual(hall.Tables.Count, 25);
        }


        [Test]
        public void ConventionBuilder_NewConventionManipulateTickets_Success()
        {
            var builder = new Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Days.AddDay(new LocalDate(2018, 7, 7), new LocalTime(9, 0), new LocalTime(23, 0))
                .Days.AddDay(new LocalDate(2018, 7, 8), new LocalTime(9, 0), new LocalTime(23, 0))
                .Tickets.AddTicket("AllDay", new LocalDate(2018, 7, 7), new LocalDate(2018, 7, 8))
                .Tickets.AddLimitedTicket(TicketLimitation.GameMaster, "GameMaster", new LocalDate(2018, 7, 7), new LocalDate(2018, 7, 8))
                .Tickets.AddLimitedTicket(TicketLimitation.Volunteer, "Volunteer", new LocalDate(2018, 7, 7), new LocalDate(2018, 7, 8))
                .Tickets.SetTransactionCode("AllDay", "Charge-Money")
                .Tickets.SetNumberOfActivities("AllDay", 5);
            
            Assert.NotNull(builder.Tickets["AllDay"]);
            Assert.NotNull(builder.Tickets["AllDay"].Days.Contains(builder.Days[new LocalDate(2018, 7, 7)]));
            Assert.NotNull(builder.Tickets["AllDay"].Days.Contains(builder.Days[new LocalDate(2018, 7, 8)]));
            Assert.NotNull(builder.Tickets["AllDay"].ActivitiesAllowed);
            Assert.AreEqual(builder.Tickets["AllDay"].ActivitiesAllowed.Value, 5);

            builder.Tickets.SetTicketPrice("AllDay", 70)
                .Tickets.SetTicketPrice("GameMaster", 0)
                .Tickets.SetTicketPrice("Volunteer", 0)
                .Tickets.SetUnlimitedActivities("AllDay")
                .Save();

            Assert.NotNull(builder.Tickets["GameMaster"]);
            Assert.NotNull(builder.Tickets["Volunteer"]);

            Assert.AreEqual(builder.Tickets["GameMaster"].TicketLimitation, TicketLimitation.GameMaster);
            Assert.AreEqual(builder.Tickets["Volunteer"].TicketLimitation, TicketLimitation.Volunteer);
            Assert.AreEqual(builder.Tickets["AllDay"].TicketLimitation, TicketLimitation.NotLimited);
  
            Assert.AreEqual(builder.Tickets["AllDay"].TransactionCode, "Charge-Money");
            Assert.AreEqual(builder.Tickets["AllDay"].IsUnlimited, true);
        }

        [Test]
        public void ConventionBuilder_MigrateToNewConvention_Success()
        {
            //TODO Move to Functional Tests
        }
    }
}
