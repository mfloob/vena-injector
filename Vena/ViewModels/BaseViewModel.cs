using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Vena.Utilities;

namespace Vena.ViewModels;

internal abstract class BaseViewModel : Presenter
{
    public DispatcherTimer Engine { get; private set; } = new DispatcherTimer();

    public void StartEngine(int interval = 100)
    {
        Engine.Interval = new TimeSpan(0, 0, 0, 0, interval);
        Engine.Start();
    }
}
