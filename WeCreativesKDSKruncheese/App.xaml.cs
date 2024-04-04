using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeCreatives_KDSPJ.Connections;
using WeCreatives_KDSPJ;

namespace WeCreativesKDSKruncheese
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceprovider;
        public static string KdcLoc => ConfigurationManager.AppSettings["KDCLOC"];
        public static string KdcName => ConfigurationManager.AppSettings["AppName"];

        public App()
        {
            ConfigureServices();
        }
        private void ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<StatusQuery>();
            services.AddSingleton<MainWindowVM>();
            services.AddTransient(s => new MainWindow
            {
                DataContext = s.GetRequiredService<MainWindowVM>()
            });

            _serviceprovider = services.BuildServiceProvider();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = _serviceprovider.GetRequiredService<MainWindow>();
            MainWindow.Show();
        }

    }
}
