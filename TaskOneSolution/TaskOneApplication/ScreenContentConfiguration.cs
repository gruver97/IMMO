using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using TaskOneApplication.Model;

namespace TaskOneApplication
{
    /// <summary>
    ///     Класс сохраняет и загружает конфигурацию содержимого экрана в хранилище приложения.
    /// </summary>
    public class ScreenContentConfiguration : IScreenContentConfiguration
    {
        private readonly StorageFolder _localStorageFolder = ApplicationData.Current.LocalFolder;

        /// <summary>
        ///     Сохраняет (с перезаписью) имеющуюся конфигурацю
        /// </summary>
        /// <returns></returns>
        public async Task SaveConfiguration(IEnumerable<ConfigurationModel> configuration, string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
                throw new ArgumentNullException("configurationName", "configurationName cannot be empty");
            try
            {
                var serializedConfiguration = JsonConvert.SerializeObject(configuration);
                var configurationFile =
                    await
                        _localStorageFolder.CreateFileAsync(configurationName, CreationCollisionOption.ReplaceExisting);
                using (IOutputStream writeStream = await configurationFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var buffer = CryptographicBuffer.ConvertStringToBinary(serializedConfiguration,
                        BinaryStringEncoding.Utf8);
                    await writeStream.WriteAsync(buffer);
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (JsonSerializationException)
            {
                throw;
            }
            catch (JsonReaderException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///     Загружает (при наличии) конфигурацию.
        /// </summary>
        /// <returns>Коллекция объектов конфигурации или null, в случае её отстутствия</returns>
        public async Task<IEnumerable<ConfigurationModel>> LoadConfiguration(string configurationName)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
                throw new ArgumentNullException("configurationName", "configurationName cannot be empty");
            try
            {
                var configurationFile =
                    await
                        _localStorageFolder.GetFileAsync(configurationName);
                using (var readStream = await configurationFile.OpenReadAsync())
                {
                    if (readStream.CanRead)
                    {
                        if (readStream.Size > uint.MaxValue) throw new IndexOutOfRangeException();
                        var buffer = new byte[readStream.Size];
                        await
                            readStream.ReadAsync(buffer.AsBuffer(), buffer.AsBuffer().Capacity, InputStreamOptions.None);
                        var deserializedString = Encoding.UTF8.GetString(buffer, 0, buffer.Count());
                        return JsonConvert.DeserializeObject<IEnumerable<ConfigurationModel>>(deserializedString);
                    }
                    return null;
                }
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (JsonSerializationException)
            {
                throw;
            }
            catch (JsonReaderException)
            {
                throw;
            }
        }
    }
}