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
    [ObservableProperty]
    private string _header;

    [ObservableProperty]
    private ICommand _command;

    [ObservableProperty]
    private ObservableCollection<MenuItemViewModel> _menuItems;
}
