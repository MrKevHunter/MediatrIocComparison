namespace MediatrOpGen
{
    public interface IPreRequestHandler<in TRequest>
    {
        void Handle(TRequest request);
    }
}