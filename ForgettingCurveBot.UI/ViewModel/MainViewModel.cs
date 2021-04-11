using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationViewModel navigationViewModel,
            IUserDetailViewModel userDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            UserDetailViewModel = userDetailViewModel;
        }

        public INavigationViewModel NavigationViewModel { get; }
        public IUserDetailViewModel UserDetailViewModel { get; }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }
    }
}
