using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.Event;
using ForgettingCurveBot.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class UserDetailViewModel : ViewModelBase, IUserDetailViewModel
    {
        private readonly ITelegramUserDataService _dataService;
        private readonly IEventAggregator _eventAggregator;
        private TelegramUserWrapper _user;

        public UserDetailViewModel(ITelegramUserDataService dataService,
            IEventAggregator eventAggregator)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenTelegramUserDetailViewEvent>()
                .Subscribe(OnOpenTelegramUserDetailView);

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(long userId)
        {
            var user = await _dataService.GetByIdAsync(userId);
            User = new TelegramUserWrapper(user);
        }

        public TelegramUserWrapper User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute()
        {
            //TODO: check valid
            return true;
        }

        private async void OnSaveExecute()
        {
            await _dataService.SaveAsync(User.Model);
            _eventAggregator.GetEvent<AfterTelegramUserSavedEvent>().Publish(
                new AfterTelegramUserSavedEventArgs
                {
                    Id = User.Id,
                    DisplayMember = $"{User.Nickname} {User.TelegramIdentification}"
                });
        }

        private async void OnOpenTelegramUserDetailView(long userId)
        {
            await LoadAsync(userId);
        }

    }
}
