using System;
using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// Пароль
    /// </summary>
    public class PasswordModel
    {
        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Поле Пароль» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет не правильный формат")]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required(ErrorMessage = "Поле «Подтверждение пароля» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Подтверждение пароля имеет не правильный формат")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public virtual string PasswordConfirm { get; set; }
    }
}
