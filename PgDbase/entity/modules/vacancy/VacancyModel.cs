using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Вакансия
    /// </summary>
    public class VacancyModel
    {        
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Опыт
        /// </summary>
        public string Experience { get; set; }

        /// <summary>
        /// Заработная плата
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// Временная
        /// </summary>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
