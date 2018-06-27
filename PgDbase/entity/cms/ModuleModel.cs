using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class ModuleModel
    {
        /// <summary>
        /// Идентификатор в бд
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор модуля-родителя
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Флаг только для дочерних элементов модуля, показывает относится контроллер к админке или внешней части
        /// </summary>
        public bool? InAdmin { get; set; }

        /// <summary>
        /// Имя модуля
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Имя контроллера
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Имя Экшена
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Вью
        /// </summary>
        public Guid View { get; set; }

        /// <summary>
        /// Описание модуля
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Составляющие части модуля
        /// </summary>
        public ModuleModel[] ModuleParts { get; set; }

        /// <summary>
        /// Разрешения для роли (только для RolesController и заполняется там же)
        /// </summary>
        public RoleModuleClaims RoleModuleClaims { get; set; }
    }

    public class RoleModuleClaims
    {
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
    }

}
