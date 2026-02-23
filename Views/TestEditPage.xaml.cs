using System.Windows;
using System.Windows.Controls;
using Courses.ViewModels;
using Courses.Services;

namespace Courses.Views
{
    public partial class TestEditPage : Page
    {
        public TestEditPage(int courseId, int? testId = null)
        {
            InitializeComponent();
            DataContext = new TestEditPageViewModel(courseId, testId);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AppNavigationService.GoBack();
        }

        private void RemoveQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is QuestionEditModel question)
            {
                var vm = DataContext as TestEditPageViewModel;
                vm?.Questions.Remove(question);
            }
        }

        private void AddOption_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is QuestionEditModel question)
            {
                question.Options.Add(new OptionEditModel { Text = "Нова відповідь" });
            }
        }

        private void RemoveOption_Click(object sender, RoutedEventArgs e)
        {
            // Шукаємо питання, якому належить ця відповідь
            var button = sender as Button;
            var option = button?.DataContext as OptionEditModel;

            if (option != null)
            {
                var vm = DataContext as TestEditPageViewModel;
                foreach (var q in vm.Questions)
                {
                    if (q.Options.Contains(option))
                    {
                        q.Options.Remove(option);
                        break;
                    }
                }
            }
        }
    }
}