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

        /// <summary>
        /// признак отключенности сайта
        /// </summary>
        public bool Disabled { get; set; }
        

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

        /// <summary>
        /// домен сайта отмечанный в списке его доменов
        /// </summary>        
        public string DefaultDomain { get; set; }

        /// <summary>
        /// Список доменов
        /// </summary>
        public Domain[] DomainList { get; set; }

        

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
        public Guid Id { get; set; }

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
        /// Id сайта
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Id модуля конкретного сайта
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Все доступные шаблоны для модуля
        /// </summary>
        public TemplateModel[] Templates { get; set; }

    }

}
