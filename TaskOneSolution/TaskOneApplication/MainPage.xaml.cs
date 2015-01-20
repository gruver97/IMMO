using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        private readonly List<ConfigurationModel> _defaultConfiguration = new List<ConfigurationModel>
        {
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test"),
            new ConfigurationModel(ItemTypeEnum.ButtonItem, "Test test test")
        };

        private readonly IScreenContentConfiguration _contentConfiguration = new ScreenContentConfiguration();


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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await InitializeScreenConfigurationAsync();
            await LoadConfigurationAsync();
        }

        private async Task LoadConfigurationAsync()
        {
            try
            {
               var screenConfiguration =  await _contentConfiguration.LoadConfigurationAsync(ConfigurationName);
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