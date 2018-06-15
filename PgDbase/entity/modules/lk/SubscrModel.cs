using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Личевой счёт
    /// </summary>
    public class SubscrModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public string Subscr { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Флаг Юрлица
        /// </summary>
        public bool Ee { get; set; }

        /// <summary>
        /// Название организации
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Адрес юридический
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Адрес почтовый
        /// </summary>
        public string PostAddress { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// По умолчанию
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Номер договора
        /// </summary>
        public string Contract { get; set; }

        /// <summary>
        /// Дата заключения договора
        /// </summary>
        public DateTime? ContractDate { get; set; }

        /// <summary>
        /// Действителен с
        /// </summary>
        public DateTime? Begin { get; set; }

        /// <summary>
        /// Действителен по
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Подразделение
        /// </summary>
        public Guid Department { get; set; }

        /// <summary>
        /// Банк
        /// </summary>
        public BankModel Bank { get; set; }
    }


    public class BankModel
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Код подразделения
        /// </summary>
        public string Dep { get; set; }

        /// <summary>
        /// БИК
        /// </summary>
        public string Bik { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        public string Kpp { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Кор счет
        /// </summary>
        public string Ks { get; set; }

        /// <summary>
        /// Расчетный счет
        /// </summary>
        public string Rs { get; set; }

    }
 }
