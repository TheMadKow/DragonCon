using System.Text;

namespace DragonCon.Modeling.Models.Common
{
    public class AgeGroup
    {
        public string Id { get; set; }
        public int? MinAge { get;set; }
        public int? MaxAge { get;set; }
        public string Name { get; set; }

        public string GetDescription()
        {
            var sb = new StringBuilder();
            sb.Append(Name);
            if (MinAge.HasValue && MaxAge.HasValue)
            {
                sb.Append($" ({MaxAge} - {MinAge})");
            }
            else if (MinAge.HasValue)
            {
                sb.Append($" (מ-{MinAge})");
            }
            else if (MaxAge.HasValue)
            {
                sb.Append($" (עד {MaxAge})");

            }

            return sb.ToString();
        }
    }
}