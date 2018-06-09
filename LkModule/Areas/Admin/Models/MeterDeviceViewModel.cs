using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления приборов учёта
    /// </summary>
    public class MeterDeviceViewModel : CoreViewModel
    {
        public Paged<MeterDevice> List { get; set; }
    }
}