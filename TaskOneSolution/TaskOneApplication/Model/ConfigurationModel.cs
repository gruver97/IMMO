namespace TaskOneApplication.Model
{
    /// <summary>
    /// Элемент, отображаемый на экране
    /// </summary>
    public class ConfigurationModel
    {
        public ItemTypeEnum ItemType { get; set; }
        /// <summary>
        /// Для текстового контента - текст. Для изображения - ссылка.
        /// </summary>
        public string Content { get; set; }
    }
}
