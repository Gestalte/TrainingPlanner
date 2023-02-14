using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TrainingPlanner.Data;

namespace TrainingPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var dbContext = new TrainingDbContext();

            if (dbContext.Database.GetPendingMigrations().ToList().Count != 0)
            {
                dbContext.Database.Migrate();
            }

            IScheduleRepository scheduleRepository = new ScheduleRepository(dbContext);

            MainWindow mainWindow = new MainWindow(scheduleRepository);
            mainWindow.Show();
        }
    }
}
