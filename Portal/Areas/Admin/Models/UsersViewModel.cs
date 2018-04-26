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
        public UserModel User { get; set; }
    }
}