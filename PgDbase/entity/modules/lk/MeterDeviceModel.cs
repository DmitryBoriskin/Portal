using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Прибор учёта
    /// </summary>
    public class MeterDeviceModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата установки
        /// </summary>
        public DateTime InstallDate { get; set; }

        /// <summary>
        /// Место установки
        /// </summary>
        public string InstallPlace { get; set; }

        /// <summary>
        /// Дата проверки
        /// </summary>
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool Disabled { get; set; }


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
        /// Разрядность
        /// </summary>
        public string Modification { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        public int? Tariff { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EnergyCategory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeviceCategory { get; set; }

        /// <summary>
        /// Трехфазный
        /// </summary>
        public bool Phase3 { get; set; }

        /// <summary>
        /// Производитель
        /// </summary>
        public string Manufactirer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PrecissionClass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string VoltageNominal { get; set; }

    }
}
