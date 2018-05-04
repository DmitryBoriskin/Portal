namespace PgDbase.entity
{
    /// <summary>
    /// Разрешения
    /// </summary>
    public class Resolution
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
}
