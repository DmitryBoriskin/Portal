using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity.cms
{
    public class CmsMenuModel
    {
        public int Num { get; set; }
        /// <summary>
        /// Название группы меню
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Псевдоним
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CmsMenuItem[] GroupItems { get; set; }
    }
    public class CmsMenuItem
    {
        /// <summary>
        /// Id записи
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// Позиция в списке
        /// </summary>
        //[Required(ErrorMessage = "Поле «Сайт отключен» не должно быть пустым.")]
        public int Permit { get; set; }
        /// <summary>
        /// Сайт отключен
        /// </summary>
        [Required(ErrorMessage = "Поле «Псевдоним» не должно быть пустым.")]
        public string Alias { get; set; }
        /// <summary>
        /// Наименование пункта меню
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// В данное поле вписывается class элемента, в котором хранится символ из шрифта Fontello
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// Группа. Определяет, в каком разделе будет отображен данный пункт меню.
        /// </summary>
        [Required(ErrorMessage = "Поле «Сайт отключен» не должно быть пустым.")]
        public string Group { get; set; }
    }
}
