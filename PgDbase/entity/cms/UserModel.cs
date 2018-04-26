using System;

namespace PgDbase.entity.cms
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
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Соль пароля
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Хэш-пароля
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Код для смены пароля
        /// </summary>
        public Guid ChangePassCode { get; set; }

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
        /// Кол-во неудачных попыток авторизации
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Последнее время авторизации
        /// </summary>
        public DateTime? TryLogin { get; set; }

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
    }
}
