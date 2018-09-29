namespace DragonCon.Modeling.Models.Common
{
    public abstract class Wrapper<T>
    {
        protected Wrapper(T model)
        {
            Model = model;
        }

        protected Wrapper()
        {

        }

        public T Model { get; protected set; }
    }
}