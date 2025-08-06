using SequenceDemo.Sequences.IndexPusher;
using SequenceDemo.Sequences.Plasma;
using SequenceDemo.Views;
using SequenceEngine.Bases;
using SequenceEngine.Manager;
using System.Reflection;
using System.Windows;
using VSLibrary.Common.MVVM.Core;

namespace SequenceDemo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        RegisterSequences();

        if (VSContainer.Instance.Resolve(typeof(MainWindow)) is MainWindow mainWindow)
        {
            mainWindow.Show();
        }
    }

    private void RegisterSequences()
    {
        var container = VSContainer.Instance;

        SequenceManager sequenceManager = new SequenceManager();
        sequenceManager.AddModule(new List<ISequenceModule> { new SeqIndexPusher(), new SeqPlasma() });

        container.RegisterInstance(sequenceManager);


        container.AutoInitialize(Assembly.GetExecutingAssembly());
        UIInitializer.RegisterServices(container);
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
        var sequence = VSContainer.Instance.Resolve<SequenceManager>();

        if(sequence != null)
        {
            sequence.Stop();
            sequence.Disposable();
        }    
    }
}
