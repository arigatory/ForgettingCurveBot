using ForgettingCurveBot.UI.Event;
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
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMember;

        public NavigationItemViewModel(long id, string displayMember,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            Id = id;
            DisplayMember = displayMember;
            OpenTeregramUserDetailViewCommand = new DelegateCommand(OnOpenTelegramUserDeatailView);
        }

        private IEventAggregator _eventAggregator;

        public long Id { get; }
        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenTeregramUserDetailViewCommand { get; }

        private void OnOpenTelegramUserDeatailView()
        {
            _eventAggregator.GetEvent<OpenTelegramUserDetailViewEvent>()
                       .Publish(Id);
        }
    }
}
