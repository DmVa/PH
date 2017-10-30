using PH.Config;
using PH.Data;
using PH.Win.ViewModels;
using PH.Win.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace PH.Win
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            var settings = ConfigurationManager.GetSection(PHConfigSection.CONFIG_SECTION_NAME) as PHConfigSection;
            DataService dataService = new DataService();
            dataService.Init(settings);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            MainWindowViewModel mainViewModel = new MainWindowViewModel();
            mainViewModel.Init(dataService);
            mainWindow.DataContext = mainViewModel;
            if (mainViewModel.LoadCommand.CanExecute(null))
                mainViewModel.LoadCommand.Execute(null);
        }

        private void Application_DispatcherUnhandledException(object sender,
                       System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            string errorMessage = string.Format("Error: {0}", ex == null ? "none" : ex.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
