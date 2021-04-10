using Autofac;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.ViewModel;

namespace ForgettingCurveBot.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<TelegramUserDataService>().As<ITelegramUserDataService>();

            return builder.Build();
        }
    }
}
