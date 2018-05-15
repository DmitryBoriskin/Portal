using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Авторизованный пользователь
    /// </summary>
    public class FrontUserModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Mail { get; set; }

        /// <summary>
        /// Соль для шифрования пароля
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Хэш для шифрования пароля
        /// </summary>
        public string Hash { get; set; }

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
        public string Patronymic { get; set; }

        /// <summary>
        /// Запрещён
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Дата блокировки
        /// </summary>
        public DateTime? LockDate { get; set; }

        /// <summary>
        /// Подтверждение регистрации
        /// </summary>
        public string VerificationCode { get; set; }

        /// <summary>
        /// Сброс пароля
        /// </summary>
        public string ResetPwdCode { get; set; }
    }
}

