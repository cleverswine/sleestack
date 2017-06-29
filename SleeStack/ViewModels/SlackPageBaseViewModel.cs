using System;

namespace SleeStack.ViewModels
{
    public class SlackPageBaseViewModel : ViewModelBase
    {
        public SlackPageBaseViewModel()
        {
            BusyStart = () =>
            {
                var statbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statbar.ProgressIndicator.ProgressValue = null;
                //statbar.ProgressIndicator.Text = "Loading...";
                //statbar.BackgroundColor = new SolidColorBrush(Colors.Black).Color;
                //statbar.ForegroundColor = new SolidColorBrush(Colors.Black).Color;
                statbar.ProgressIndicator.ShowAsync();
            };

            BusyStop = () =>
            {
                var statbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statbar.ProgressIndicator.HideAsync();
            };
        }

        protected Action BusyStart { get; set; }
        protected Action BusyStop { get; set; }
    }
}