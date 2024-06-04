using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Bases.Factories
{
    public interface IWidgetFactory<T>
    {
        Func<T> Widget { get; }
    }
}
