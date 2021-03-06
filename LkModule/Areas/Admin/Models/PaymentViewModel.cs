﻿using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class PaymentViewModel : CoreViewModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<PaymentModel> List { get; set; }
    }
}