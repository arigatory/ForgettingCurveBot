using ForgettingCurveBot.UI.Event;
using ForgettingCurveBot.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Func<ITelegramUserDetailViewModel> _telegramUserDetailViewModelCreator;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private ITelegramUserDetailViewModel _telegramUserDetailViewModel;


        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<ITelegramUserDetailViewModel> telegramUserDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            NavigationViewModel = navigationViewModel;
            _telegramUserDetailViewModelCreator = telegramUserDetailViewModelCreator;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenTelegramUserDetailViewEvent>()
                .Subscribe(OnOpenTelegramUserDetailView);
            _eventAggregator.GetEvent<AfterTelegramUserDeletedEvent>()
                .Subscribe(AfterTelegramUserDeleted);

            CreateNewTelegramUserCommand = new DelegateCommand(OnCreateNewTelegramUserExecute);

            NavigationViewModel = navigationViewModel;
        }


        public INavigationViewModel NavigationViewModel { get; }


        public ITelegramUserDetailViewModel TelegramUserDetailViewModel
        {
            get { return _telegramUserDetailViewModel; }
            private set
            {
                _telegramUserDetailViewModel = value;
                OnPropertyChanged();
            }
        }



        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewTelegramUserCommand { get; }


        private async void OnOpenTelegramUserDetailView(long? userId)
        {
            if (TelegramUserDetailViewModel!=null && TelegramUserDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("Были сделаны изменения, переключить пользователя?","Вопрос");
                if (result == MessageDialogResult.Cancel)
                {
                    return;
                }
            }
            TelegramUserDetailViewModel = _telegramUserDetailViewModelCreator();
            await _telegramUserDetailViewModel.LoadAsync(userId);
        }

        private void OnCreateNewTelegramUserExecute()
        {
            OnOpenTelegramUserDetailView(null);
        }


        private void AfterTelegramUserDeleted(long id)
        {
            TelegramUserDetailViewModel = null;
        }
    }
}
