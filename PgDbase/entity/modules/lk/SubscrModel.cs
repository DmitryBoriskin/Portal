using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Лицевой счет
    /// </summary>
    public class SubscrShortModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Номер ID
        /// </summary>
        public long? SubscrId { get; set; }

        /// <summary>
        /// Название лиц счета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// По умолчанию
        /// </summary>
        public bool? Default { get; set; }

        /// <summary>
        /// Баланс 
        /// </summary>
        public Decimal? Saldo { get; set; }

        /// <summary>
        /// Пени
        /// </summary>
        public Decimal? Peni { get; set; }

        /// <summary>
        /// Проценты
        /// </summary>
        public Decimal? Percent { get; set; }

        /// <summary>
        /// На дату
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Последний платеж
        /// </summary>
        public Decimal? LastPaymentAmount { get; set; }

        /// <summary>
        /// Последний платеж
        /// </summary>
        public DateTime? LastPaymentDate { get; set; }

        /// <summary>
        /// Id Последний платеж
        /// </summary>
        public string LastPaymentLink { get; set; }
    }

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
        /// Номер в ОМНИС
        /// </summary>
        public long Link { get; set; }

        /// <summary>
        /// Номер лицевого счета
        /// </summary>
        public long Subscr { get; set; }

        /// <summary>
        /// дата открытия лс
        /// </summary>
        public DateTime? Begin { get; set; }

        /// <summary>
        /// дата закрытия лс
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// По умолчанию
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Отделение
        /// </summary>
        public Guid Department { get; set; }

        /// <summary>
        /// Флаг Юрлица
        /// </summary>
        public bool Ee { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        public string Kpp { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string NameShort { get; set; }


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
        /// Fax
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Сайт
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// Линк договора в ОМНИСЕ
        /// </summary>
        public long? ContractLink { get; set; }

        /// <summary>
        /// Номер договора и пр названия
        /// </summary>
        public string Contract { get; set; }

        /// <summary>
        /// Действителен с
        /// </summary>
        public DateTime? ContractBegin { get; set; }

        /// <summary>
        /// Действителен по
        /// </summary>
        public DateTime? ContractEnd { get; set; }

        /// <summary>
        /// Дата заключения договора
        /// </summary>
        public DateTime? ContractDate { get; set; }


        /// <summary>
        /// Банк
        /// </summary>
        public BankModel Bank { get; set; }

        /// <summary>
        /// Настройки лицевого счета
        /// </summary>
        public SubscrConfigs Configs { get; set; }
    }

    /// <summary>
    /// Банковские реквизиты
    /// </summary>
    public class BankModel
    {
        /// <summary>
        /// Наименование банка
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// БИК
        /// </summary>
        public string Bik { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }


        /// <summary>
        /// Расчетный счет
        /// </summary>
        public string Rs { get; set; }

    }




    /// <summary>
    /// Настройки лицевого счета
    /// </summary>
    public class SubscrConfigs
    {
        /// <summary>
        /// Ссылка на электронный документооборот
        /// </summary>
        public string EDO {get; set;}

        /// <summary>
        /// Персональный менеджер
        /// </summary>
        public SubscrManager Manager { get; set; }
    }

    /// <summary>
    /// Персональный менеджер
    /// </summary>
    public class SubscrManager
    {
        public Guid Id { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }
    }
 }
