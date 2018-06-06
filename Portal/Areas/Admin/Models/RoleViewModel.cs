using PgDbase.entity;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель пользователей для представления
    /// </summary>
    public class RoleViewModel : CoreViewModel
    {
        /// <summary>
        /// Постраничный список
        /// </summary>
        public RoleModel[] List { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public RoleModel Item { get; set; }

        /// <summary>
        /// Список модулей
        /// </summary>
        public ModuleModel[] Modules { get; set; }

    }
}