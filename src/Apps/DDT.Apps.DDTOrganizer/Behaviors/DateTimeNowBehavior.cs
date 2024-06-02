// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// From: https://github.com/MahApps/MahApps.Metro/blob/develop/src/MahApps.Metro.Samples/MahApps.Metro.Demo/Behaviors/DateTimeNowBehavior.cs

using System;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using MahApps.Metro.Controls;

namespace DDT.Apps.DDTOrganizer.Behaviors;

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
