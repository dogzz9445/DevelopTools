using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DDT.Core.WidgetSystems.Contracts.Services
{
    public interface IApplicationService
    {
        Dispatcher Dispatcher { get; }
    }
}
