using PgDbase.entity;


namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель пользователей для представления
    /// </summary>
    public class ModuleViewModel : CoreViewModel
    {
        /// <summary>
        /// Постраничный список модулей
        /// </summary>
        public Paged<ModuleModel> List { get; set; }

        /// <summary>
        /// Модуль
        /// </summary>
        public ModuleModel Item { get; set; }

        /// <summary>
        /// Список доступных шаблонов
        /// </summary>
        public TemplateModel[] Templates { get; set; }

    }

}