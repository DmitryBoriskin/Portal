﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace PgDbase.models
{
	/// <summary>
	/// Database       : portal
	/// Data Source    : chuvashia.com
	/// Server Version : 9.5.5
	/// </summary>
	public partial class CMSdb : LinqToDB.Data.DataConnection
	{
		public ITable<core_controllers>            core_controllers            { get { return this.GetTable<core_controllers>(); } }
		public ITable<core_log_actions>            core_log_actions            { get { return this.GetTable<core_log_actions>(); } }
		public ITable<core_log_sections>           core_log_sections           { get { return this.GetTable<core_log_sections>(); } }
		public ITable<core_logs>                   core_logs                   { get { return this.GetTable<core_logs>(); } }
		public ITable<core_material_categories>    core_material_categories    { get { return this.GetTable<core_material_categories>(); } }
		public ITable<core_material_category_link> core_material_category_link { get { return this.GetTable<core_material_category_link>(); } }
		public ITable<core_materials>              core_materials              { get { return this.GetTable<core_materials>(); } }
		public ITable<core_menu>                   core_menu                   { get { return this.GetTable<core_menu>(); } }
		public ITable<core_page_group_links>       core_page_group_links       { get { return this.GetTable<core_page_group_links>(); } }
		public ITable<core_page_groups>            core_page_groups            { get { return this.GetTable<core_page_groups>(); } }
		public ITable<core_pages>                  core_pages                  { get { return this.GetTable<core_pages>(); } }
		public ITable<core_site_controllers>       core_site_controllers       { get { return this.GetTable<core_site_controllers>(); } }
		public ITable<core_site_domains>           core_site_domains           { get { return this.GetTable<core_site_domains>(); } }
		public ITable<core_sites>                  core_sites                  { get { return this.GetTable<core_sites>(); } }
		public ITable<core_user_group_resolutions> core_user_group_resolutions { get { return this.GetTable<core_user_group_resolutions>(); } }
		public ITable<core_user_groups>            core_user_groups            { get { return this.GetTable<core_user_groups>(); } }
		public ITable<core_user_resolutions>       core_user_resolutions       { get { return this.GetTable<core_user_resolutions>(); } }
		public ITable<core_user_site_link>         core_user_site_link         { get { return this.GetTable<core_user_site_link>(); } }
		public ITable<core_users>                  core_users                  { get { return this.GetTable<core_users>(); } }
		public ITable<core_views>                  core_views                  { get { return this.GetTable<core_views>(); } }

		public CMSdb()
			: base("CMSdb")
		{
			InitDataContext();
		}

		public CMSdb(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table(Schema="core", Name="controllers")]
	public partial class core_controllers
	{
		[Column,        Nullable] public string c_name            { get; set; } // character varying(128)
		[Column,     NotNull    ] public string c_controller_name { get; set; } // character varying(128)
		[Column,        Nullable] public string c_action_name     { get; set; } // character varying(128)
		[Column,     NotNull    ] public Guid   c_default_view    { get; set; } // uuid
		[PrimaryKey, NotNull    ] public Guid   id                { get; set; } // uuid
		[Column,        Nullable] public Guid?  pid               { get; set; } // uuid
		[Column,        Nullable] public string c_desc            { get; set; } // text

		#region Associations

		/// <summary>
		/// fk_controller_parent_id
		/// </summary>
		[Association(ThisKey="pid", OtherKey="id", CanBeNull=true, KeyName="fk_controller_parent_id", BackReferenceName="fk_controller_parent_id_BackReferences")]
		public core_controllers fkcontrollerparentid { get; set; }

		/// <summary>
		/// fk_controller_parent_id_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="pid", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_controllers> fk_controller_parent_id_BackReferences { get; set; }

		/// <summary>
		/// fk_site_controllers__controllers_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_controller", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_site_controllers> fksites { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="log_actions")]
	public partial class core_log_actions
	{
		[PrimaryKey, NotNull    ] public string c_action      { get; set; } // character varying(32)
		[Column,        Nullable] public string c_action_name { get; set; } // character varying(100)
		[Column,        Nullable] public int?   id_old        { get; set; } // integer
		[Identity               ] public int    id            { get; set; } // integer

		#region Associations

		/// <summary>
		/// fk_log_log_action_BackReference
		/// </summary>
		[Association(ThisKey="c_action", OtherKey="f_action", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_logs> fkloglogactions { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="log_sections")]
	public partial class core_log_sections
	{
		[PrimaryKey, NotNull] public string c_alias        { get; set; } // character varying(64)
		[Column,     NotNull] public string c_section_name { get; set; } // character varying(64)

		#region Associations

		/// <summary>
		/// fk_log_log_sections_BackReference
		/// </summary>
		[Association(ThisKey="c_alias", OtherKey="f_logsections", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_logs> fkloglogsectionss { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="logs")]
	public partial class core_logs
	{
		[PrimaryKey, NotNull    ] public Guid     id            { get; set; } // uuid
		[Column,     NotNull    ] public Guid     f_page        { get; set; } // uuid
		[Column,        Nullable] public string   c_page_name   { get; set; } // character varying(512)
		[Column,        Nullable] public Guid?    f_site        { get; set; } // uuid
		[Column,     NotNull    ] public string   f_logsections { get; set; } // character varying(64)
		[Column,     NotNull    ] public Guid     f_user        { get; set; } // uuid
		[Column,     NotNull    ] public DateTime d_date        { get; set; } // timestamp (6) without time zone
		[Column,     NotNull    ] public string   f_action      { get; set; } // character varying(32)
		[Column,     NotNull    ] public string   c_ip          { get; set; } // character varying(16)
		[Column,        Nullable] public string   c_json        { get; set; } // text
		[Column,        Nullable] public string   c_comment     { get; set; } // character varying(1024)

		#region Associations

		/// <summary>
		/// fk_log_users
		/// </summary>
		[Association(ThisKey="f_user", OtherKey="id", CanBeNull=false, KeyName="fk_log_users", BackReferenceName="fklogs")]
		public core_users fklogusers { get; set; }

		/// <summary>
		/// fk_log_log_sections
		/// </summary>
		[Association(ThisKey="f_logsections", OtherKey="c_alias", CanBeNull=false, KeyName="fk_log_log_sections", BackReferenceName="fkloglogsectionss")]
		public core_log_sections fkloglogsections { get; set; }

		/// <summary>
		/// fk_log_log_action
		/// </summary>
		[Association(ThisKey="f_action", OtherKey="c_action", CanBeNull=false, KeyName="fk_log_log_action", BackReferenceName="fkloglogactions")]
		public core_log_actions fkloglogaction { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="material_categories")]
	public partial class core_material_categories
	{
		[PrimaryKey, NotNull] public Guid   id      { get; set; } // uuid
		[Column,     NotNull] public string c_name  { get; set; } // character varying(128)
		[Column,     NotNull] public string c_alias { get; set; } // character varying
		[Column,     NotNull] public int    n_sort  { get; set; } // integer
		[Column,     NotNull] public Guid   f_site  { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_materials_categories_sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_materials_categories_sites", BackReferenceName="fkmaterialscategoriess")]
		public core_sites fkmaterialscategoriessites { get; set; }

		/// <summary>
		/// fk_materials_categories_link_f_mk_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_maaterials_category", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_material_category_link> fkmaterialscategorieslinkfmks { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="material_category_link")]
	public partial class core_material_category_link
	{
		[PrimaryKey, NotNull    ] public Guid  id                    { get; set; } // uuid
		[Column,        Nullable] public Guid? f_materials           { get; set; } // uuid
		[Column,        Nullable] public Guid? f_maaterials_category { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_materials_categories_link_f_mk
		/// </summary>
		[Association(ThisKey="f_maaterials_category", OtherKey="id", CanBeNull=true, KeyName="fk_materials_categories_link_f_mk", BackReferenceName="fkmaterialscategorieslinkfmks")]
		public core_material_categories fkmaterialscategorieslinkfmk { get; set; }

		/// <summary>
		/// fk_materials_categories_link
		/// </summary>
		[Association(ThisKey="f_materials", OtherKey="gid", CanBeNull=true, KeyName="fk_materials_categories_link", BackReferenceName="fkcategorieslinks")]
		public core_materials fkmaterialscategorieslink { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="materials")]
	public partial class core_materials
	{
		[Identity               ] public int      id            { get; set; } // integer
		[PrimaryKey, NotNull    ] public Guid     gid           { get; set; } // uuid
		[Column,     NotNull    ] public DateTime d_date        { get; set; } // timestamp (6) without time zone
		[Column,     NotNull    ] public string   c_name        { get; set; } // character varying(512)
		[Column,        Nullable] public string   c_text        { get; set; } // text
		[Column,        Nullable] public string   c_photo       { get; set; } // character varying(512)
		[Column,        Nullable] public string   c_keyw        { get; set; } // character varying(256)
		[Column,        Nullable] public string   c_desc        { get; set; } // character varying(512)
		[Column,        Nullable] public string   c_source_url  { get; set; } // character varying(512)
		[Column,        Nullable] public string   c_source_name { get; set; } // character varying(128)
		[Column,        Nullable] public int?     c_view_count  { get; set; } // integer
		[Column,        Nullable] public bool?    b_disabled    { get; set; } // boolean
		[Column,        Nullable] public bool?    b_important   { get; set; } // boolean
		[Column,     NotNull    ] public Guid     f_site        { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_materials_sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_materials_sites", BackReferenceName="fkmaterialss")]
		public core_sites fksites { get; set; }

		/// <summary>
		/// fk_materials_categories_link_BackReference
		/// </summary>
		[Association(ThisKey="gid", OtherKey="f_materials", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_material_category_link> fkcategorieslinks { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="menu")]
	public partial class core_menu
	{
		[PrimaryKey, NotNull    ] public Guid   id       { get; set; } // uuid
		[Column,        Nullable] public string c_title  { get; set; } // character varying(128)
		[Column,     NotNull    ] public string c_alias  { get; set; } // character varying(32)
		[Column,        Nullable] public string c_class  { get; set; } // character varying(50)
		[Column,        Nullable] public int?   n_sort   { get; set; } // integer
		[Column,        Nullable] public Guid?  f_parent { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_menu_parent
		/// </summary>
		[Association(ThisKey="f_parent", OtherKey="id", CanBeNull=true, KeyName="fk_menu_parent", BackReferenceName="fk_menu_parent_BackReferences")]
		public core_menu fkparent { get; set; }

		/// <summary>
		/// fk_menu_parent_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_parent", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_menu> fk_menu_parent_BackReferences { get; set; }

		/// <summary>
		/// fk_user_group_resolutions__menu_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_menu", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_user_group_resolutions> fkusergroupresolutionss { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="page_group_links")]
	public partial class core_page_group_links
	{
		[PrimaryKey, NotNull] public Guid id           { get; set; } // uuid
		[Column,     NotNull] public Guid f_page       { get; set; } // uuid
		[Column,     NotNull] public Guid f_page_group { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_page_group_link_page_group
		/// </summary>
		[Association(ThisKey="f_page_group", OtherKey="id", CanBeNull=false, KeyName="fk_page_group_link_page_group", BackReferenceName="fkpagegrouplinkpagegroups")]
		public core_page_groups fkpagegrouplinkpagegroup { get; set; }

		/// <summary>
		/// fk_page_group_link_page
		/// </summary>
		[Association(ThisKey="f_page", OtherKey="gid", CanBeNull=false, KeyName="fk_page_group_link_page", BackReferenceName="fkpagegrouplinkpages")]
		public core_pages fkpagegrouplinkpage { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="page_groups")]
	public partial class core_page_groups
	{
		[PrimaryKey, NotNull] public Guid   id     { get; set; } // uuid
		[Column,     NotNull] public string c_name { get; set; } // character varying(128)
		[Column,     NotNull] public int    n_sort { get; set; } // integer
		[Column,     NotNull] public Guid   f_site { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_page_groups_sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_page_groups_sites", BackReferenceName="fkpagegroupss")]
		public core_sites fkpagegroupssites { get; set; }

		/// <summary>
		/// fk_page_group_link_page_group_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_page_group", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_page_group_links> fkpagegrouplinkpagegroups { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="pages")]
	public partial class core_pages
	{
		[PrimaryKey, NotNull    ] public Guid   gid                { get; set; } // uuid
		[Column,     NotNull    ] public string c_name             { get; set; } // character varying(512)
		[Column,     NotNull    ] public Guid   pgid               { get; set; } // uuid
		[Column,        Nullable] public string c_path             { get; set; } // character varying(1024)
		[Column,        Nullable] public string c_alias            { get; set; } // character varying(128)
		[Column,        Nullable] public string c_text             { get; set; } // text
		[Column,        Nullable] public string c_url              { get; set; } // character varying(256)
		[Column,     NotNull    ] public int    n_sort             { get; set; } // integer
		[Column,     NotNull    ] public Guid   f_site             { get; set; } // uuid
		[Column,     NotNull    ] public bool   b_disabled         { get; set; } // boolean
		[Column,        Nullable] public string c_keyw             { get; set; } // character varying(512)
		[Column,        Nullable] public string c_desc             { get; set; } // character varying(1024)
		[Column,        Nullable] public int?   f_sites_controller { get; set; } // integer
		[Column,     NotNull    ] public bool   b_deleteble        { get; set; } // boolean

		#region Associations

		/// <summary>
		/// fk_page_page
		/// </summary>
		[Association(ThisKey="pgid", OtherKey="gid", CanBeNull=false, KeyName="fk_page_page", BackReferenceName="fk_page_page_BackReferences")]
		public core_pages fkpagepage { get; set; }

		/// <summary>
		/// fk_page_sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_page_sites", BackReferenceName="fkpages")]
		public core_sites fkpagesites { get; set; }

		/// <summary>
		/// fk_page_group_link_page_BackReference
		/// </summary>
		[Association(ThisKey="gid", OtherKey="f_page", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_page_group_links> fkpagegrouplinkpages { get; set; }

		/// <summary>
		/// fk_page_page_BackReference
		/// </summary>
		[Association(ThisKey="gid", OtherKey="pgid", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_pages> fk_page_page_BackReferences { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="site_controllers")]
	public partial class core_site_controllers
	{
		[PrimaryKey, NotNull    ] public Guid   id           { get; set; } // uuid
		[Column,     NotNull    ] public Guid   f_site       { get; set; } // uuid
		[Column,        Nullable] public Guid?  f_controller { get; set; } // uuid
		[Column,        Nullable] public Guid?  f_view       { get; set; } // uuid
		[Column,        Nullable] public string c_desc       { get; set; } // text

		#region Associations

		/// <summary>
		/// fk_site_controllers__view
		/// </summary>
		[Association(ThisKey="f_view", OtherKey="id", CanBeNull=true, KeyName="fk_site_controllers__view", BackReferenceName="fksitecontrollersviews")]
		public core_views fksitecontrollersview { get; set; }

		/// <summary>
		/// fk_site_controllers__sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_site_controllers__sites", BackReferenceName="fksitecontrollerss")]
		public core_sites fksitecontrollerssites { get; set; }

		/// <summary>
		/// fk_site_controllers__controllers
		/// </summary>
		[Association(ThisKey="f_controller", OtherKey="id", CanBeNull=true, KeyName="fk_site_controllers__controllers", BackReferenceName="fksites")]
		public core_controllers fksitecontrollerscontrollers { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="site_domains")]
	public partial class core_site_domains
	{
		[Column,     NotNull] public Guid   f_site    { get; set; } // uuid
		[Column,     NotNull] public string c_domain  { get; set; } // character varying(256)
		[Column,     NotNull] public bool   b_default { get; set; } // boolean
		[PrimaryKey, NotNull] public Guid   id        { get; set; } // uuid
		[Identity           ] public int    num       { get; set; } // integer

		#region Associations

		/// <summary>
		/// fk_sites_domains_sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_sites_domains_sites", BackReferenceName="fkdomainss")]
		public core_sites fksitesdomainssites { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="sites")]
	public partial class core_sites
	{
		[PrimaryKey, NotNull    ] public Guid   id         { get; set; } // uuid
		[Column,     NotNull    ] public string c_name     { get; set; } // character varying(512)
		[Column,     NotNull    ] public bool   b_disabled { get; set; } // boolean
		[Column,        Nullable] public string c_fullname { get; set; } // character varying(4096)

		#region Associations

		/// <summary>
		/// fk_page_groups_sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_page_groups> fkpagegroupss { get; set; }

		/// <summary>
		/// fk_page_sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_pages> fkpages { get; set; }

		/// <summary>
		/// fk_sites_domains_sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_site_domains> fkdomainss { get; set; }

		/// <summary>
		/// fk_materials_sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_materials> fkmaterialss { get; set; }

		/// <summary>
		/// fk_user_site_link_sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_user_site_link> fkusersitelinks { get; set; }

		/// <summary>
		/// fk_site_controllers__sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_site_controllers> fksitecontrollerss { get; set; }

		/// <summary>
		/// fk_materials_categories_sites_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_site", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_material_categories> fkmaterialscategoriess { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="user_group_resolutions")]
	public partial class core_user_group_resolutions
	{
		[PrimaryKey, Identity   ] public int   id          { get; set; } // integer
		[Column,        Nullable] public Guid? f_usergroup { get; set; } // uuid
		[Column,        Nullable] public bool? b_read      { get; set; } // boolean
		[Column,        Nullable] public bool? b_write     { get; set; } // boolean
		[Column,        Nullable] public bool? b_change    { get; set; } // boolean
		[Column,        Nullable] public bool? b_delete    { get; set; } // boolean
		[Column,     NotNull    ] public Guid  f_menu      { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_user_group_resolutions__menu
		/// </summary>
		[Association(ThisKey="f_menu", OtherKey="id", CanBeNull=false, KeyName="fk_user_group_resolutions__menu", BackReferenceName="fkusergroupresolutionss")]
		public core_menu fkusergroupresolutionsmenu { get; set; }

		/// <summary>
		/// fk_resolution_group_user_group
		/// </summary>
		[Association(ThisKey="f_usergroup", OtherKey="id", CanBeNull=true, KeyName="fk_resolution_group_user_group", BackReferenceName="fkresolutiongroupusergroups")]
		public core_user_groups fkresolutiongroupusergroup { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="user_groups")]
	public partial class core_user_groups
	{
		[PrimaryKey, NotNull] public Guid   id      { get; set; } // uuid
		[Column,     NotNull] public string c_title { get; set; } // character varying(128)
		[Column,     NotNull] public string c_alias { get; set; } // character varying(32)

		#region Associations

		/// <summary>
		/// fk_user_site_link_user_group_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_user_group", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_user_site_link> fkusersitelinkusergroups { get; set; }

		/// <summary>
		/// fk_resolution_group_user_group_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_usergroup", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_user_group_resolutions> fkresolutiongroupusergroups { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="user_resolutions")]
	public partial class core_user_resolutions
	{
		[PrimaryKey, NotNull    ] public Guid  id               { get; set; } // uuid
		[Column,        Nullable] public Guid? f_usersitelink   { get; set; } // uuid
		[Column,        Nullable] public bool? b_read           { get; set; } // boolean
		[Column,        Nullable] public bool? b_write          { get; set; } // boolean
		[Column,        Nullable] public bool? b_change         { get; set; } // boolean
		[Column,        Nullable] public bool? b_delete         { get; set; } // boolean
		[Column,        Nullable] public Guid? f_sitecontroller { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_resolution_user
		/// </summary>
		[Association(ThisKey="f_usersitelink", OtherKey="id", CanBeNull=true, KeyName="fk_resolution_user", BackReferenceName="fkresolutionusers")]
		public core_user_site_link fkresolutionuser { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="user_site_link")]
	public partial class core_user_site_link
	{
		[PrimaryKey, NotNull] public Guid id           { get; set; } // uuid
		[Column,     NotNull] public Guid f_site       { get; set; } // uuid
		[Column,     NotNull] public Guid f_user       { get; set; } // uuid
		[Column,     NotNull] public Guid f_user_group { get; set; } // uuid

		#region Associations

		/// <summary>
		/// fk_user_site_link_user_group
		/// </summary>
		[Association(ThisKey="f_user_group", OtherKey="id", CanBeNull=false, KeyName="fk_user_site_link_user_group", BackReferenceName="fkusersitelinkusergroups")]
		public core_user_groups fkusersitelinkusergroup { get; set; }

		/// <summary>
		/// fk_user_site_link_users
		/// </summary>
		[Association(ThisKey="f_user", OtherKey="id", CanBeNull=false, KeyName="fk_user_site_link_users", BackReferenceName="fkusersitelinks")]
		public core_users fkusersitelinkusers { get; set; }

		/// <summary>
		/// fk_user_site_link_sites
		/// </summary>
		[Association(ThisKey="f_site", OtherKey="id", CanBeNull=false, KeyName="fk_user_site_link_sites", BackReferenceName="fkusersitelinks")]
		public core_sites fkusersitelinksites { get; set; }

		/// <summary>
		/// fk_resolution_user_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_usersitelink", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_user_resolutions> fkresolutionusers { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="users")]
	public partial class core_users
	{
		[PrimaryKey, NotNull    ] public Guid      id                 { get; set; } // uuid
		[Column,        Nullable] public string    c_email            { get; set; } // character varying(128)
		[Column,        Nullable] public string    c_salt             { get; set; } // character varying(32)
		[Column,        Nullable] public string    c_hash             { get; set; } // character varying(128)
		[Column,        Nullable] public Guid?     c_change_pass_code { get; set; } // uuid
		[Column,     NotNull    ] public string    c_surname          { get; set; } // character varying(128)
		[Column,     NotNull    ] public string    c_name             { get; set; } // character varying(128)
		[Column,        Nullable] public string    c_patronymic       { get; set; } // character varying(128)
		[Column,     NotNull    ] public bool      b_disabled         { get; set; } // boolean
		[Column,     NotNull    ] public int       n_error_count      { get; set; } // integer
		[Column,        Nullable] public DateTime? d_try_login        { get; set; } // timestamp (6) without time zone

		#region Associations

		/// <summary>
		/// fk_user_site_link_users_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_user", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_user_site_link> fkusersitelinks { get; set; }

		/// <summary>
		/// fk_log_users_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_user", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_logs> fklogs { get; set; }

		#endregion
	}

	[Table(Schema="core", Name="views")]
	public partial class core_views
	{
		[PrimaryKey, NotNull    ] public Guid   id           { get; set; } // uuid
		[Column,     NotNull    ] public string c_name       { get; set; } // character varying(256)
		[Column,     NotNull    ] public string c_path       { get; set; } // character varying(512)
		[Column,     NotNull    ] public Guid   f_controller { get; set; } // uuid
		[Column,        Nullable] public string c_img        { get; set; } // character varying(512)

		#region Associations

		/// <summary>
		/// fk_site_controllers__view_BackReference
		/// </summary>
		[Association(ThisKey="id", OtherKey="f_view", CanBeNull=true, IsBackReference=true)]
		public IEnumerable<core_site_controllers> fksitecontrollersviews { get; set; }

		#endregion
	}

	public static partial class TableExtensions
	{
		public static core_controllers Find(this ITable<core_controllers> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_log_actions Find(this ITable<core_log_actions> table, string c_action)
		{
			return table.FirstOrDefault(t =>
				t.c_action == c_action);
		}

		public static core_log_sections Find(this ITable<core_log_sections> table, string c_alias)
		{
			return table.FirstOrDefault(t =>
				t.c_alias == c_alias);
		}

		public static core_logs Find(this ITable<core_logs> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_material_categories Find(this ITable<core_material_categories> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_material_category_link Find(this ITable<core_material_category_link> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_materials Find(this ITable<core_materials> table, Guid gid)
		{
			return table.FirstOrDefault(t =>
				t.gid == gid);
		}

		public static core_menu Find(this ITable<core_menu> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_page_group_links Find(this ITable<core_page_group_links> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_page_groups Find(this ITable<core_page_groups> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_pages Find(this ITable<core_pages> table, Guid gid)
		{
			return table.FirstOrDefault(t =>
				t.gid == gid);
		}

		public static core_site_controllers Find(this ITable<core_site_controllers> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_site_domains Find(this ITable<core_site_domains> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_sites Find(this ITable<core_sites> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_user_group_resolutions Find(this ITable<core_user_group_resolutions> table, int id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_user_groups Find(this ITable<core_user_groups> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_user_resolutions Find(this ITable<core_user_resolutions> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_user_site_link Find(this ITable<core_user_site_link> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_users Find(this ITable<core_users> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}

		public static core_views Find(this ITable<core_views> table, Guid id)
		{
			return table.FirstOrDefault(t =>
				t.id == id);
		}
	}
}
  
