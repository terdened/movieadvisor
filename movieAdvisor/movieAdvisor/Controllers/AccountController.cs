using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using movieAdvisor.Models;

namespace movieAdvisor.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }


        public ActionResult Login()
        {
            return RedirectToAction("LogOn");
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(USERS model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (MOVIEADVISOREntities2 entities = new MOVIEADVISOREntities2())
                {
                    bool userValid = entities.USERS.Any(user => user.USERNAME == model.USERNAME && user.PASSWORD == model.PASSWORD);

                    if (userValid)
                    {
                        FormsAuthentication.SetAuthCookie(model.USERNAME, false);
                        if(!Roles.IsUserInRole(model.USERNAME, entities.USERS.Where(user => user.USERNAME == model.USERNAME && user.PASSWORD == model.PASSWORD).FirstOrDefault().ROLES))
                            Roles.AddUserToRole(model.USERNAME, entities.USERS.Where(user => user.USERNAME == model.USERNAME && user.PASSWORD == model.PASSWORD).FirstOrDefault().ROLES);
                        
                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Имя пользователя или пароль указаны неверно.");
                    }
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Попытка зарегистрировать пользователя
                using (MOVIEADVISOREntities2 entities = new MOVIEADVISOREntities2())
                {
                    if (model.Password == model.ConfirmPassword)
                    {
                        USERS tempUser = new USERS();

                        if (entities.USERS.Count() > 0)
                        {
                            int tempId = entities.USERS.OrderByDescending(u=>u.ID).FirstOrDefault().ID;
                            tempUser.ID = tempId + 1;
                        }
                        else
                            tempUser.ID = 0;

                        tempUser.USERNAME = model.UserName;
                        tempUser.PASSWORD = model.Password;
                        tempUser.ROLES = "user";
                        tempUser.EMAIL = model.Email;

                        entities.USERS.AddObject(tempUser);
                        int createStatus;
                        
                        try
                        {
                            createStatus = entities.SaveChanges();
                        }
                        catch
                        {
                            createStatus = -1;
                        }

                        if (createStatus == 1)
                        {
                            FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError("", createStatus.ToString());
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Введеные пароли не совпадают.");
                    }
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // При некоторых сценариях сбоя операция смены пароля ChangePassword вызывает исключение,
                // а не возвращает значение false (ложь).
                int changePasswordSucceeded;
                try
                {
                    using (MOVIEADVISOREntities2 entities = new MOVIEADVISOREntities2())
                    {

                        if (entities.USERS.Where(u => u.USERNAME == User.Identity.Name).Where(u => u.PASSWORD == model.OldPassword).ToList().Count > 0)
                        {
                            entities.USERS.Where(u => u.USERNAME == User.Identity.Name).First().PASSWORD = model.NewPassword;
                            changePasswordSucceeded = entities.SaveChanges();
                        }
                        else
                            changePasswordSucceeded = -1;
                    }
                }
                catch (Exception)
                {
                    changePasswordSucceeded = -1;
                }

                if (changePasswordSucceeded == 1)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный текущий пароль или недопустимый новый пароль.");
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Полный список кодов состояния см. по адресу http://go.microsoft.com/fwlink/?LinkID=177550
            //.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Введите другое имя пользователя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Имя пользователя для данного адреса электронной почты уже существует. Введите другой адрес электронной почты.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Указан недопустимый пароль. Введите допустимое значение пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Указан недопустимый адрес электронной почты. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Указан недопустимый ответ на вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Указан недопустимый вопрос для восстановления пароля. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Указано недопустимое имя пользователя. Проверьте значение и повторите попытку.";

                case MembershipCreateStatus.ProviderError:
                    return "Поставщик проверки подлинности вернул ошибку. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос создания пользователя был отменен. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";

                default:
                    return "Произошла неизвестная ошибка. Проверьте введенное значение и повторите попытку. Если проблему устранить не удастся, обратитесь к системному администратору.";
            }
        }
        #endregion
    }
}
