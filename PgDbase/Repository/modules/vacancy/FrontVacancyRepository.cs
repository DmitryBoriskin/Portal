using PgDbase.entity;
using PgDbase.Vacancy.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Repository.front
{
    /// <summary>
    /// Репозиторий вакансий для работы во внешней части
    /// </summary>
    public partial class Repository
    {
        /// <summary>
        /// Возвращает постраничный список вакансий
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<VacancyModel> GetVacancies(FilterModel filter)
        {
            using (var db = new CMSdb(_context))
            {
                Paged<VacancyModel> result = new Paged<VacancyModel>();
                var query = db.vacancy_vacancies
                    .Where(w => !w.b_disabled);
                
                if (!String.IsNullOrWhiteSpace(filter.SearchText))
                {
                    string[] search = filter.SearchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (search != null && search.Count() > 0)
                    {
                        foreach (string p in filter.SearchText.Split(' '))
                        {
                            if (!String.IsNullOrWhiteSpace(p))
                            {
                                query = query.Where(w => w.c_title.Contains(p)
                                                      || w.c_text.Contains(p));
                            }
                        }
                    }
                }

                query = query.OrderByDescending(o => o.d_date);
                int itemsCount = query.Count();

                var list = query
                    .Skip(filter.Size * (filter.Page - 1))
                    .Take(filter.Size)
                    .Select(s => new VacancyModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Date = s.d_date,
                        IsDisabled = s.b_disabled,
                        Text = s.c_text
                    }).ToArray();

                return new Paged<VacancyModel>
                {
                    Items = list,
                    Pager = new PagerModel
                    {
                        PageNum = filter.Page,
                        PageSize = filter.Size,
                        TotalCount = itemsCount
                    }
                };
            }
        }

        /// <summary>
        /// Возвращает вакансию
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VacancyModel GetVacancy(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.vacancy_vacancies
                    .Where(w => w.id == id)
                    .Select(s => new VacancyModel
                    {
                        Id = s.id,
                        Title = s.c_title,
                        Text = s.c_text,
                        Experience = s.c_experience,
                        Salary = s.c_salary,
                        IsTemporary = s.b_temporary,
                        Date = s.d_date,
                        IsDisabled = s.b_disabled
                    }).SingleOrDefault();
            }
        }
    }
}
