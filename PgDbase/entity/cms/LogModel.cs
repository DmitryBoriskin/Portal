using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Лог
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// Id пользователя, внесшего изменения
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// IP пользователя, внесшего изменения
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Сайт
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Раздел сайта, в котором произошли изменения
        /// </summary>
        public LogModule Section { get; set; }

        /// <summary>
        /// Тип изменений
        /// </summary>
        public LogAction Action { get; set; }

        /// <summary>
        /// Id изменяемой записи
        /// </summary>
        public Guid PageId { get; set; }

        /// <summary>
        /// Текст, характеризующий изменяемую запись
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Comment { get; set; }
    }
    /// <summary>
    /// Действие для логирования
    /// </summary>
    public enum LogAction
    {
        /// <summary>
        /// Неопределено
        /// </summary>
        undefined = 0,

        /// <summary>
        /// Создание
        /// </summary>
        insert,

        /// <summary>
        /// Изменение
        /// </summary>
        update,

        /// <summary>
        /// Удаление
        /// </summary>
        delete,

        /// <summary>
        /// Восстановление
        /// </summary>
        recovery,

        /// <summary>
        /// Авторизация
        /// </summary>
        login,

        /// <summary>
        /// Выход из системы
        /// </summary>
        logoff,
        /// <summary>
        /// Неудачная попытка входа
        /// </summary>
        loginfailed,

        /// <summary>
        /// Блокировка аккаунта
        /// </summary>
        lockout,

        /// <summary>
        /// Изменение прав доступа
        /// </summary>
        resolutions,
    }

    /// <summary>
    /// Секция для логирования
    /// </summary>
    public enum LogModule
    {
        /// <summary>
        /// Неопределена
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Аккаунт
        /// </summary>
        Account = 1,

        /// <summary>
        /// Баннеры
        /// </summary>
        Banners = 2,

        /// <summary>
        /// структуры CMS
        /// </summary>
        CmsMenu = 3,

        /// <summary>
        /// Организации
        /// </summary>
        Orgs = 4,

        /// <summary>
        /// Пользователи портала
        /// </summary>
        PortalUsers = 5,

        /// <summary>
        /// Карта сайта
        /// </summary>
        SiteMap = 6,

        /// <summary>
        /// Сайты
        /// </summary>
        Sites = 7,

        /// <summary>
        /// Пользователи сайтов
        /// </summary>
        Users = 8,

        /// <summary>
        /// Материалы
        /// </summary>
        Materials = 9,

        /// <summary>
        /// События 
        /// </summary>
        Events = 10,

        /// <summary>
        /// Вакансии
        /// </summary>
        Vacancy = 11,

        /// <summary>
        /// Группа пользователей
        /// </summary>
        UserGroup = 12,

        /// <summary>
        /// Разделы сайта
        /// </summary>
        Module = 13,

        /// <summary>
        /// Голосование
        /// </summary>
        Vote = 14,

        /// <summary>
        /// Фотоальбомы
        /// </summary>
        PhotoAlbums = 15,

        /// <summary>
        /// Фотоальбомы
        /// </summary>
        Anketa = 16,

        /// <summary>
        /// новостная группа
        /// </summary>
        MaterialGroup = 17,

        /// <summary>
        /// Связь пользователя с сайтом
        /// </summary>
        UserSiteLinks = 18
    }

    /// <summary>
    /// Уровни логирования
    /// </summary>
    public enum LogLevelEnum
    {
        /// <summary>
        /// Вывод всего подряд. На тот случай, если Debug не позволяет локализовать ошибку. В нем полезно отмечать вызовы разнообразных блокирующих и асинхронных операций.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Журналирование моментов вызова «крупных» операций. Старт/остановка потока, запрос пользователя и т.п.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Разовые операции, которые повторяются крайне редко, но не регулярно. (загрузка конфига, плагина, запуск бэкапа)
        /// </summary>
        Info = 2,

        /// <summary>
        /// Неожиданные параметры вызова, странный формат запроса, использование дефолтных значений в замен не корректных. Вообще все, что может свидетельствовать о не штатном использовании.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Повод для внимания разработчика. Тут интересно окружение конкретного места ошибки.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Выводим все до чего дотянуться можем, так как дальше приложение работать не будет.
        /// </summary>
        Fatal = 5

    }

    public class DislyEventArgs
    {
        public LogLevelEnum EventLevel { get; private set; }
        public String Message { get; private set; }
        public Exception Exception { get; private set; }

        public DislyEventArgs(LogLevelEnum eventLevel, String message, Exception exception = null)
        {
            EventLevel = eventLevel;
            Message = message;
            Exception = exception;
        }

    }
    
}
