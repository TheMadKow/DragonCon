namespace DragonCon.Modeling.Models.Common
{
    public abstract class Wrapper<T>
    {
        protected Wrapper(T inner)
        {
            Inner = inner;
        }

        protected Wrapper()
        {

        }

        public T Inner { get; protected set; }
    }
}