using System.Collections.Generic;

namespace DragonCon.Modeling.Models.Common
{
    public class Activity
    {
        public static Activity General => new GeneralActivity();

        public string Id { get; set; } = null;
        public string Name { get; set; } = string.Empty;
        
        public bool IsSubActivity { get; set; }
        public List<Activity> SubActivities { get; set; } = new List<Activity>();

        private class GeneralActivity : Activity
        {
            public GeneralActivity()
            {
                Name = "כללי";
                IsSubActivity = true;
            }
        }

    }
}
