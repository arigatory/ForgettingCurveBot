using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data.Lookups;
using ForgettingCurveBot.UI.Data.Repositories;
using ForgettingCurveBot.UI.Event;
using ForgettingCurveBot.UI.View.Services;
using ForgettingCurveBot.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class TelegramUserDetailViewModel : ViewModelBase, ITelegramUserDetailViewModel
    {
        private readonly ITelegramUserRepository _telegramUserRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private readonly INotificationIntervalsLookupDataService _notificationIntervalsLookupDataService;
        private TelegramUserWrapper _user;
        private bool _hasChanges;


        public TelegramUserDetailViewModel(ITelegramUserRepository telegramUserRepository,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            INotificationIntervalsLookupDataService notificationIntervalsLookupDataService)
        {
            _telegramUserRepository = telegramUserRepository;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _notificationIntervalsLookupDataService = notificationIntervalsLookupDataService;
            
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);

            NotificationIntervals = new ObservableCollection<LookupItem>();
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ObservableCollection<LookupItem> NotificationIntervals { get; }

        private async void OnDeleteExecute()
        {
            var result = _messageDialogService.ShowOkCancelDialog("Вы действительно хотите удалить пользователя?","Вопрос");
            if (result == MessageDialogResult.OK)
            {
                _telegramUserRepository.Remove(TelegramUser.Model);
                await _telegramUserRepository.SaveAsync();
                _eventAggregator.GetEvent<AfterTelegramUserDeletedEvent>().Publish(TelegramUser.Id);
            }
        }

        public async Task LoadAsync(long? userId)
        {
            var user = userId.HasValue
                ? await _telegramUserRepository.GetByIdAsync(userId.Value)
                : CreateNewTelegramUser();
            InitializeUser(user);

            await LoadNotificationIntervalsAsync();
        }

        private void InitializeUser(TelegramUser user)
        {
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

        private async Task LoadNotificationIntervalsAsync()
        {
            NotificationIntervals.Clear();
            NotificationIntervals.Add(new NullLookupItem { DisplayMember = " - "});
            var lookup = await _notificationIntervalsLookupDataService.GetNotificationIntervalsLookupAsync();
            foreach (var lookupItem in lookup)
            {
                NotificationIntervals.Add(lookupItem);
            }
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

        private ICommand banCommand;
        public ICommand BanCommand => banCommand ??= new DelegateCommand(Ban);

        private void Ban()
        {
        }
    }
}
