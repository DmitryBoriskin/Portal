using PgDbase.entity;
using Portal.Models;
using System.Collections.Generic;

namespace LkModule.Areas.Lk.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class StatisticsFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Платежи Список 
        /// </summary>
        public List<PaymentModel> Payments { get; set; }

        /// <summary>
        /// Платежи json
        /// </summary>
        public string PaymentsByDateJson { get; set; }

        /// <summary>
        ///  Начисления Список 
        /// </summary>
        public List<InvoiceModel> Accruals { get; set; }

        /// <summary>
        /// Начисления json
        /// </summary>
        public string AccrualsByDateJson { get; set; }

        /// <summary>
        ///  Потребление Список 
        /// </summary>
        public List<InvoiceModel> Consumption { get; set; }


        /// <summary>
        /// Потребление json
        /// </summary>
        public string ConsumptionByDateJson { get; set; }

    }
}