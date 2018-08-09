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
        /// Платежи json
        /// </summary>
        public string PaymentsByDateJson { get; set; }

        /// <summary>
        /// Начисления json
        /// </summary>
        public string InvoicesByDateJson { get; set; }

        /// <summary>
        /// Начисления и платежи (объединение) json
        /// </summary>
        public string InvoicesAndPaymentsByDateJson { get; set; }

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