using ForgettingCurveBot.Model;
using System.Text;
using System.Threading.Tasks;

namespace ForgettingCurveBot.UI.Wrapper
{
    public class TelegramUserWrapper : NotifyDataErrorInfoBase
    {
        public TelegramUserWrapper(TelegramUser model)
        {
            Model = model;
        }

        public TelegramUser Model { get; }

        public long Id { get { return Model.Id; } }
        public long TelegramIdentification { get { return Model.TelegramIdentification; } }


        public string Nickname
        {
            get { return Model.Nickname; }
            set
            {
                Model.Nickname = value;
                OnPropertyChanged();
            }
        }
    }
}
