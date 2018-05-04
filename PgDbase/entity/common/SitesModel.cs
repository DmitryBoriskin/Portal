using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity
{
    /// <summary>
    /// Модель, описывающая список сайтов и пейджер
    /// </summary>
    public class SitesList
    {
        /// <summary>
        /// Список сайтов 
        /// </summary>
        public SitesModel[] Data;

        /// <summary>
        /// Пейджер
        /// </summary>
        public PagerModel Pager;

        public int CountAllSites;
        public int CountOrgSites;
        public int CountGsSites;
        public int CountEventSites;
    }

    /// <summary>
    /// Модель, описывающая сайт
    /// </summary>
    public class SitesModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        [Required(ErrorMessage = "Поле «Название» не должно быть пустым.")]
        public string Title { get; set; }

        /// <summary>
        /// Длинное название
        /// </summary>
        public string LongTitle { get; set; }

#warning Чем отличаются Disabled и SiteOff?   
        /// <summary>
        /// 
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Флаг отключенного сайта
        /// </summary>
        public bool SiteOff { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        //[Required(ErrorMessage = "Поле «Доменное имя» не должно быть пустым.")]
        //[RegularExpression(@"^[^-]([a-zA-Z0-9-]+)$", ErrorMessage = "Поле «Доменное имя» может содержать только буквы латинского алфавита и символ - (дефис). Доменное имя не может начинаться с дефиса.")]
        //public string Alias { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Тема
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// Идентификатор контента
        /// </summary>
        public Guid ContentId { get; set; }    

        /// <summary>
        /// Пользовательские скрипты
        /// </summary>
        public string Scripts { get; set; }

#warning Чем отличаются MainDomain и DefaultDomain?
        /// <summary>
        /// домен сайта отмечанный в списке его доменов
        /// </summary>
        public string MainDomain { get; set; }

        public string DefaultDomain { get; set; }

#warning Чем отличаются DomainList, DomainListString и DomainListArray?
        /// <summary>
        /// Список доменов
        /// </summary>
        public Domain[] DomainList { get; set; }

        /// <summary>
        /// Список дополнительных доменов в виде строки
        /// </summary>
        public string DomainListString { get; set; }

        /// <summary>
        /// Список дополнительных доменов
        /// </summary>
        public IEnumerable<string> DomainListArray { get; set; }

        /// <summary>
        /// Ссылки на соц сети
        /// </summary>
        public SocialShare SocialShareBtns { get; set; }

        public SiteModuleModel[] Modules { get; set; }
    }

    /// <summary>
    /// Сокращённая модель сайта
    /// </summary>
    public class SitesShortModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Флаг отключенного сайта
        /// </summary>
        public bool SiteOff { get; set; }
        /// <summary>
        /// Список доменов
        /// </summary>
        public Domain[] DomainList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Checked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Origin { get; set; }
    }

    /// <summary>
    /// Модель, описывающая домен
    /// </summary>
    public class Domain
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// Доменное имя
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Доменное имя является основным
        /// </summary>
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// Ссылки на соц сети
    /// </summary>
    public class SocialShare
    {
        /// <summary>
        /// facebook link
        /// </summary>
        public string Facebook { get; set; }

        /// <summary>
        /// vk link
        /// </summary>
        public string Vk { get; set; }

        /// <summary>
        /// instagramm link
        /// </summary>
        public string Instagramm { get; set; }

        /// <summary>
        /// odnoklassniki link
        /// </summary>
        public string Odnoklassniki { get; set; }

        /// <summary>
        /// twitter link
        /// </summary>
        public string Twitter { get; set; }
    }

    /// <summary>
    /// Модуль по отношению к сайту
    /// </summary>
    public class SiteModuleModel: ModuleModel
    {
        /// <summary>
        /// Id записи модуля конкретного сайта
        /// </summary>
        public Guid? SiteModuleId { get; set; }
        /// <summary>
        /// Модуль подключен к сайту (оплачен)
        /// </summary>
        public bool Checked { get; set; }

    }

}
