namespace Engine
{
    public interface IReusableBag<T>
    {
        public void Return(T t);
        public T Get();
    }
}