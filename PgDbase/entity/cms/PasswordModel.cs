using System.ComponentModel.DataAnnotations;

namespace PgDbase.entity
{
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
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Пароль имеет неправильный формат")]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [Required(ErrorMessage = "Поле «Подтверждение пароля» не должно быть пустым.")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Длина пароля должна быть от 6 до 16 символов")]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{6,16}", ErrorMessage = "Подтверждение пароля имеет неправильный формат")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public virtual string PasswordConfirm { get; set; }
    }
}
