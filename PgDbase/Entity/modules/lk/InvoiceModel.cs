using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Выставленный счёт
    /// </summary>
    public class InvoiceModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Линк в ОМНИС
        /// </summary>
        public long Link { get; set; }

        /// <summary>
        /// Лицевой счет
        /// </summary>
        public Guid SubscrId { get; set; }

        /// <summary>
        /// Лицевой счет
        /// </summary>
        public long Subscr { get; set; }

        /// <summary>
        /// Дата регистрации документа
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///  дата: расчет с
        /// </summary>
        public DateTime? DateBegin { get; set; }

        /// <summary>
        /// дата: расчет по
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Ожидаемая дата оплаты
        /// </summary>
        public DateTime? DateDue { get; set; }

        /// <summary>
        /// Период
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Фактура полностью оплачена
        /// </summary>
        public bool Payed { get; set; }

        /// <summary>
        /// 1 - ДТ документ, 0 - КТ документ
        /// </summary>
        public bool Debit { get; set; }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public short? UnitId { get; set; }

        /// <summary>
        ///Единицы измерения
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// сумма по документу(с налогом)
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Налог
        /// </summary>
        public decimal? Tax { get; set; }

        /// <summary>
        /// Суммарное потребление
        /// </summary>
        public decimal? Cons { get; set; }

        /// <summary>
        /// Тарифицируемое потребление 1
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Тарифицируемое потребление 2
        /// </summary>
        public decimal? Quantity2 { get; set; }

        /// <summary>
        /// Id Статуса
        /// </summary>
        public short? SaleCategoryId { get; set; }

        /// <summary>
        /// договор
        /// </summary>
        public string SaleCategory { get; set; }


        /// <summary>
        /// Id Статуса
        /// </summary>
        public short? StatusId { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Id Типа документа
        /// </summary>
        public short? DocTypeId { get; set; }
        /// <summary>
        /// Тип документа
        /// </summary>
        public string DocType { get; set; }

        /// <summary>
        /// Id Типа задолженности
        /// </summary>
        public short? DebtTypeId { get; set; }
        /// <summary>
        /// Тип задолженности
        /// </summary>
        public string DebtType { get; set; }

        /// <summary>
        /// Ссылка на платеж (линк в ОМНИС)
        /// </summary>
        public long? PaysheetId { get; set; }

        /// <summary>
        /// Ссылка на расчетную ведомость (линк в ОМНИС)
        /// </summary>
        public long? PaymentId { get; set; }

        /// <summary>
        /// Номенклатура по  счет-фактуре
        /// </summary>
        public InvoiceDetailModel[] Details { get; set; }
    }

    public class InvoiceDetailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Линк в ОМНИС
        /// </summary>
        public long Link { get; set; }

        /// <summary>
        ///  дата: расчет с
        /// </summary>
        public DateTime? DateBegin { get; set; }

        /// <summary>
        /// дата: расчет по
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// период
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Линк на счет-фактуру
        /// </summary>
        public Guid? InvoiceId { get; set; }

        /// <summary>
        /// Линк на счет-фактуру из ОМНИС
        /// </summary>
        public long? InvoiceLInk { get; set; }

        /// <summary>
        /// Подоперация
        /// </summary>
        public short? SuboperationTypeId { get; set; }

        /// <summary>
        /// Подоперация - наименование
        /// </summary>
        public string SuboperationType { get; set; }

        /// <summary>
        /// Временная зона
        /// </summary>
        public short? TimeZoneId { get; set; }

        /// <summary>
        /// Временная зона
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Единицы измерения
        /// </summary>
        public short? UnitId { get; set; }

        /// <summary>
        ///Единицы измерения
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Группа строки СФ
        /// </summary>
        public short? InvoceGrp { get; set; }

        /// <summary>
        /// Группа строки ИС(начислений)
        /// </summary>
        public short? BillGrp { get; set; }

        /// <summary>
        /// тариф
        /// </summary>
        public short? TariffId { get; set; }

        /// <summary>
        /// тариф
        /// </summary>
        public string Tariff { get; set; }


        /// <summary>
        /// Количество для тарификации
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// ставка тарифа
        /// </summary>
        public decimal? TariffAmount { get; set; }

        /// <summary>
        /// налог
        /// </summary>
        public decimal? TaxAmount { get; set; }

        /// <summary>
        /// сумма по документу(с налогом)
        /// </summary>
        public decimal? Amount { get; set; }


        /// <summary>
        /// Количество для тарификации
        /// </summary>
        public decimal? Quantity0 { get; set; }

        /// <summary>
        /// ставка тарифа
        /// </summary>
        public decimal? TariffAmount0 { get; set; }

        /// <summary>
        /// налог
        /// </summary>
        public decimal? TaxAmount0 { get; set; }

        /// <summary>
        /// сумма по документу(с налогом)
        /// </summary>
        public decimal? Amount0 { get; set; }

        /// <summary>
        /// Количество для тарификации
        /// </summary>
        public decimal? Quantity1 { get; set; }

        /// <summary>
        /// ставка тарифа
        /// </summary>
        public decimal? TariffAmount1 { get; set; }

        /// <summary>
        /// налог
        /// </summary>
        public decimal? TaxAmount1 { get; set; }

        /// <summary>
        /// сумма по документу(с налогом)
        /// </summary>
        public decimal? Amount1 { get; set; }
    }
}
