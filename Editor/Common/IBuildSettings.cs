namespace Sweet.BuildTools.Editor
{
    public interface IBuildSettings
    {
        string Id { get; }


        void Run();
    }
}