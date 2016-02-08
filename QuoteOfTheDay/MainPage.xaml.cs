using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Navigation;

namespace QuoteOfTheDay
{
    public sealed partial class MainPage : QuoteOfTheDay.Common.LayoutAwarePage
    {
        DataTransferManager dtm;
        QuoteOfTheDayVM vm;

        public MainPage()
        {
            this.InitializeComponent();
            vm = (QuoteOfTheDayVM)DataContext;
            vm.QuoteCompleted += Quote_Completed;
        }

        private void Quote_Completed(object sender, EventArgs e)
        {
            if (vm.Quote != null)
            {
                //Create the Large Tile
                var largeTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText09);
                largeTile.GetElementsByTagName("text")[0].InnerText = vm.Author;
                largeTile.GetElementsByTagName("text")[1].InnerText = vm.Quote;

                //Create a Small Tile
                var smallTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText04);
                smallTile.GetElementsByTagName("text")[0].InnerText = vm.Quote;

                //Merge the two updates into one <visual> XML node
                var newNode = largeTile.ImportNode(smallTile.GetElementsByTagName("binding").Item(0), true);
                largeTile.GetElementsByTagName("visual").Item(0).AppendChild(newNode);

                TileNotification notification = new TileNotification(largeTile);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
            }
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            dtm = DataTransferManager.GetForCurrentView();
            dtm.DataRequested += dtm_DataRequested;
            RegisterBackgroundTask();
        }

        private async void RegisterBackgroundTask()
        {
            string taskName = "Background";
            string taskEntryPoint = "BackgroundTask.Background";

            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(120, false));
                taskBuilder.Register();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            dtm.DataRequested -= dtm_DataRequested;
        }

        //Share information
        void dtm_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            string title = "Quote of the day";
            string content = "\"" + vm.Quote + "\" by " + vm.Author;

            DataPackage data = args.Request.Data;
            data.Properties.Title = title;
            data.SetText(content);
        }
    }
}
