using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDT.Core.WidgetSystems.Controls;

public partial class WidgetOptionBase : ObservableRecipient
{
    [ObservableProperty]
    private string _name;
}
