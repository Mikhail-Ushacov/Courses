using Courses.Services;
using System.Windows;
using Wpf.Ui.Appearance;

namespace Courses
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplicationThemeManager.ApplySystemTheme();

            var dbService = new DatabaseService();
            var initializer = new DatabaseInitializer(dbService.ConnectionString);
            var content = new ContentInitializer(dbService.ConnectionString);

            initializer.Initialize();
            content.SeedInitialData();
        }
    }
}
