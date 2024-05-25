using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.WPF.Bases;

public interface IActionBarItem
{
    public string Title { get; }
    public string Icon { get; }
    public bool? Enabled { get; }
    public bool? Pressed { get; }
    public Func<Task> DoAction { get; }
}
