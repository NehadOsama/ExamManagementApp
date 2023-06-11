using CemexExamApp.DBCore;
using CemexExamApp.Models;
using CemexExamApp.Repository;
using CemexExamApp.ViewModel;
using CemexExamApp.ViewModel.VMAccount;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using TheInventory.Models;


namespace CemexExamApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICemexManagExam<SecUser> secuserRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccountController(ICemexManagExam<SecUser> secuserRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            this.secuserRepository = secuserRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login()
        {
            httpContextAccessor.HttpContext.SignOutAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string returnURL)
        {
            if (!string.IsNullOrEmpty(returnURL) && Url.IsLocalUrl(returnURL))
            {
                return Redirect(returnURL);
            }
           
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "please enter valid Username or password");
                return View("Login");
            }
            AdminUser adminUser = new AdminUser();
             var obj = adminUser.Find(model.UserName);
           
            if (obj == null)
            {
                ModelState.AddModelError("", "Username not exist or not active");
                return View("Login");
            }
            else
            {
                try
                {
                    if ((obj.AD && adminUser.LoginAD(model.UserName.Trim(), model.Password.Trim())) ||
                          (!obj.AD && adminUser.IsValidPassword(model.Password.Trim(), obj.Password)))
                    {
                        addclaim(obj, model.Password);
                        // TO do : security session

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                     
                        ModelState.AddModelError("", "Invalid Username or Password");
                        return View("Login");
                    }


                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException == null ? ex.Message : ex.Message + " " + ex.InnerException.Message);
                    return View("Login");
                }
            }


        }
        public async void addclaim(SecUser obj, string password)
        {
            AdminUser Userobj = new AdminUser();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , obj.Username),
                new Claim(ClaimTypes.NameIdentifier ,obj.ID.ToString()),
                new Claim(ClaimTypes.Role, obj.Role.Name.ToString()),
               new Claim("CurrentRoleId" , obj.Role.ID.ToString()),

            };

            //var percontrollactionlist = Userobj.FindControllerActionByGroupId(obj.RoleID);
            //foreach (var i in percontrollactionlist)
            //{
              
            //    claims.Add(new Claim(i.ControllerAction.layoutName, "true"));
            //}

            
            var claimsidentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {

                IsPersistent = false

            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsidentity), authProperties);



        }

        public async Task<ActionResult> Logout()
        {

            //  await signInManager.SignOutAsync();              // to delete Identity
            await HttpContext.SignOutAsync();             // to delete cookies 

            
            return RedirectToAction("Login", "Account");

        }

        [CustomPrivilege]
        public IActionResult MyProfile()
        {
            AdminUser adminUser = new AdminUser();

            var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            SecUser secUser =  adminUser.Find(int.Parse(currentUserId));
            return View(secUser);
        }

        
        
        public IActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            AdminUser adminUser = new AdminUser();
            SecUser secUser = adminUser.GetActiveUserByEmail(forgetPasswordViewModel.Email);
            if (secUser != null)
            {
                string GeneratePass = adminUser.GetRandomPassword(9);
                secUser.Password = adminUser.GetHashPassword(GeneratePass);
                string Body = @" <pre style='font-family:Calibri; font-size:medium'><b>Dear Cemex employee ,</b><br/>
         Your New Passwword is :  </pre><br/>";
                Body += "<pre style='font-family:Calibri; font-size:medium'>" + GeneratePass + "</pre>";
                Body += "<br/> You can change your password from your profile page after login by above password.";
                Body += " <br/><pre style='font-family:Calibri;font-size:medium'> <b>Best Regards,</b><br/>";
                Body += "CEMEX Team <br/>";
                Body += "This is an automatic Email generated by CEMEX Team ,Please do not reply to this Email </pre>";

                AdminHelper.SendMail(null, secUser.Email.Trim(), null, "CEMEX Exam Management User Password", Body, null);
                ViewBag.Message = "We have sent an email with the instructions to reset your password.";
                //  return JavaScript("<script>alert(\"some message\")</script>");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email");
            }
            return View();
        }



        
        [CustomPrivilege]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                AdminUser adminUser = new AdminUser();
                //Logic 
               string CurrentUserId =  httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
               SecUser secUser = secuserRepository.Find(int.Parse(CurrentUserId));
                if (!secUser.AD)
                {
                    string hashedOldPass = adminUser.GetHashPassword(changePasswordViewModel.CurrentPassword);
                    
                    if(adminUser.IsValidPassword(changePasswordViewModel.CurrentPassword, hashedOldPass))
                    {
                        string NewPass = changePasswordViewModel.NewPassword;

                        string hashedNewPass = adminUser.GetHashPassword(changePasswordViewModel.NewPassword);
                        secUser.Password = hashedNewPass;
                        secuserRepository.Update(int.Parse(CurrentUserId), secUser);
                        ViewBag.Message = "Your password is Successfully Changed.";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Current Password is Wrong.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your Account is Active Directory and Cannot change password from here.");
                }

            }
            
          
            return View();
        }
    }
}
