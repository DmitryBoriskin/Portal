using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class SettingsFrontModel:LayoutViewModel
    {
        public UserModel UserModel { get; set; }
        //public PageModel Page { get; set; }
        //public List<PageModel> PageGroup { get; set; }
    }

    public class ChangePasswordFrontModel
    {
        [Required(ErrorMessage = "Введите старый пароль.")]
        [DataType(DataType.Password)]
        [Display(Name = "Старый пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [StringLength(100, ErrorMessage = "Длина пароля должна быть хотя бы {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль повторно.")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторите")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и старый пароль не должны совпадать.")]
        public string ConfirmPassword { get; set; }
    }
}