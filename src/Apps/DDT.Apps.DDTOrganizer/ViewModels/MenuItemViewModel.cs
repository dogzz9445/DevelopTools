using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDT.Apps.DDTOrganizer.ViewModels;

public partial class MenuItemViewModel : ObservableObject
{
    public string Header { get; set; }

    public ObservableCollection<MenuItemViewModel> MenuItems { get; set; }

    [ObservableProperty]
    private ICommand _command;
}
