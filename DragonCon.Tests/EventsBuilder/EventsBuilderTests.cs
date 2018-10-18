﻿using System.Collections.Generic;
using DragonCon.Logical.Requests;
using DragonCon.Modeling.Gateways;
using DragonCon.Modeling.Models.Common;
using FakeItEasy;
using NodaTime;
using NUnit.Framework;

namespace DragonCon.Logical.Tests.EventsBuilder
{
    [TestFixture]
    public class EventsBuilderTests
    {
        private IConventionGateway GetGateway()
        {
            return A.Fake<IConventionGateway>();
        }

        [Test]
        public void TestAddEvent_UserSuggestion_Success()
        {
            var eventRequest = new EventRequest()
            {
                UserId = "some@user.net",
                Name = "האירוע שלי",
                Description = "האירוע הכי מהמם בכנס",
                Requests = "אני זקוק לשולחן",
                DayId = "day-ID",
                Timeslide = new TimeSlot()
                {
                    From = new LocalTime(16, 00),
                    To = new LocalTime(18, 0)
                },
                EventActivityId = "Activity-ID",
                EventSystemId = "System-ID",
                //Restrictions = new EventRestriction()
                //{
                //    Age = new AgeRestriction(),
                //    MaxParticipants = 5,
                //    MinParticipants = 2
                //}, //TODO maybe restriction logic change
                Tags = new List<string>()
                {
                    "מהמם",
                    "פנטסיה"
                }
            };

            var conEvent = eventRequest.ToEvent();
            // TODO Asserts
        }

        public void TestUpdateEvent_ManagerChanges_Success()
        {
            //var ConEventVM = A.Fake<ConventionEventViewModel>();
        }
    }
}
