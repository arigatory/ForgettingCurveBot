using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.Data.Repositories;
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
    public class TelegramUserDetailViewModel : ViewModelBase, ITelegramUserDetailViewModel
    {
        private readonly ITelegramUserRepository _telegramUserRepository;
        private readonly IEventAggregator _eventAggregator;
        private TelegramUserWrapper _user;
        private bool _hasChanges;


        public TelegramUserDetailViewModel(ITelegramUserRepository telegramUserRepository,
            IEventAggregator eventAggregator)
        {
            _telegramUserRepository = telegramUserRepository;
            _eventAggregator = eventAggregator;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
        }

        public async Task LoadAsync(long? userId)
        {
            var user = userId.HasValue
                ? await _telegramUserRepository.GetByIdAsync(userId.Value)
                : CreateNewTelegramUser();
            TelegramUser = new TelegramUserWrapper(user);
            TelegramUser.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _telegramUserRepository.HasChanges();
                }
                //TODO: add raiseCanExecuteChanged() if has errors
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }


        public TelegramUserWrapper TelegramUser
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }


        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }


        public ICommand SaveCommand { get; }

        private bool OnSaveCanExecute()
        {
            // TODO: Check in addition if user has changes, and check validation
            return TelegramUser != null && HasChanges;
        }

        private async void OnSaveExecute()
        {
            await _telegramUserRepository.SaveAsync();
            HasChanges = _telegramUserRepository.HasChanges();
            _eventAggregator.GetEvent<AfterTelegramUserSavedEvent>().Publish(
                new AfterTelegramUserSavedEventArgs
                {
                    Id = TelegramUser.Id,
                    DisplayMember = $"{TelegramUser.Nickname} {TelegramUser.TelegramIdentification}"
                });
        }


        private TelegramUser CreateNewTelegramUser()
        {
            var user = new TelegramUser();
            _telegramUserRepository.Add(user);
            return user;
        }
    }
}
