using Newtonsoft.Json;

namespace TaskOneApplication.Model
{
    /// <summary>
    ///     Элемент, отображаемый на экране
    /// </summary>
    public class ConfigurationModel
    {
        public ConfigurationModel(ItemTypeEnum itemType, string content)
        {
            ItemType = itemType;
            Content = content;
        }

        [JsonProperty("itemType")]
        public ItemTypeEnum ItemType { get; set; }

        /// <summary>
        ///     Для текстового контента - текст. Для изображения - ссылка.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}