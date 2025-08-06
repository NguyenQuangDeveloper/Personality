using ChamberControl;
using ChamberDemo2.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChamberDemo2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ChamberViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<TestViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<TestWindow>();
            ServiceProvider = services.BuildServiceProvider();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            var testWindow = ServiceProvider.GetRequiredService<TestWindow>();
            testWindow.Show();
        }
    }
}
