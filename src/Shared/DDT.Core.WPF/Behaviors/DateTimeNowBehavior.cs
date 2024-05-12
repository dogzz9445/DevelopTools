using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using DateTimePicker = MahApps.Metro.Controls.DateTimePicker;

namespace DDT.Core.WPF.Behavior;

public class DateTimeNowBehavior : Behavior<DateTimePicker>
{
    private DispatcherTimer? _dispatcherTimer;

    protected override void OnAttached()
    {
        base.OnAttached();
        this._dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
                                                    DispatcherPriority.DataBind,
                                                    (sender, args) => this.AssociatedObject.SelectedDateTime = DateTime.Now,
                                                    Dispatcher.CurrentDispatcher);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        this._dispatcherTimer?.Stop();
    }
}
