using System;
using System.Collections.Generic;
using System.Text;

namespace DragonCon.Features.Convention.Home
{
    public class ContactUsViewModel
    {
        public bool IsEnglish { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
    }
}
