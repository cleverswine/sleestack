using Autofac;
using Slack;
using Slack.Common;
using Slack.Interfaces;
using SleeStack.Common;
using SleeStack.Fakes;
using SleeStack.ViewModels;

namespace SleeStack
{
    public class ViewModelLocator
    {
        private readonly IContainer _container;

        public ViewModelLocator()
        {
            var containerBuilder = new ContainerBuilder();

            // Services
            containerBuilder.RegisterType<MemoryCache>().As<ICache>().SingleInstance();
            containerBuilder.RegisterType<SlackClient>().As<ISlackClient>().SingleInstance();
            containerBuilder.RegisterType<AppSettings>().As<ISettings>().SingleInstance();

            // ViewModels
            containerBuilder.RegisterType<HubPageViewModel>();
            containerBuilder.RegisterType<MessagesPageViewModel>();

            _container = containerBuilder.Build();

            //var t = _container.Resolve<ISettings>();
            //t.SlackApiAuthToken = "xoxp-2404162910-2405391908-2442253910-4a03af";
            //t.Save();
        }

        public HubPageViewModel HubPageViewModel
        {
            get
            {
                return _container.Resolve<HubPageViewModel>();
            }
        }

        public MessagesPageViewModel MessagesPageViewModel
        {
            get
            {
                return _container.Resolve<MessagesPageViewModel>();
            }
        }
    }
}