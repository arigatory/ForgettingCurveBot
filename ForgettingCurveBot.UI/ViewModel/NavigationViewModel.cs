using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
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
        }

        public ObservableCollection<LookupItem> Users { get; } = new();

        private LookupItem _selectedUser;

        public LookupItem SelectedUser
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
                Users.Add(item);
            }
        }
    }
}
