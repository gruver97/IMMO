using System.Collections.Generic;
using System.Threading.Tasks;
using TaskOneApplication.Model;

namespace TaskOneApplication
{
    /// <summary>
    /// Интерфейс, обеспечивающий работу с конфигурацией содержимого экрана
    /// </summary>
    public interface IScreenContentConfiguration
    {
        Task SaveConfigurationAsync(IEnumerable<ConfigurationModel> configuration, string configurationName);
        Task<IEnumerable<ConfigurationModel>> LoadConfigurationAsync(string configurationName);
    }
}