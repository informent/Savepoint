using System.Windows;
namespace Savepoint;
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if (AnalysisCommand.IsAnalysisCommand(e.Args))
        {
            try { AnalysisCommand.Run(e.Args); Shutdown(0); }
            catch (Exception) { Shutdown(2); }
            return;
        }
        MainWindow = new MainWindow(); MainWindow.Show();
    }
}
