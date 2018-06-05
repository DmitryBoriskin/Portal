using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Email confirmed
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Mobile
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Mobile confirmed
        /// </summary>
        public bool PhoneConfirmed { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronimyc { get; set; }

        /// <summary>
        /// Запрещеность
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Др
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegDate { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{Surname} {Name} {Patronimyc}";
            }
        }

        /// <summary>
        /// Группа 
        /// </summary>
        public RoleModel[] Roles { get; set; }

        /// <summary>
        /// Группа 
        /// </summary>
        public RoleModel[] Sites { get; set; }

        /// <summary>
        /// Подключенные ЛС
        /// </summary>
        public Guid[] Subscrs { get; set; }
    }
}
