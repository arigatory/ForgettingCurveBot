using ForgettingCurveBot.Model;
using ForgettingCurveBot.UI.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private readonly IUserLookupDataService _userLookupDataService;

        public NavigationViewModel(IUserLookupDataService userLookupDataService)
        {
            _userLookupDataService = userLookupDataService;
        }

        public ObservableCollection<LookupItem> Users { get; } = new();

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
