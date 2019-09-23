using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DragonCon.Features.Management.Events
{
    public class AgeSystemCreateUpdateViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "חובה להזין שם להגבלת הגיל")]

        public string Name { get; set; }
        
        [Range(minimum:1, maximum:99, ErrorMessage = "גיל חייב להיות בין 1 ל-99")]
        public int? MinAge { get;set; }
        
        [Range(minimum:1, maximum:99, ErrorMessage = "גיל חייב להיות בין 1 ל-99")]
        public int? MaxAge { get;set; }
        
        public string ErrorMessage { get; set; }
    }
}
