using System.Windows;

namespace Tower.Application.Views.Music
{
    /// <summary>
    /// Interaction logic for MusicCurrentMediaProgressBarView.xaml
    /// </summary>
    public partial class MusicCurrentMediaProgressBarView
    {
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register("Percent",
            typeof(double), typeof(MusicCurrentMediaProgressBarView), new PropertyMetadata(PercentChangedCallback));

        private static void PercentChangedCallback(object sender, DependencyPropertyChangedEventArgs e) =>
            ((MusicCurrentMediaProgressBarView)sender).PercentChanged(e.NewValue);

        public double Percent
        {
            get => (double)GetValue(PercentProperty);
            set => SetValue(PercentProperty, value);
        }

        private void PercentChanged(object parameter)
        {
            if (parameter is double percent)
                Width = (percent / 100) * MaxWidth;
        }

        public MusicCurrentMediaProgressBarView()
        {
            InitializeComponent();
        }
    }
}
