using Wpf.Ui.Controls;

namespace Courses.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public async Task ShowMessageAsync(string title, string message)
        {
            var messageBox = new MessageBox
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK"
            };
            await messageBox.ShowDialogAsync();
        }
    }
}
