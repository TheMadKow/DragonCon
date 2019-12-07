using System;
using System.Collections.Generic;
using System.Text;
using DragonCon.Modeling.Models.Conventions;
using DragonCon.Modeling.Models.HallsTables;

namespace DragonCon.Features.Management.Convention
{
    public class ConventionUpdateViewModel
    {
        public NameDatesCreateUpdateViewModel NameDate { get; set; }
        public TicketsUpdateViewModel Tickets { get; set; }
        public HallsUpdateViewModel Halls { get; set; }
        public DetailsUpdateViewModel Details { get; set; }
        public ConventionSettings Settings { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class DetailsUpdateViewModel
    {
        public Dictionary<string, string> Metadata { get; set; }
        public List<PhoneRecord> Phonebook { get; set; }
        public string ConventionId { get; set; }
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
