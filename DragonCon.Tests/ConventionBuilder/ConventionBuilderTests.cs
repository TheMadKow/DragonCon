using System.Linq;
using DragonCon.Logical.Convention;
using DragonCon.Logical.Factories;
using DragonCon.Modeling.Gateways;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Tickets;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace DragonCon.Logic.Tests.ConventionBuilder
{
    [TestClass]
    public class ConventionBuilderTests
    {
        private IConventionGateway GetGateway()
        {
            return A.Fake<IConventionGateway>();
        }

        [TestMethod]
        public void ConventionBuilder_NewConvention_CorrectNameObject()
        {
            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Save();
            Assert.AreEqual(builder.GetConvention().Name, "Test Convention");
        }

        [TestMethod]
        public void ConventionBuilder_NewConventionManipulateDates_Success()
        {
            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Days.AddDay(new LocalDate(2018, 7, 7), new LocalTime(9, 0), new LocalTime(23, 0))
                .Days.AddDay(new LocalDate(2018, 7, 8), new LocalTime(9, 0), new LocalTime(23, 0))
                .Days.UpdateDay(new LocalDate(2018, 7, 7), new LocalTime(11, 0), new LocalTime(22, 0))
                .Days.RemoveDay(new LocalDate(2018, 7, 8))
                .Days.SetTimeSlotStrategy(new LocalDate(2018, 7, 7), TimeSlotStrategy.Exact246Windows)
                .Days.SetTimeSlotStrategy(new LocalDate(2018, 7, 8), TimeSlotStrategy.Exact246Windows)
                .Save();

            var onlyDay = builder.Days[new LocalDate(2018, 7, 7)];

            Assert.AreEqual(onlyDay.Date, new LocalDate(2018, 7, 7));
            Assert.AreEqual(onlyDay.EndTime, new LocalTime(22, 0));
            Assert.AreEqual(onlyDay.StartTime, new LocalTime(11, 0));
            Assert.AreEqual(onlyDay.TimeSlotStrategy, TimeSlotStrategy.Exact246Windows);
        }

        [TestMethod]
        public void ConventionBuilder_NewConventionManipulateHallsTables_Success()
        {
            var tables = HallsBuilder.RoomsFromNumericRange(1, 25);

            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Halls.AddHall("אולם השחקנים", "ללא תיאור")
                .Halls.SetDescription("אולם השחקנים", "עם תיאור")
                .Halls.SetHallTables("אולם השחקנים", tables)
                .Halls.RenameHall("אולם השחקנים", "אולם המשחקים")
                .Halls.RemoveHall("אולם השחקנים")
                .Halls.RemoveHall("אולם המשחקים")
                .Save();

            var hall = builder.Halls["אולם השחקנים"];

            Assert.AreEqual(hall.Name, "אולם השחקנים");
            Assert.AreEqual(hall.Description, "עם תיאור");
            Assert.AreEqual(hall.Tables.Count, 25);

        }


        [TestMethod]

        public void ConventionBuilder_NewConventionManipulateTickets_Success()
        {
            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewConvention("Test Convention")
                .Days.AddDay(new LocalDate(2018, 7, 7), new LocalTime(9, 0), new LocalTime(23, 0))
                .Days.AddDay(new LocalDate(2018, 7, 8), new LocalTime(9, 0), new LocalTime(23, 0))
                .Tickets.AddTicket("AllDay", new LocalDate(2018, 7, 7), new LocalDate(2018, 7, 8))
                .Tickets.AddLimitedTicket(TicketLimitation.GameMaster, "GameMaster", new LocalDate(2018, 7, 7), new LocalDate(2018, 7, 8))
                .Tickets.AddLimitedTicket(TicketLimitation.Volunteer, "Volunteer", new LocalDate(2018, 7, 7), new LocalDate(2018, 7, 8))
                .Tickets.SetTransactionCode("AllDay", "Charge-Money")
                .Tickets.SetNumberOfActivities("AllDay", 5)
                .Tickets.SetTicketPrice("AllDay", 70)
                .Tickets.SetUnlimitedActivities("AllDay")
                .Save();



            //    builder.Tickets.SetTicketName("AllDay", "New Name")
            //    .Tickets.SetTicketDays(new List<LocalDate>()
            //    {
            //        new LocalDate(2018, 7, 7),
            //    });



            //builder.Tickets.Remove("AllDay");
        }

        [TestMethod]
        public void ConventionBuilder_MigrateToNewConvention_Success()
        {
            var oldCon = new ConventionWrapper();

            var builder = new Logical.Convention.ConventionBuilder(GetGateway())
                .NewFromOldConvention("My New Con", oldCon,
                    Logical.Convention.ConventionBuilder.Migrate.Days,
                    Logical.Convention.ConventionBuilder.Migrate.Halls,
                    Logical.Convention.ConventionBuilder.Migrate.Tickets);

        }
    }
}
