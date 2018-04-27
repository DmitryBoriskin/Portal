namespace PgDbase.entity
{
    /// <summary>
    /// Пейджер
    /// </summary>
    public class PagerModel
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int PageNum { get; set; } = 1;

        /// <summary>
        /// Кол-во эл-тов на странице
        /// </summary>
        public int PageSize { get; set; } = 15;

        /// <summary>
        /// Общее кол-во эл-тов
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Кол-во страниц
        /// </summary>
        public int PageCount {
            get
            {
                return (TotalCount / PageSize) + (TotalCount % PageSize > 0 ? 1 : 0);
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalCount"></param>
        public PagerModel(int pageSize, int pageNum, int totalCount)
        {
            PageSize = PageSize;
            PageNum = pageNum;
            TotalCount = totalCount;
        }
    }
}
