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
        private readonly ITelegramUserDataService _telegramUserDataService;
        private TelegramUser _selectedUser;


        public MainViewModel(ITelegramUserDataService telegramUserDataService)
        {
            _telegramUserDataService = telegramUserDataService;
        }

        public ObservableCollection<TelegramUser> Users { get; set; } = new();




        public TelegramUser SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }


        public void Load()
        {
            var users = _telegramUserDataService.GetAll();
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }
    }
}
