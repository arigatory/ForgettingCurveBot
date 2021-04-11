using ForgettingCurveBot.UI.Event;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Func<ITelegramUserDetailViewModel> _telegramUserDetailViewModelCreator;
        private readonly IEventAggregator _eventAggregator;
        private ITelegramUserDetailViewModel _telegramUserDetailViewModel;


        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<ITelegramUserDetailViewModel> telegramUserDetailViewModelCreator,
            IEventAggregator eventAggregator)
        {
            NavigationViewModel = navigationViewModel;
            _telegramUserDetailViewModelCreator = telegramUserDetailViewModelCreator;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenTelegramUserDetailViewEvent>()
                .Subscribe(OnOpenTelegramUserDetailView);

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

        private async void OnOpenTelegramUserDetailView(long userId)
        {
            if (TelegramUserDetailViewModel!=null && TelegramUserDetailViewModel.HasChanges)
            {
                var result = MessageBox.Show("Были сделаны изменения, переключить пользователя?","Вопрос", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            TelegramUserDetailViewModel = _telegramUserDetailViewModelCreator();
            await _telegramUserDetailViewModel.LoadAsync(userId);
        }
    }
}
