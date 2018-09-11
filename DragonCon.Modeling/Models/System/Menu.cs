using System;
using System.Collections.Generic;

namespace DragonCon.Modeling.Models.System
{
    public class Menu
    {
        public string Text { get; set; }
        public int Order { get; set; }
        public bool IsExternalContent { get; set; }
        public Uri Hyperlink { get; set; }
        public List<Menu> Submenus { get; set; }
    }
}