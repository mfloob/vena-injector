using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Vena.Utilities;

namespace Vena.ViewModels;

internal class ProcessViewModel : BaseViewModel
{
    public ProcessViewModel()
    {
        Engine.Tick += OnTick;
        StartEngine();
    }

    private void OnTick(object sender, EventArgs e)
    {
        var processes = Process.GetProcesses().Where(x => x.ProcessName.Contains(_processFilter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        if (!AreEqual(processes.Select(x => x.Id), _processes.Select(x => x.Id)))
            Processes = processes.OrderBy(x => x.ProcessName);
    } 

    private IEnumerable<Process> _processes = new List<Process>();
    public IEnumerable<Process> Processes
    {
        get => _processes;
        set => Update(ref _processes, value);
    }

    private Process? _selectedProcess = null;
    public Process? SelectedProcess
    {
        get => _selectedProcess;
        set => Update(ref _selectedProcess, value);
    }

    private string _selectedProcessName = "Selected Process: ";
    public string SelectedProcessName
    {
        get => _selectedProcessName;
        set => Update(ref _selectedProcessName, $"Selected Process:     {value}");
    }

    private string _processFilter = string.Empty;
    public string ProcessFilter
    {
        get => _processFilter;
        set
        {
            if (value is null)
                value = string.Empty;
            Update(ref _processFilter, value);
        }
    }

    public ICommand SelectProcessCommand => new Command(x =>
    {
        var window = x as Window;
        if (window is null)
            return;

        if (_selectedProcess is null)
            window.DialogResult = false;

        window.DialogResult = true;
        window.Close();
    });


    private bool AreEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
    {
        var dictionary = a.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        foreach (var item in b)
        {
            int value;
            if (!dictionary.TryGetValue(item, out value))
            {
                return false;
            }
            if (value == 0)
            {
                return false;
            }
            dictionary[item] -= 1;
        }
        return dictionary.All(x => x.Value == 0);
    }
}
