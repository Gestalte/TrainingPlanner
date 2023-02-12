using System.Windows;

namespace TrainingPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MainWindowViewModel ViewModel;

        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
