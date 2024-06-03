using CommunityToolkit.Mvvm.Input;
using DDT.Core.WidgetSystems.Contracts.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DDT.Apps.DDTOrganizer.Views;

public class UserConveter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        if (value is User user)
            return user.UserName;
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}

public class User : INotifyPropertyChanged
{
    private string _userName;
    public string UserName
    {
        get => _userName;
        set
        {
            if (_userName == value)
                return;

            _userName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserName)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class LoginViewModel : INotifyPropertyChanged
{
    private User _currentUser;
    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser == value)
                return;
            if (_currentUser != null)
                _currentUser.PropertyChanged -= RaisePropertyChanged;
            _currentUser = value;
            if (_currentUser != null)
                _currentUser.PropertyChanged += RaisePropertyChanged;
            RaisePropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentUser)));
        }
    }

    public void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        => PropertyChanged?.Invoke(sender, e);

    public event PropertyChangedEventHandler? PropertyChanged;

    [RelayCommand]
    public void SetRandomUserName() => CurrentUser.UserName = Guid.NewGuid().ToString();

    public LoginViewModel(IAuthService? authService) 
    {
        CurrentUser = new User();
    }
}

/// <summary>
/// Interaction logic for LoginView.xaml
/// </summary>
public partial class LoginView : Page
{
    public LoginViewModel ViewModel;
    public LoginView()
    {
        InitializeComponent();

        DataContext = ViewModel = new LoginViewModel(null);
    }
}
