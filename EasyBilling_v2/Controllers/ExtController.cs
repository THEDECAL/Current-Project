using EasyBilling.Attributes;
using EasyBilling.Controllers;
using EasyBilling.Data;
using EasyBilling.Helpers;
using EasyBilling.Interfaces;
using EasyBilling.Models;
using EasyBilling.Models.Entities;
using EasyBilling.Services;
using EasyBilling.ViewModels;
using EasyBillingV2.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static Newtonsoft.Json.JsonConvert;

namespace Microsoft.AspNetCore.Mvc
{
    [Authorize]
    [CheckAccessRights]
    public abstract class ExtController<TEntity> : Controller, IApiCrud<TEntity> where TEntity : ExtEntity
    {
        public EntityRepository<TEntity> ERepo { get; }
        public RoleManager<Role> RoleMngr { get; }
        public UserManager<User> UserMngr { get; }
        public IServiceScopeFactory SsFactory { get; }
        public BillingDbContext DbContext { get; }
        public DbSet<TEntity> DbSet { get => DbContext?.Set<TEntity>(); }
        public string DisplayName { get => (GetType().GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName; }
        public string ControllerName { get => GetType().Name.Replace("Controller", ""); }
        public string ActionName { get => RouteData.Values["action"].StrChk().FirstCharToUpper(); }
        public string UrlPath { get => HttpContext.Request.Path.Value.StrChk(); }
        public bool IsApi { get => UrlPath.Contains(@"/api/"); }
        public Type EntityType { get; }
        public PaginationState PgnState { get; }
        public TableDataViewModel<TEntity> TableData { get => ViewBag[nameof(TableData)]; set => ViewBag[nameof(TableData)] = value; }

        ///
        public ExtController(BillingDbContext ctx,
                RoleManager<Role> roleMngr,
                UserManager<User> userMngr,
                IServiceScopeFactory ssFactory)
        {
            EntityType = typeof(TEntity);
            DbContext = ctx;
            PgnState = new PaginationState();
            ERepo = new EntityRepository<TEntity>(this);
            RoleMngr = roleMngr;
            UserMngr = userMngr;
            SsFactory = ssFactory;

            DbContext.Set<TEntity>().Local.PropertyChanging += OnPropertyChanging;
        }

        /// <summary>
        /// Метод оброботчика во время изменения объекта (до сохранения в БД)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            sender.
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index() => await Task.Run(() => View("CustomIndex"));

        /// <summary>
        /// Синхронизация Cookie настрок пагинации
        /// </summary>
        /// <returns></returns>
        private void PaginationStateSync()
        {
                var pgStCookie = ControllerName + "Settings";
                string pagStateJson = null;
                HttpContext.Request.Cookies.TryGetValue(pgStCookie, out pagStateJson);

                if (pagStateJson != null)
                {
                    PgnState.ChangeState(DeserializeObject<PaginationState>(pagStateJson) ?? new PaginationState());
                }
                else
                {
                    var opt = new CookieOptions()
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(1),
                        MaxAge = TimeSpan.FromDays(7),
                        SameSite = SameSiteMode.Lax,
                        IsEssential = true,
                        Secure = true
                    };

                    HttpContext.Response.Cookies.Append(pgStCookie, SerializeObject(PgnState), opt);
                }
            }

        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string actionDisplayName = "";
            try
            {
                var actionDnAtt = ControllerContext.ActionDescriptor.MethodInfo
                    .GetCustomAttribute<DisplayNameAttribute>();
                actionDisplayName = " - " + actionDnAtt?.DisplayName;
            }
            catch (Exception ex)
            { Debug.WriteLine(ex.StackTrace); }

            PaginationStateSync();

            ViewData["Title"] = $"{DisplayName}{actionDisplayName}";
            ViewData["ControllerName"] = ControllerName;
            ViewData["ActionName"] = ActionName;

            //Добавление данных таблицы в соотвествии с состоянием пагинации
            TableData = new TableDataViewModel<TEntity>(SsFactory, PgnState, UrlPath);

            await base.OnActionExecutionAsync(context, next);
        }

        #region => ApiCrud
        [HttpPost]
        public async Task<ActionResult> Add(TEntity model)
        {
            if (model != null)
            {
                var result = await ERepo.Add(model);
                if (!ModelState.IsValid) { return BadRequest(ModelState); }
                else if (result) { return Ok(model); }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Change(TEntity model)
        {
            if (model != null)
            {
                var result = await ERepo.Change(model);
                if (!ModelState.IsValid) { return BadRequest(ModelState); }
                else if (result) { return Ok(model); }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Remove(int id)
        {
            if (id != 0)
            {
                var result = await ERepo.Remove(id);
                if (!ModelState.IsValid) { return BadRequest(ModelState); }
                else if (result) { return Ok(); }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Get(int id)
        {
            if (id != 0)
            {
                var result = await ERepo.Get(id);
                if (!ModelState.IsValid) { return BadRequest(ModelState); }
                else if (result != null) { return Ok(result); }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> Find(PaginationState pgState = null)
        {
            var result = await ERepo.Find(pgState);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            else if (result != null) { return Ok(result); }

            return BadRequest();
        }
        #endregion
    }
}