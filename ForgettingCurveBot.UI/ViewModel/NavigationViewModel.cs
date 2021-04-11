using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.Data.Lookups;
using ForgettingCurveBot.UI.Event;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly IUserLookupDataService _userLookupDataService;
        private readonly IEventAggregator _eventAggregator;

        public NavigationViewModel(IUserLookupDataService userLookupDataService,
            IEventAggregator eventAggregator)
        {
            _userLookupDataService = userLookupDataService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AfterTelegramUserSavedEvent>().Subscribe(AfterTelegramUserSaved);
        }

        private void AfterTelegramUserSaved(AfterTelegramUserSavedEventArgs obj)
        {
            var lookupItem = Users.Single(l => l.Id == obj.Id);
            lookupItem.DisplayMember = obj.DisplayMember;
        }

        public ObservableCollection<NavigationItemViewModel> Users { get; } = new();

        private NavigationItemViewModel _selectedUser;

        public NavigationItemViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                if (_selectedUser!=null)
                {
                    _eventAggregator.GetEvent<OpenTelegramUserDetailViewEvent>()
                        .Publish(_selectedUser.Id);
                }
            }
        }


        public async Task LoadAsync()
        {
            var lookup = await _userLookupDataService.GetUsersLookupAsync();
            Users.Clear();
            foreach (var item in lookup)
            {
                Users.Add(new NavigationItemViewModel(item.Id, item.DisplayMember));
            }
        }
    }
}
