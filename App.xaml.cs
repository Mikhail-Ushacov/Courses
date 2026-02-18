using Courses.Services;
using System.Windows;

namespace Courses
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbService = new DatabaseService();
            var initializer = new DatabaseInitializer(dbService.ConnectionString);
            var content = new ContentInitializer(dbService.ConnectionString);

            initializer.Initialize();
            content.SeedInitialData();
        }
    }
}