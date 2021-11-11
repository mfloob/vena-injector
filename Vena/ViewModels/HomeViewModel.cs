using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Vena.Controls;
using Vena.Utilities;

namespace Vena.ViewModels;

internal class HomeViewModel : BaseViewModel
{
    private bool _shouldInject = false;

    public HomeViewModel()
    {
        Engine.Tick += OnTick;
        StartEngine(250);
    }

    private async void OnTick(object sender, EventArgs e)
    {
        if (IsInjected == Visibility.Visible)
        {
            await ShowInjected();
            return;
        }

        if (!_shouldInject)
            return;

        if (!File.Exists(_dllPath))
        {
            MessageBox.Show("DLL not found");
            _shouldInject = false;
            return;
        }

        if (_selectedProcess is null || _selectedProcess.HasExited)
        {
            MessageBox.Show("Process is null or exited");
            _shouldInject = false;
            SelectedProcess = null;
            return;
        }

        _shouldInject = false;
        IsLoading = Visibility.Visible;

        if (await Memory.Inject(_dllPath, _selectedProcess))
            IsInjected = Visibility.Visible;
        else
            MessageBox.Show("Failed to inject");

        IsLoading = Visibility.Hidden;
    }   

    private async Task ShowInjected()
    {
        await Task.Delay(new TimeSpan(0, 0, 4));
        IsInjected = Visibility.Hidden;
    }

    private string _dllPath = string.Empty;
    public string DllPath
    {
        get => _dllPath;
        set => Update(ref _dllPath, value);
    }

    private string _selectedProcessString = string.Empty;
    public string SelectedProcessString
    {
        get => _selectedProcessString;
        set => Update(ref _selectedProcessString, value);
    }

    private Process? _selectedProcess;
    public Process? SelectedProcess
    {
        get => _selectedProcess;
        set 
        { 
            Update(ref _selectedProcess, value);
            SelectedProcessString = value?.ProcessName ?? string.Empty;
        }
    }

    private Visibility _isInjected = Visibility.Hidden;
    public Visibility IsInjected
    {
        get => _isInjected;
        set => Update(ref _isInjected, value);
    }

    private Visibility _isLoading = Visibility.Hidden;
    public Visibility IsLoading
    {
        get => _isLoading;
        set => Update(ref _isLoading, value);
    }

    public ICommand SelectDllCommand => new Command(_ =>
    {
        var dialog = new OpenFileDialog();
        dialog.Title = "Select DLL";
        dialog.Filter = "Dll Files (*.dll)|*.dll;";
        if (dialog.ShowDialog() ?? false)
            DllPath = dialog.FileName;
    });

    public ICommand SelectProcessCommand => new Command(_ =>
    {
        var dialog = new ProcessWindow();
        if (dialog.ShowDialog() ?? false)
        {
            var process = (dialog.DataContext as ProcessViewModel)?.SelectedProcess;
            if (process?.HasExited ?? true)
            {
                MessageBox.Show($"{process?.ProcessName ?? "Process"} is null or exited");
                return;
            }
            SelectedProcess = process;
        }
    });

    public ICommand InjectCommand => new Command(_ =>
    {
        if (IsInjected == Visibility.Visible)
            return;

        _shouldInject = true;
    });
}
