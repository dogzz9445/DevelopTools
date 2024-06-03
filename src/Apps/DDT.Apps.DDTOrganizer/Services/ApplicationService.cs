using System;
using System.Collections.Generic;
using System.Windows.Threading;
using DDT.Core.WidgetSystems.Contracts.Services;

namespace DDT.Apps.DDTOrganizer.Services;

public class ApplicationService : IApplicationService
{
    public Dispatcher Dispatcher => App.Current.Dispatcher;
}
