﻿using System.Windows;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MainWindowViewModel ViewModel;

        public MainWindow(IScheduleRepository scheduleRepository)
        {
            ViewModel = new MainWindowViewModel(scheduleRepository);
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
