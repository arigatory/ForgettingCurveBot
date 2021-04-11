using Prism.Events;

namespace ForgettingCurveBot.UI.Event
{
    public class AfterTelegramUserSavedEvent : PubSubEvent<AfterTelegramUserSavedEventArgs>
    {
    }

    public class AfterTelegramUserSavedEventArgs
    {
        public long Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
