using System;
using System.ComponentModel.DataAnnotations;

namespace PgDbase.entity
{
    public enum ClaimSection
    {
        Undefined,
        CMS,
        Module
    }

    public class RoleClaimModel
    {
  
        public int Id { get; set; }

        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }

        public bool Checked { get; set; }

        /// <summary>
        /// Раздел (CMS/Modules)
        /// </summary>
        public ClaimSection Section { get; set; }

    }


    public class RoleModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public string Discriminator { get; set; }

        public RoleClaimModel[] Claims { get; set; }

        public SitesModel[] Sites { get; set; }

    }

    public class UserRoleModel
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }


    
    /// <summary>
    /// Разрешения
    /// </summary>
    public class ResolutionModel
    {
        /// <summary>
        /// Чтение
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Запись
        /// </summary>
        public bool IsWrite { get; set; }

        /// <summary>
        /// Изменение
        /// </summary>
        public bool IsChange { get; set; }

        /// <summary>
        /// Удаление
        /// </summary>
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// Права на разделы
    /// </summary>
    public class ClaimParams
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid ContentId { get; set; }

        /// <summary>
        /// Идентификатор меню
        /// </summary>
        public Guid MenuId { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string Claim { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public bool IsChecked { get; set; }
    }
}
