using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DDT.Core.WidgetSystems.Bases.States;

/// <summary>
/// Singleton
/// </summary>
public partial class WidgetStateManager : ObservableObject
{
    #region Singleton
    private readonly static WidgetStateManager _instance = new WidgetStateManager();

    public static WidgetStateManager Instance
    {
        get => _instance;
    }
    #endregion

}
