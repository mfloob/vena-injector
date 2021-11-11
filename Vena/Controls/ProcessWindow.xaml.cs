using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vena.ViewModels;

namespace Vena.Controls;

public partial class ProcessWindow : Window
{
    private ProcessViewModel _vm = new ProcessViewModel();
    public ProcessWindow()
    {
        InitializeComponent();
        DataContext = _vm;

        _vm.Processes = Process.GetProcesses().OrderBy(x => x.ProcessName);
    }

    private void ProcessInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var process = e?.AddedItems[0] as Process;
            if (process is null)
                return;

            _vm.SelectedProcess = process;
            _vm.SelectedProcessName = process.ProcessName;
        }
        catch { }
    }
}
