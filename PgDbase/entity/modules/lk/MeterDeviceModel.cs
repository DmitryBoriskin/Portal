using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Прибор учёта
    /// </summary>
    public class PuModel
    {
         /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Link { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid SubscrId { get; set; }

        /// <summary>
        /// лицевой счет линк из ОМНИС
        /// </summary>
        public long SubscrLink { get; set; }

        /// <summary>
        /// Идентификатор модели устройства
        /// </summary>
        public int? DeviceId { get; set; }


        /// <summary>
        /// Кроме приборов учета могут быть еще др. учетные показатели 
        /// </summary>
        public bool IsPu { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// наименование/описание пу
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Год выпуска
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Дата установки
        /// </summary>
        public DateTime? InstallDate { get; set; }

        /// <summary>
        /// Место установки
        /// </summary>
        public string InstallPlace { get; set; }

        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// Дата следующей проверки
        /// </summary>
        public DateTime? NextCheckDate { get; set; }

        /// <summary>
        ///  Заменить до
        /// </summary>
        public DateTime? ReplaceBeforeDate { get; set; }

        /// <summary>
        /// Дата гос. проверки
        /// </summary>
        public DateTime? ValidDate { get; set; }

        /// <summary>
        /// Тариф по уровню напряжения
        /// </summary>
        public string EnergyLvl { get; set; }

        /// <summary>
        /// Расчетный коэффициент, коэф. трансформации
        /// </summary>
        public decimal Multiplier { get; set; }

        /// <summary>
        /// Марка
        /// </summary>
        public DeviceModel DeviceInfo { get; set; }
    }


    public class DeviceModel
    {
  
        /// <summary>
        /// Место установки, др характ. параметры
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Разрядность счетчика 0,5S/12/8,4
        /// </summary>
        public string Modification { get; set; }

        /// <summary>
        /// Межповерочный интервал, лет
        /// </summary>
        public int? CheckCycle { get; set; }

        /// <summary>
        /// одно, двухтарифный и тп
        /// </summary>
        public int? Tariff { get; set; }

        /// <summary>
        /// Вид номенклатуры
        /// </summary>
        public string EnergyCategory { get; set; }

        /// <summary>
        /// Категория - Профильный счетчик(часы) ,Трансформатор тока и тп
        /// </summary>
        public string DeviceCategory { get; set; }

        /// <summary>
        /// фазность - трехфазный, да / нет
        /// </summary>
        public bool Phase3 { get; set; }

        /// <summary>
        /// Производитель
        /// </summary>
        public string Manufactirer { get; set; }

        /// <summary>
        /// Класс точности
        /// </summary>
        public string PrecissionClass { get; set; }

        /// <summary>
        /// Для счетчиков - Номинальное напряжение, В; для ТТ и ТН - Класс напряжения, кВ
        /// </summary>
        public string VoltageNominal { get; set; }

    }
}
