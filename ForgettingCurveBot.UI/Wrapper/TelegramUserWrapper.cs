using ForgettingCurveBot.Model;

namespace ForgettingCurveBot.UI.Wrapper
{

    public class TelegramUserWrapper : ModelWrapper<TelegramUser>
    {
        public TelegramUserWrapper(TelegramUser model) : base(model)
        {
        }

        public long Id { get { return Model.Id; } }
        public long TelegramIdentification { get { return Model.TelegramIdentification; } }


        public string Nickname
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }
    }
}
