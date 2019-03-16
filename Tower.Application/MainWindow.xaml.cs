using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Tower.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Resets the alarm clock to view hours when re-opened
        private void AlarmOkButton_OnClick(object sender, RoutedEventArgs e)
        {
            AlarmClock.DisplayMode = ClockDisplayMode.Hours;
        }
    }
}
