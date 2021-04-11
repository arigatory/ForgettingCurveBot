using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class UserDetailViewModel : ViewModelBase, IUserDetailViewModel
    {
        private readonly ITelegramUserDataService _dataService;

        public UserDetailViewModel(ITelegramUserDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task LoadAsync(long userId)
        {
            User = await _dataService.GetByIdAsync(userId);
        }

        private TelegramUser _user;

        public TelegramUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

    }
}
