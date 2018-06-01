using LinqToDB;
using PgDbase.entity;
using PgDbase.models;
using System;
using System.Linq;

namespace PgDbase.Repository.cms
{
    /// <summary>
    /// Репозиторий для работы с вакансиями
    /// </summary>
    public partial class CmsRepository
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
                    .Where(w => w.f_site == _siteId);

                if (filter.Disabled.HasValue)
                {
                    query = query.Where(w => w.b_disabled == filter.Disabled.Value);
                }
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

        /// <summary>
        /// Добавляет вакансию
        /// </summary>
        /// <param name="vacancy"></param>
        /// <returns></returns>
        public bool InsertVacancy(VacancyModel vacancy)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = vacancy.Id,
                        PageName = vacancy.Title,
                        Section = LogModule.Vacancies,
                        Action = LogAction.insert
                    };
                    InsertLog(log);

                    bool result = db.vacancy_vacancies.Insert(() => new vacancy_vacancies
                    {
                        id = vacancy.Id,
                        c_title = vacancy.Title,
                        c_text = vacancy.Text,
                        c_experience = vacancy.Experience,
                        c_salary = vacancy.Salary,
                        b_temporary = vacancy.IsTemporary,
                        d_date = vacancy.Date,
                        b_disabled = vacancy.IsDisabled,
                        f_site = _siteId
                    }) > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Обновляет вакансию
        /// </summary>
        /// <param name="vacancy"></param>
        /// <returns></returns>
        public bool UpdateVacancy(VacancyModel vacancy)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    var log = new LogModel
                    {
                        PageId = vacancy.Id,
                        PageName = vacancy.Title,
                        Section = LogModule.Vacancies,
                        Action = LogAction.update
                    };
                    InsertLog(log);

                    bool result = db.vacancy_vacancies
                        .Where(w => w.id == vacancy.Id)
                        .Set(s => s.c_title, vacancy.Title)
                        .Set(s => s.c_text, vacancy.Text)
                        .Set(s => s.c_experience, vacancy.Experience)
                        .Set(s => s.c_salary, vacancy.Salary)
                        .Set(s => s.b_temporary, vacancy.IsTemporary)
                        .Set(s => s.d_date, vacancy.Date)
                        .Set(s => s.b_disabled, vacancy.IsDisabled)
                        .Update() > 0;

                    tr.Commit();
                    return result;
                }
            }
        }

        /// <summary>
        /// Проверяет существование вакансии
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckVacancyExists(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                return db.vacancy_vacancies
                    .Where(w => w.id == id).Any();
            }
        }

        /// <summary>
        /// Удаление вакансии
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteVacancy(Guid id)
        {
            using (var db = new CMSdb(_context))
            {
                using (var tr = db.BeginTransaction())
                {
                    bool result = false;
                    var vac = db.vacancy_vacancies.Where(w => w.id == id).SingleOrDefault();
                    if (vac != null)
                    {
                        var log = new LogModel
                        {
                            PageId = id,
                            PageName = vac.c_title,
                            Section = LogModule.Vacancies,
                            Action = LogAction.delete
                        };
                        InsertLog(log, vac);

                        result = db.Delete(vac) > 0;
                    }

                    tr.Commit();
                    return result;
                }
            }
        }
    }
}
