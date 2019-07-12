using DragonCon.Modeling.Models.Conventions;

namespace DragonCon.Logical.Convention
{
    public abstract class BuilderBase<T>
    where T : class
    {
        protected BuilderBase(ConventionBuilder parent, ConventionWrapper convention)
        {
            Convention = convention;
            Parent = parent;
        }

        protected ConventionBuilder Parent { get; set; }
        protected ConventionWrapper Convention { get; set; }

        public T this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key))
                    return default;

                return Convention.GetById<T>(key);
            }
        }

        public bool KeyExists(string key)
        {
            return this[key] != null;
        }


    }
}
