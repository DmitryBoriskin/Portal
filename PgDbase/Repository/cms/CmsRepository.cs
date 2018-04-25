namespace PgDbase
{
    /// <summary>
    /// Репозиторий для работы с сущностями
    /// </summary>
    public partial class CmsRepository
    {
        /// <summary>
        /// Контекст подключения
        /// </summary>
        private string _context = null;

        /// <summary>
        /// Домен
        /// </summary>
        private string _domain = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        public CmsRepository()
        {
            _context = "dbConnection";
        }

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="domain">Домен</param>
        public CmsRepository(string domain)
        {
            _context = "dbConnection";
            _domain = domain;
        }
    }
}
