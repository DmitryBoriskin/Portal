using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Расчетная ведомость
    /// </summary>
    public class PaysheetModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }


        /// <summary>
        /// Линк начисления в ОМНИС
        /// </summary>
        public long Link { get; set; }

        /// <summary>
        /// Лицевой счет
        /// </summary>
        public Guid SubscrId { get; set; }

        /// <summary>
        /// Лицевой счет
        /// </summary>
        public long SubscrLink { get; set; }

        /// <summary>
        /// Id Типа документа
        /// </summary>
        public short? DocTypeId { get; set; }
        /// <summary>
        /// Тип документа
        /// </summary>
        public string DocType { get; set; }

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
        /// Дата
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
        /// Номер документа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Период
        /// </summary>
        public int PeriodId { get; set; }

        /// <summary>
        /// Период в формате DateTime
        /// </summary>
        public DateTime Period {
            get {

                var str = PeriodId.ToString();
                var year = str.Substring(0, 4);
                var month = str.Substring(4, 1) == "0" ? str.Substring(5, 1) : str.Substring(4, 2);

                return new DateTime(int.Parse(year), int.Parse(month), 1, 0, 0, 0);
            }
        }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// налог
        /// </summary>
        public decimal? TaxAmount { get; set; }

        /// <summary>
        ///  Тарифицируемое потребление
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        ///  Тарифицируемое потребление2
        /// </summary>
        public decimal? Quantity2 { get; set; }

        /// <summary>
        ///  Суммарное потребление
        /// </summary>
        public decimal? Cons { get; set; }

        /// <summary>
        /// Ведомость расчета - детали
        /// </summary>
        public PaySheetDetailsModel Details { get; set; }

        /// <summary>
        /// Связанные Документы
        /// </summary>
        public DocumentModel[] Documents { get; set; }
    }

    /// <summary>
    /// Расчетная ведомость - детализация
    /// </summary>
    public class PaySheetDetailsModel
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
        public Guid? PaysheetId { get; set; }

        /// <summary>
        /// Линк на счет-фактуру из ОМНИС
        /// </summary>
        public long? PaysheetLInk { get; set; }

        /// <summary>
        /// пу
        /// </summary>
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// ПУ Линкв ОМНИС
        /// </summary>
        public long? DeviceLink { get; set; }

        /// <summary>
        /// ПУ, название точки учета
        /// </summary>
        public string DeviceName { get; set; }


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
        ///  Количество для тарификации
        /// </summary>
        public decimal? Cons { get; set; }

        /// <summary>
        ///  Расход по ПУ
        /// </summary>
        public decimal? MrCons { get; set; }

        /// <summary>
        /// Процент потребления
        /// </summary>
        public decimal? Persent { get; set; }

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
        /// налог в процентах
        /// </summary>
        public decimal? TaxPersent { get; set; }

        /// <summary>
        /// сумма по документу(с налогом)
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// метод расчета
        /// </summary>
        public string CalcMethod { get; set; }

        /// <summary>
        ///  измерение типа э/э(АЭ-)
        /// </summary>
        public short? EnergyTypeId { get; set; }

        /// <summary>
        /// измерение типа э/э(АЭ-)
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// Номинальное напряжение
        /// </summary>
        public string VoltageNominal { get; set; }

        /// <summary>
        /// Акт.Э/Э
        /// </summary>
        public string SaleItem { get; set; }

        /// <summary>
        /// Активная Электроэнергия
        /// </summary>
        public string SaleItemName { get; set; }

        /// <summary>
        /// Тариф по уровню напряжения
        /// </summary>
        public short? EnergyLvlId { get; set; }

        /// <summary>
        ///Тариф по уровню напряжения
        /// </summary>
        public string EnergyLvl { get; set; }

    }

}
