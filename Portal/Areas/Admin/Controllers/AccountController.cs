﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PgDbase.entity;
using System.Web.Security;
using PgDbase;
using Portal.Areas.Admin.Models;

namespace Portal.Areas.Admin.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        protected bool _IsAuthenticated = System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        protected AccountRepository _accountRepository;
        protected int maxLoginError = 5;

        /// <summary>
        /// Page autorization
        /// </summary>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            // Авторизованного пользователя направляем на главную страницу
            if (_IsAuthenticated) return RedirectToAction("", "Main");
            else
            {
                string UID = RequestUserInfo.CookiesValue(".ASPXAUTHMORE");

                if (UID != string.Empty)
                {
                    AccountModel AccountInfo = _accountRepository.getCmsAccount(new Guid(UID));
                    // Если пользователь найден
                    if (AccountInfo != null)
                    {
                        FormsAuthentication.SetAuthCookie(AccountInfo.Id.ToString(), false);
                        return RedirectToAction("Index", "Main");
                    }
                }

                return View();
            }
        }


        [HttpPost]
        public ActionResult LogIn(LogInModel model)
        {
            try
            {
                // Ошибки в форме
                if (!ModelState.IsValid) return View(model);

                string _login = model.Login;
                string _pass = model.Pass;
                bool _remember = model.RememberMe;

                AccountModel AccountInfo = _accountRepository.getCmsAccount(_login);

                // Если пользователь найден
                if (AccountInfo != null)
                {
                    if (AccountInfo.Disabled)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "Пользователь заблокирован и не имеет прав на доступ в систему администрирования");
                    }
                    else if (AccountInfo.CountError && AccountInfo.LockDate.Value.AddMinutes(15) >= DateTime.Now)
                    {
                        // Оповещение о блокировке
                        ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                        ModelState.AddModelError("", "————");
                        ModelState.AddModelError("", "Вы можете повторить попытку через " + (AccountInfo.LockDate.Value.AddMinutes(16) - DateTime.Now).Minutes + " минут");
                    }
                    else
                    {
                        // Проверка на совпадение пароля
                        Cripto password = new Cripto(AccountInfo.Salt, AccountInfo.Hash);
                        if (password.Verify(_pass.ToCharArray()))
                        {
                            // Удачная попытка, Авторизация
                            FormsAuthentication.SetAuthCookie(AccountInfo.Id.ToString(), _remember);

                            HttpCookie MyCookie = new HttpCookie(".ASPXAUTHMORE");
                            MyCookie.Value = HttpUtility.UrlEncode(AccountInfo.Id.ToString(), System.Text.Encoding.UTF8);
                            MyCookie.Domain = "." + Settings.BaseURL;
                            Response.Cookies.Add(MyCookie);


                            // Записываем данные об авторизации пользователя
                            _accountRepository.SuccessLogin(AccountInfo.Id, RequestUserInfo.IP);

                            return RedirectToAction("Index", "Main");
                        }
                        else
                        {
                            // Неудачная попытка
                            // Записываем данные о попытке авторизации и плучаем кол-во неудавшихся попыток входа
                            int attemptNum = _accountRepository.FailedLogin(AccountInfo.Id, RequestUserInfo.IP);
                            if (attemptNum == maxLoginError)
                            {
                                #region Оповещение о блокировке
                                // Формируем код востановления пароля
                                Guid RestoreCode = Guid.NewGuid();
                                _accountRepository.setRestorePassCode(AccountInfo.Id, RestoreCode, RequestUserInfo.IP);

                                // оповещение на e-mail
                                string Massege = String.Empty;
                                Mailer Letter = new Mailer();
                                Letter.Theme = "Блокировка пользователя";
                                Massege = "<p>Уважаемый " + AccountInfo.Surname + " " + AccountInfo.Name + ", в системе администрирования сайта " + Request.Url.Host + " было 5 неудачных попыток ввода пароля.<br />В целях безопасности, ваш аккаунт заблокирован.</p>";
                                Massege += "<p>Для восстановления прав доступа мы сформировали для Вас ссылку, перейдя по которой, Вы сможете ввести новый пароль для вашего аккаунта и учетная запись будет разблокирована.</p>";
                                Massege += "<p>Если вы вспомнили пароль и хотите ещё раз пропробовать авторизоваться, то подождите 15 минут. Спустя это время, система позволит Вам сделать ещё попытку.</p>";
                                Massege += "<p><a href=\"http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/\">http://" + Request.Url.Host + "/Admin/Account/ChangePass/" + RestoreCode + "/</a></p>";
                                Massege += "<p>С уважением, администрация сайта!</p>";
                                Massege += "<hr><i><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</i></span>";
                                Letter.MailTo = AccountInfo.Mail;
                                Letter.Text = Massege;

                                //Логируем в SendMail
                                var res = Letter.SendMail();

                                #endregion
                                ModelState.AddModelError("", "После " + maxLoginError + " неудачных попыток авторизации Ваш пользователь временно заблокирован.");
                                ModelState.AddModelError("", "Вам на почту отправлено сообщение с инструкцией по разблокировки и смене пароля.");
                                ModelState.AddModelError("", "---");
                                ModelState.AddModelError("", "Если вы хотите попробовать ещё раз, подождите 15 минут.");
                            }
                            else
                            {
                                // Оповещение об ошибке
                                string attemptCount = (maxLoginError - attemptNum == 1) ? "Осталась 1 попытка" : "Осталось " + (maxLoginError - attemptNum) + " попытки";
                                ModelState.AddModelError("", "Пара логин и пароль не подходят.");
                                ModelState.AddModelError("", attemptCount + " ввода пароля.");
                            }
                        }
                    }
                }
                else
                {
                    // Оповещение о неверном логине
                    ModelState.AddModelError("", "Такой пользователь не зарегистрирован в системе.");
                    ModelState.AddModelError("", "Проверьте правильность вводимых данных.");
                }


                return View();
            }
            catch (HttpAntiForgeryException ex)
            {
                return View();
            }
        }
    }
}