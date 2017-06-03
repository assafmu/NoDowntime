namespace NoDowntime
{
    public interface IHostService
    {
        void Initialize();
        void Recycle();
        void Stop();
        string GetName();
        bool InPlaceRecycling { get; set; }
    }
}