using DataSourceProvider;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class Background : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            DataService ds = new DataService();
            var quote = await ds.GetQuoteOfTheDayAsync();

            //Create the Large Tile
            var largeTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText09);
            largeTile.GetElementsByTagName("text")[0].InnerText = quote.Author;
            largeTile.GetElementsByTagName("text")[1].InnerText = quote.Content;

            //Create a Small Tile
            var smallTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText04);
            smallTile.GetElementsByTagName("text")[0].InnerText = quote.Content;

            //Merge the two updates into one <visual> XML node
            var newNode = largeTile.ImportNode(smallTile.GetElementsByTagName("binding").Item(0), true);
            largeTile.GetElementsByTagName("visual").Item(0).AppendChild(newNode);

            TileNotification notification = new TileNotification(largeTile);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);

            deferral.Complete();
        }
    }
}
