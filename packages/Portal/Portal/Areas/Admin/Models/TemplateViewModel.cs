using PgDbase.entity;


namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель пользователей для представления
    /// </summary>
    public class TemplateViewModel : CoreViewModel
    {
        /// <summary>
        /// Постраничный список
        /// </summary>
        public Paged<TemplateModel> List { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TemplateModel Item { get; set; }

        /// <summary>
        /// Модули
        /// </summary>
        public ModuleModel[] Modules { get; set; }

    }
}