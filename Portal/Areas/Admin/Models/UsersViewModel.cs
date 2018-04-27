using PgDbase.entity;
using PgDbase.entity.cms;
using PgDbase.Services;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель пользователей для представления
    /// </summary>
    public class UsersViewModel : CoreViewModel
    {
        /// <summary>
        /// Постраничный список
        /// </summary>
        public PagedEnumerable<UserModel> List { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public UserModel Item { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public PasswordModel Password { get; set; }

        /// <summary>
        /// Группы
        /// </summary>
        public GroupListModel[] Groups { get; set; }
    }
}