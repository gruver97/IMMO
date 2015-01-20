using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem,
                "Test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem,
                "Test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.TextBlockItem, "Test test test")
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
                        LayoutStackPanel.Children.Add(new TextBlock
                        {
                            Margin = new Thickness(0, 10, 0, 10),
                            Text = configurationModel.Content
                        });
                        break;
                    case ItemTypeEnum.ButtonItem:
                        LayoutStackPanel.Children.Add(new Button
                        {
                            Content = configurationModel.Content,
                            Margin = new Thickness(0, 10, 0, 10)
                        });
                        break;
                    case ItemTypeEnum.ImageItem:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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