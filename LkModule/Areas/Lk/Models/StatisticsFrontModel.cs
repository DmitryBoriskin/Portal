﻿using PgDbase.entity;
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
        /// Начисления и платежи, баланс для главной страницы
        /// </summary>
        public Paged<DebitCreditModel> DebitCreditData { get; set; }

        /// <summary>
        /// Текущий баланс
        /// </summary>
        public SubscrShortModel Balance { get; set; }

    }
}