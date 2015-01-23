namespace FixedThreadPool
{
    public interface IFixedThreadPool
    {
        bool Execute(ITask task, Priority priority);
        void Stop();
    }
}