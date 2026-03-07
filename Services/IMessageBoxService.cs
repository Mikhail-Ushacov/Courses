namespace Courses.Services
{
    public interface IMessageBoxService
    {
        Task ShowMessageAsync(string title, string message);
    }
}
