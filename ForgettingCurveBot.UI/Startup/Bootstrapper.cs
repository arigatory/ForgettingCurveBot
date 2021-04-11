using Autofac;
using ForgettingCurveBot.UI.Data;
using ForgettingCurveBot.UI.ViewModel;
using ForgetttingCurveBot.DataAccess;
using Prism.Events;

namespace ForgettingCurveBot.UI.Startup
{
    public class Bootstrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<ForgettingCurveBotDbContext>().AsSelf();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<UserDetailViewModel>().As<IUserDetailViewModel>();


            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<TelegramUserDataService>().As<ITelegramUserDataService>();

            return builder.Build();
        }
    }
}
