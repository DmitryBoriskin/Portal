using PgDbase.entity;


namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель пользователей для представления
    /// </summary>
    public class ModuleViewModel : CoreViewModel
    {
        /// <summary>
        /// Постраничный список
        /// </summary>
        public Paged<ModuleModel> List { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public ModuleModel Item { get; set; }

    }
}