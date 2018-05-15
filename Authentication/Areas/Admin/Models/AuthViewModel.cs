using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System.Web.Mvc;

namespace Authentication.Admin.Models
{
    /// <summary>
    /// Модель пользователей для представления
    /// </summary>
    public class AuthViewModel : CoreViewModel
    {
        /// <summary>
        /// Пользователи внешней части
        /// </summary>
        public Paged<FrontUserModel> List { get; set; }

        /// <summary>
        ///  Пользователь внешней части
        /// </summary>
        public FrontUserModel Item { get; set; }

    }

}