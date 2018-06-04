﻿using PgDbase.entity;

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
        public Paged<UserModel> List { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public UserModel Item { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public PasswordModel Password { get; set; }

        /// <summary>
        /// Группы вообще все
        /// </summary>
        public RoleModel[] Roles { get; set; }

        /// <summary>
        /// Группы вообще все
        /// </summary>
        public RoleModel[] Sites { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterTreeModel Filter { get; set; }
    }
}