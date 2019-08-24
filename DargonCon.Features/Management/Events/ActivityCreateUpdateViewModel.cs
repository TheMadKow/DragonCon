using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Features.Management.Events
{
    public class ActivityCreateUpdateViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לפעילות")]
        public string Name { get; set; }
        public List<SubActivityViewModel> SubActivities {get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SubActivityViewModel
    {
        public SubActivityViewModel(){}
        
        public SubActivityViewModel(Activity subActivity)
        {
            IsDeleted = false;

            Id = subActivity.Id;
            Name = subActivity.Name;
        }

        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לסוג הפעילות")]
        public string Name { get; set; }
        public bool IsDeleted {get; set; }
    }
}
