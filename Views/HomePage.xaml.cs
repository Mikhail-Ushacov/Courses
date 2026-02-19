using Courses.ViewModels;
using System;
using System.Windows.Controls;

namespace Courses.Views
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }
    }
}
