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
        Task SaveConfiguration(IEnumerable<ConfigurationModel> configuration, string configurationName);
        Task<IEnumerable<ConfigurationModel>> LoadConfiguration(string configurationName);
    }
}