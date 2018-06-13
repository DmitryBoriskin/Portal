using System;
using System.ComponentModel.DataAnnotations;

namespace PgDbase.entity
{
    public class CmsMenuItemModel
    {
        /// <summary>
        /// Id записи
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Родительский Id
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// Позиция в списке
        /// </summary>
        //[Required(ErrorMessage = "Поле «Сайт отключен» не должно быть пустым.")]
        public int Sort { get; set; }
        /// <summary>
        /// Сайт отключен
        /// </summary>
        [Required(ErrorMessage = "Поле «Псевдоним» не должно быть пустым.")]
        public string Alias { get; set; }
        /// <summary>
        /// Наименование пункта меню
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Name { get; set; }
        /// <summary>
        /// В данное поле вписывается class элемента, в котором хранится символ из шрифта Fontello
        /// </summary>
        public string Icon { get; set; }
        ///// <summary>
        ///// Группа. Определяет, в каком разделе будет отображен данный пункт меню.
        ///// </summary>

        /// <summary>
        /// Показывать в меню
        /// </summary>
        public bool ShowInMenu { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CmsMenuItemModel[] Childs { get; set; }
    }
}
