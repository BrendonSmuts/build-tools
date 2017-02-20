namespace SweetEditor.Build
{
    public interface IBuildSettings
    {
        string Id { get; }


        void Run();
    }
}