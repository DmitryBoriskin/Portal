using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Разрешения
    /// </summary>
    public class ResolutionModel
    {
        /// <summary>
        /// Чтение
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Запись
        /// </summary>
        public bool IsWrite { get; set; }

        /// <summary>
        /// Изменение
        /// </summary>
        public bool IsChange { get; set; }

        /// <summary>
        /// Удаление
        /// </summary>
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// Права на разделы
    /// </summary>
    public class ClaimParams
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Идентификатор меню
        /// </summary>
        public Guid MenuId { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string Claim { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
