﻿using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления лицевого счёта
    /// </summary>
    public class SubscrViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<SubscrModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public SubscrModel Item { get; set; }

        /// <summary>
        /// Подразделения
        /// </summary>
        public IEnumerable<SelectListItem> Departments { get; set; }

        /// <summary>
        /// Список лицевых счетов
        /// </summary>
        public SubscrModel[] Subscrs { get; set; }

        /// <summary>
        /// Список выбранных ЛС
        /// </summary>
        public SubscrModel[] SelectedSubscrs { get; set; }

        /// <summary>
        /// Подразделения
        /// </summary>
        public IEnumerable<SelectListItem> Managers { get; set; }
    }
}