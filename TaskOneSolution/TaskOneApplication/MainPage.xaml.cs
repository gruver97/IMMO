using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using TaskOneApplication.Model;

namespace TaskOneApplication
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string ConfigurationName = "DefaultConfiguration";
        private readonly IScreenContentConfiguration _contentConfiguration = new ScreenContentConfiguration();

        private readonly List<ConfigurationModel> _defaultConfiguration = new List<ConfigurationModel>
        {
            new ConfigurationModel("Button1", ItemTypeEnum.ButtonItem, "Test"),
            new ConfigurationModel("Button0", ItemTypeEnum.ButtonItem, string.Empty),
            new ConfigurationModel("TextBlock1", ItemTypeEnum.TextBlockItem, "Test"),
            new ConfigurationModel("Image1", ItemTypeEnum.ImageItem,
                "http://s3.amazonaws.com/digitaltrends-uploads-prod/2014/02/Nokia-needs-to-escape-Windows-Phone.jpg"),
            new ConfigurationModel("Image0", ItemTypeEnum.ImageItem, string.Empty),
            new ConfigurationModel("Button2", ItemTypeEnum.ButtonItem, "Test test"),
            new ConfigurationModel("Image2", ItemTypeEnum.ImageItem,
                "http://www.computerra.ru/wp-content/uploads/2013/06/video-review-htcs-windows-phone-8x-300x224.jpg"),
            new ConfigurationModel("Button3", ItemTypeEnum.ButtonItem, "Test test test"),
            new ConfigurationModel("Image3", ItemTypeEnum.ImageItem,
                "http://static.trustedreviews.com/94/00002fa4e/9d67/Windows-Phone-8-Update-3.jpeg")
        };

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.
        ///     This parameter is typically used to configure the page.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Func<Task> emptyConfigurationAction = async () =>
            {
                var messageDialog = new MessageDialog("Пустая конфигурация");
                await messageDialog.ShowAsync();
            };
            await InitializeScreenConfigurationAsync();
            var screenConfiguration = await LoadConfigurationAsync();
            if (screenConfiguration == null)
            {
                await emptyConfigurationAction.Invoke();
            }
            var configurationModels = screenConfiguration as IList<ConfigurationModel> ?? screenConfiguration.ToList();
            if (!configurationModels.Any())
            {
                await emptyConfigurationAction.Invoke();
            }
            foreach (var configurationModel in configurationModels)
            {
                switch (configurationModel.ItemType)
                {
                    case ItemTypeEnum.TextBlockItem:
                        var block = CreateTextBlock(configurationModel);
                        LayoutStackPanel.Children.Add(block);
                        break;
                    case ItemTypeEnum.ButtonItem:
                        var button = CreateButton(configurationModel);
                        button.Tapped += (sender, eventArgs) =>
                        {
                            foreach (
                                var child in
                                    LayoutStackPanel.Children.Cast<FrameworkElement>()
                                        .Where(item => item.Name == "TextBlock1"))
                            {
                                if (child is TextBlock)
                                {
                                    var textBlock = child as TextBlock;
                                    textBlock.Text = "Don't stop me now.";
                                }
                            }
                        };
                        LayoutStackPanel.Children.Add(button);
                        break;
                    case ItemTypeEnum.ImageItem:
                        var border = await CreateImageAsync(configurationModel);
                        LayoutStackPanel.Children.Add(border);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private async Task<Border> CreateImageAsync(ConfigurationModel configurationModel)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Red),
                BorderThickness = new Thickness(3),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            var grid = new Grid();
            var progressRing = new ProgressRing() {IsActive = true, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center};
            grid.Children.Add(progressRing);
            border.Child = grid;
            if (!string.IsNullOrWhiteSpace(configurationModel.Content))
            {
                var bitmapImage = new BitmapImage();                              
                using (var imageStream = await DownloadImageAsync(new Uri(configurationModel.Content)))
                {
                    imageStream.Seek(0);
                    await bitmapImage.SetSourceAsync(imageStream);
                }
                var xamlImage = new Image
                {
                    Width = ScrollViewer.ViewportWidth,
                    Height = ScrollViewer.ViewportHeight,
                    Stretch = Stretch.Uniform,
                    Name = configurationModel.ItemName,
                    Source = bitmapImage
                };
                grid.Children.Add(xamlImage);
                bitmapImage.UriSource = new Uri(configurationModel.Content);
                bitmapImage.ImageOpened += (sender, eventArgs) =>
                {
                    progressRing.IsActive = false;
                };                                              
            }
            else
            {
                border.Visibility = Visibility.Collapsed;
            }
            return border;
        }

        private async Task<IRandomAccessStream> DownloadImageAsync(Uri imageUri)
        {
            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.GetAsync(imageUri);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                {
                    var inMemoryRandomAccessStream = new InMemoryRandomAccessStream();
                    await stream.CopyToAsync(inMemoryRandomAccessStream.AsStreamForWrite());
                    return inMemoryRandomAccessStream;
                }
            }
            else return null;
        }

        private Button CreateButton(ConfigurationModel configurationModel)
        {
            var button = new Button
            {
                Content = configurationModel.Content,
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Name = configurationModel.ItemName
            };
            if (string.IsNullOrWhiteSpace(configurationModel.Content)) button.Visibility = Visibility.Collapsed;
            return button;
        }

        private TextBlock CreateTextBlock(ConfigurationModel configurationModel)
        {
            var block = new TextBlock
            {
                Margin = new Thickness(0, 10, 0, 10),
                Text = configurationModel.Content,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 32,
                Name = configurationModel.ItemName
            };
            if (string.IsNullOrWhiteSpace(configurationModel.Content)) block.Visibility = Visibility.Collapsed;
            return block;
        }

        private async Task<IEnumerable<ConfigurationModel>> LoadConfigurationAsync()
        {
            try
            {
                return await _contentConfiguration.LoadConfigurationAsync(ConfigurationName);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        private async Task InitializeScreenConfigurationAsync()
        {
            try
            {
                await _contentConfiguration.SaveConfigurationAsync(_defaultConfiguration, ConfigurationName);
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}