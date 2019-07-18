using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DragonCon.Modeling.Models.Common;

namespace DragonCon.Features.Management.Events
{
    public class ActivitySystemCreateUpdateViewModel
    {
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לפעילות")]
        public string Name { get; set; }
        public List<SystemViewModel> Systems {get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SystemViewModel
    {
        public SystemViewModel(){}
        
        public SystemViewModel(EventSystem system)
        {
            IsDeleted = false;

            Id = system.Id;
            Name = system.Name;
        }

        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם לשיטה")]
        public string Name { get; set; }
        public bool IsDeleted {get; set; }
    }
}
