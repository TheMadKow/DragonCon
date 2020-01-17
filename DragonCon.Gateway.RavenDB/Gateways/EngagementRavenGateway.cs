using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DragonCon.Features.Management.Participants;
using DragonCon.Modeling.Helpers;
using DragonCon.Modeling.Models.Common;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.Identities;
using DragonCon.Modeling.Models.Payment;
using NodaTime;

namespace DragonCon.RavenDB.Gateways
{
    public class EngagementRavenGateway : RavenGateway
    {
        public EngagementRavenGateway(IServiceProvider provider) : base(provider)
        {
        }

        private Answer ValidateParticipantFields(ParticipantCreateUpdateViewModel viewmodel)
        {
            if (viewmodel.FullName.IsEmptyString())
                return Answer.Error("אין שם");

            if (viewmodel.PhoneNumber.IsEmptyString())
                return Answer.Error("אין טלפון");

            if (viewmodel.YearOfBirth < (DateTime.Today.Year - 120) ||
                                                   viewmodel.YearOfBirth > DateTime.Today.Year)
                return Answer.Error("תאריך לידה לא תקין");

            return Answer.Success;
        }


        public async Task<Answer> UpdateParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateParticipantFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;

            var participantId = viewmodel.Id;
            IParticipant participant = null;
            if (participantId.StartsWith("ShortTerm"))
            {
                participant = Session.Load<ShortTermParticipant>(participantId);
            }

            if (participantId.StartsWith("LongTerm"))
            {
                participant = Session.Load<LongTermParticipant>(participantId);
            }

            if (participant != null)
            {
                if (participant is LongTermParticipant longTerm)
                {
                    longTerm.IsAllowingPromotions = viewmodel.IsAllowingPromotions ?? false;
                }

                participant.FullName = viewmodel.FullName;
                participant.PhoneNumber = viewmodel.PhoneNumber ?? string.Empty;
                participant.YearOfBirth = viewmodel.YearOfBirth;

                Session.Store(participant, participant.Id);
                Session.SaveChanges();

                return await Task.FromResult(Answer.Success);
            }
            else
            {
                return await Task.FromResult(Answer.Error("Can't find participant"));
            }
        }

        public async Task<Answer> AddParticipant(ParticipantCreateUpdateViewModel viewmodel)
        {
            var answer = ValidateParticipantFields(viewmodel);
            if (answer.AnswerType != AnswerType.Success)
                return answer;


            IParticipant model;
            UserEngagement engagement = new UserEngagement
            {
                CreatedOn = SystemClock.Instance.GetCurrentInstant(),
                ConventionId = Actor.ManagedConvention.ConventionId,
                ConventionStartDate = Actor.ManagedConvention.Days
                    .Min(x => x.Date)
                    .ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Payment = new PaymentInvoice()
            };

            if (viewmodel.Email.IsEmptyString())
            {
                model = new ShortTermParticipant
                {
                    CreatedById = Actor.Me.Id
                };
            }
            else
            {
                model = new LongTermParticipant
                {
                    UserName = viewmodel.Email,
                    Email = viewmodel.Email,
                    IsAllowingPromotions = viewmodel.IsAllowingPromotions ?? false,
                };
                engagement.IsLongTerm = true;
            }

            model.FullName = viewmodel.FullName;
            model.PhoneNumber = viewmodel.PhoneNumber ?? string.Empty;
            model.YearOfBirth = viewmodel.YearOfBirth;

            var result = await Identities.AddNewParticipant(model);

            engagement.ParticipantId = model.Id;
            if (result.IsSuccess)
            {
                Session.Store(engagement);
                Session.SaveChanges();
            }

            if (result.IsSuccess && result.IsLongTerm)
            {
                await Hub.SendCreationPasswordAsync(model as LongTermParticipant, result.Token);
            }

            if (result.IsSuccess == false)
                return Answer.Error(result.Errors?.AsJson());

            return new Answer(AnswerType.Success)
            {
                Message = model.Id
            };
        }
    }
}
