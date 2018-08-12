using PgDbase.entity;
using Portal.Models;


namespace LkModule.Areas.Lk.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class PuFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<PuModel> List { get; set; }
        /// <summary>
        /// Прибор учета
        /// </summary>
        public PuModel Devices { get; set; }
        /// <summary>
        /// Показания прибора учета
        /// </summary>
        public Paged<MeterModel> DevicesMeter { get; set; }
        //public MeterModel[] DevicesMeter { get; set; }

        /// <summary>
        /// Данные по потреблению для построения графиков
        /// </summary>
        public string СonsumptionDataJson { get; set; }
    }
}