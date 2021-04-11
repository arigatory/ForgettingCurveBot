using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.Event;
using Prism.Events;
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
        private readonly IEventAggregator _eventAggregator;

        public UserDetailViewModel(ITelegramUserDataService dataService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenTelegramUserDetailViewEvent>()
                .Subscribe(OnOpenTelegramUserDetailView);
        }

        private async void OnOpenTelegramUserDetailView(long userId)
        {
            await LoadAsync(userId);
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
