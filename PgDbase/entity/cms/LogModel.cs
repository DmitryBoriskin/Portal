using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Лог
    /// </summary>
    public class LogModel
    {
        /// <summary>
        /// Пользователь
        /// </summary>
        public Guid User { get; set; }

        /// <summary>
        /// Страница
        /// </summary>
        public Guid Page { get; set; }

        /// <summary>
        /// Название страницы
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Секция
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// Ip-адрес
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Домен
        /// </summary>
        public Guid Site { get; set; }
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
