using System.Collections.Generic;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;

namespace DragonCon.Features.Management.Convention
{
    public class ConventionUpdateViewModel
    {
        public NameDatesCreateUpdateViewModel NameDate { get; set; }
        public TicketsUpdateViewModel Tickets { get; set; }
        public HallsUpdateViewModel Halls { get; set; }
        public SettingsUpdateViewModel Settings { get; set; }
    }
    public class SettingsUpdateViewModel : ConventionSettings
    {
        public SettingsUpdateViewModel() { }
        public SettingsUpdateViewModel(string conId, ConventionSettings settings)
        {
            ConventionId = conId;
            AllowEventsSuggestions = settings.AllowEventsSuggestions;
            AllowEventsRegistration = settings.AllowEventsRegistration;
            AllowPayments = settings.AllowPayments;
            AllowManagementTests = settings.AllowManagementTests;
        }

        public string ConventionId { get; set; }

        public ConventionSettings CreateSettings()
        {
            return new ConventionSettings()
            {
                AllowEventsRegistration = AllowEventsRegistration,
                AllowEventsSuggestions = AllowEventsSuggestions,
                AllowManagementTests = AllowManagementTests,
                AllowPayments = AllowPayments
            };
        }
    }


    public class HallsUpdateViewModel{
        public List<HallViewModel> Halls { get; set; }
        public string ConventionId { get; set; }
    }
   
    public class HallViewModel : Hall
    {
        public HallViewModel()
        {

        }

        public HallViewModel(Hall value)
        {
            IsDeleted = false;
            FirstTable = value.FirstTable;
            LastTable = value.LastTable;
            Name = value.Name;
            Description = value.Description;
            Id = value.Id;
        }

        public bool IsDeleted { get; set; }
    }

}
