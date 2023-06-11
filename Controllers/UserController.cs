using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CemexExamApp.ContextData;
using CemexExamApp.Models;
using CemexExamApp.Models.Repositories;
using CemexExamApp.Repository;
using CemexExamApp.DBCore;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using CemexExamApp.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CemexExamApp.Controllers
{

    public class UserController : Controller
    {

        private readonly ICemexManagExam<SecUser> UserDbRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment hosting;

        public UserController(ICemexManagExam<SecUser> secuserRepository
            ,IHttpContextAccessor httpContextAccessor
            , IWebHostEnvironment hosting)
        {
            this.UserDbRepository = secuserRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.hosting = hosting;
        }


        // GET: User
        [CustomPrivilege]
        public ActionResult Index()
        {
            return View(UserDbRepository.List());
        }


        // GET: User/Create
        [CustomPrivilege]

        public ActionResult Create()
        {
            AdminUser adminUser = new AdminUser();
            ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel userViewModel)
        {
            try
            {
                AdminUser adminUser = new AdminUser();
               
                ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    if (adminUser.GetUserByUserName(userViewModel.Username) != null)
                    {
                        ModelState.AddModelError("", "UserName Already Exist Before.");
                    }
                    if (adminUser.GetUserByEmail(userViewModel.Email) != null)
                    {
                        ModelState.AddModelError("", "Email Already Exist Before.");
                    }
                    if ((userViewModel.ProfilePic != null) && (userViewModel.ProfilePic.FileName != ""))
                    {
                        if (userViewModel.ProfilePic.FileName.ToLower().EndsWith(".png") ||
                            userViewModel.ProfilePic.FileName.ToLower().EndsWith(".jpg"))
                        {
                            var fileSize = userViewModel.ProfilePic.Length;
                            if (fileSize > (5 * 1024 ))
                            {

                                ModelState.AddModelError("", "File size is too large. Maximum file size permitted is 5 MB");

                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", " File extension Must Be JPG - NPG ");
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        SecUser NewUSer = new SecUser();

                        if ((userViewModel.ProfilePic != null) && (userViewModel.ProfilePic.FileName != ""))
                        {
                            string filePath = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + userViewModel.ProfilePic.FileName;
                            var fullPath = Path.Combine(hosting.WebRootPath, "ProfilePic", filePath);
                            string res = AdminHelper.UploadFile(userViewModel.ProfilePic, fullPath);
                            NewUSer.ProfilePic = filePath;
                        }

                       
                        NewUSer.Username = userViewModel.Username.Trim();
                        NewUSer.Email = userViewModel.Email.Trim();
                        NewUSer.RoleID = userViewModel.RoleID;
                        NewUSer.Mobile = userViewModel.Mobile;
                        
                        NewUSer.AD = userViewModel.AD;
                        NewUSer.Active = userViewModel.Active;
                        NewUSer.FirstName = userViewModel.FirstName;
                        NewUSer.LastName = userViewModel.FullName;
                        NewUSer.CreatedDate = DateTime.Now;
                        NewUSer.CreatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

                        if (!userViewModel.AD)
                        {
                            string GeneratePass = adminUser.GetRandomPassword(9);
                            NewUSer.Password = adminUser.GetHashPassword(GeneratePass);
                            string Body = @" <pre style='font-family:Calibri; font-size:medium'><b>Dear Cemex employee ,</b><br/>
         Your New Passwword is :  </pre><br/>";
                            Body += "<pre style='font-family:Calibri; font-size:medium'>" + GeneratePass + "</pre>";
                            Body += "<br/> You can change your password from your profile page after login by above password.";
                            Body += " <br/><pre style='font-family:Calibri;font-size:medium'> <b>Best Regards,</b><br/>";
                            Body += "CEMEX Team <br/>";
                            Body += "This is an automatic Email generated by CEMEX Team ,Please do not reply to this Email </pre>";
                            AdminHelper.SendMail(null, NewUSer.Email.Trim(), null, "CEMEX Exam Management User Password", Body, null);
                        }
                        else
                        {
                            SecUser secUser1 =    adminUser.GetUserAD(AdminHelper.ConfigurationManager.AppSetting["ADDomain"], userViewModel.Email.Trim());
                        }

                        UserDbRepository.Add(NewUSer); 
                        ViewBag.Message = "New User Added Succssuflly.";
                        return View(userViewModel);
                    }
                    else
                    {
                       
                        return View(userViewModel);
                    }
                }
                else
                {
                 
                    return View(userViewModel);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return View(userViewModel);
            }
        }

        // GET: User/Edit/5
        [CustomPrivilege]
        public ActionResult Edit(int? id)
        {
            AdminUser adminUser = new AdminUser();
            ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");
            if (id == null)
            {
                return NotFound();
            }

            var ExistUser = adminUser.Find(id.Value);
            if (ExistUser == null)
            {
                return NotFound();
            }
            else
            {
                UserViewModel userViewModel = new UserViewModel();
                userViewModel.Username = ExistUser.Username.Trim();
                userViewModel.Email = ExistUser.Email.Trim();
                userViewModel.Active = ExistUser.Active;
                userViewModel.RoleID = ExistUser.RoleID;
                userViewModel.Mobile = ExistUser.Mobile;
                userViewModel.SavedPicPath = ExistUser.ProfilePic;
                userViewModel.AD = ExistUser.AD;
                userViewModel.FirstName = ExistUser.FirstName;
                userViewModel.FullName = ExistUser.LastName;
                userViewModel.LastUpdatedDate = DateTime.Now;
                userViewModel.LastUpdatedBy = ExistUser.LastUpdatedBy;


                return View(userViewModel);
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserViewModel userViewModel)
        {
            try
            {
                AdminUser adminUser = new AdminUser();
                ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");

                if (id != userViewModel.ID)
                {
                    return NotFound();
                }
                //Should cheeck if location name and storecode exist before
                if (ModelState.IsValid)
                {
                    if (adminUser.GetUserByUserNameBefore(userViewModel.Username, userViewModel.ID) != null)
                    {
                        ModelState.AddModelError("", "UserName Already Exist Before.");

                    }
                    if (adminUser.GetUserByEmailBefore(userViewModel.Email, userViewModel.ID) != null)
                    {
                        ModelState.AddModelError("", "Email Already Exist Before.");

                    }
                    if ((userViewModel.ProfilePic != null) && (userViewModel.ProfilePic.FileName != ""))
                    {
                        if (userViewModel.ProfilePic.FileName.ToLower().EndsWith(".png") ||
                            userViewModel.ProfilePic.FileName.ToLower().EndsWith(".jpg"))
                        {

                            var fileSize = userViewModel.ProfilePic.Length;
                            if (fileSize > (5 * 1024 * 1024))
                            {

                                ModelState.AddModelError("", "File size is too large. Maximum file size permitted is 5 MB");

                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", " File extension Must Be JPG - NPG ");
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        var updateUser = adminUser.Find(id);

                        if ((userViewModel.ProfilePic != null) && (userViewModel.ProfilePic.FileName != ""))
                        {
                            string filePath = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + userViewModel.ProfilePic.FileName;
                            var fullPath = Path.Combine(hosting.WebRootPath, "ProfilePic", filePath);
                            string res = AdminHelper.UploadFile(userViewModel.ProfilePic, fullPath);
                            updateUser.ProfilePic = filePath;
                        }
                       

                        updateUser.Username = userViewModel.Username.Trim();
                        updateUser.Email = userViewModel.Email.Trim();
                        updateUser.Active = userViewModel.Active;
                        updateUser.RoleID = userViewModel.RoleID;
                        updateUser.Mobile = userViewModel.Mobile;
                        updateUser.AD = userViewModel.AD;
                        updateUser.FirstName = userViewModel.FirstName;
                        updateUser.LastName = userViewModel.FullName;
                        updateUser.LastUpdatedDate = DateTime.Now;
                        updateUser.LastUpdatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                      
         //               if (!userViewModel.AD)
         //               {
         //                   string GeneratePass = adminUser.GetRandomPassword(9);
         //                   updateUser.Password = adminUser.GetHashPassword(GeneratePass);
         //                   string Body = @" <pre style='font-family:Calibri; font-size:medium'><b>Dear Cemex employee ,</b><br/>
         //Your New Passwword is :  </pre><br/>";
         //                   Body += "<pre style='font-family:Calibri; font-size:medium'>" + GeneratePass + "</pre>";
         //                   Body += "<br/> You can change your password from your profile page after login by above password.";
         //                   Body += " <br/><pre style='font-family:Calibri;font-size:medium'> <b>Best Regards,</b><br/>";
         //                   Body += "CEMEX Team <br/>";
         //                   Body += "This is an automatic Email generated by CEMEX Team ,Please do not reply to this Email </pre>";
         //                   AdminHelper.SendMail(null, updateUser.Email.Trim(), null, "CEMEX Exam Management User Password", Body, null);
         //               }
         //               else
         //               {
         //                   SecUser secUser1 = adminUser.GetUserAD(AdminHelper.ConfigurationManager.AppSetting["ADDomain"], userViewModel.Email.Trim());
         //               }
                        adminUser.Update(id, updateUser);
                        ViewBag.Message = "User Updated Successfully";
                        return View(userViewModel);

                    }
                    else
                    {
                        return View(userViewModel);
                    }
                }
                else
                {
                    return View(userViewModel);
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return View(userViewModel);
            }
        }

        [CustomPrivilege]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                UserDbRepository.Delete(id);
                ViewBag.Message = "User Deleted Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [CustomPrivilege]
        public ActionResult UnDelete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                UserDbRepository.UnDelete(id);
                ViewBag.Message = "User Activated Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [CustomPrivilege]
        public IActionResult Search()
        {
            AdminUser adminUser = new AdminUser();
            ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult Search(UserViewModel userViewModel)
        {
            AdminUser adminUser = new AdminUser();
            ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");

            if (userViewModel.RoleID == -1 && string.IsNullOrEmpty(userViewModel.FirstName) 
                && string.IsNullOrEmpty(userViewModel.FullName) && string.IsNullOrEmpty(userViewModel.Email)
                && string.IsNullOrEmpty(userViewModel.Mobile))
            {
                ModelState.AddModelError("", "Please insert searching data.");
                return View(userViewModel);
            }
            // To make isValid should make viewModel to avoid null objects which not allow null at DB Model schema
            IList<SecUser> userslist = adminUser.Search(userViewModel.FirstName, userViewModel.FullName,
                userViewModel.RoleID, userViewModel.Email, userViewModel.Mobile);
            if (userslist.Count > 0)
            {
                return View("SearchResult", userslist);
            }
            else
            {
                ViewBag.Message = "No Data Found";
                return View("Search", userViewModel);
            }


        }

        [HttpPost]
        public IActionResult GetUserAD(UserViewModel userViewModel)
        {
            AdminUser adminUser = new AdminUser();
            ViewData["RoleID"] = new SelectList(adminUser.GetRoles(), "ID", "Name");
            ModelState.Clear();
            if (!string.IsNullOrEmpty(userViewModel.Email))
            {
                
                SecUser secUser1 = adminUser.GetUserAD(AdminHelper.ConfigurationManager.AppSetting["ADDomain"], userViewModel.Email.Trim());
                if (secUser1 != null)
                {
                    //TempData["SecUser"] = secUser1;
                    ViewBag.FirstName = secUser1.FirstName;
                    ViewBag.FullName = secUser1.LastName;
                    ViewBag.Username = secUser1.Username;
                    ModelState.Clear();
                    return View("Create", userViewModel);
                }
                else
                {
                    ModelState.AddModelError("Email", "This Email doesn't have data in Active Directory");
                    return View("Create");

                }    
            }
            else
            {
                ModelState.AddModelError("Email", "The Email Field is mandatory");
                return View("Create");
            }
          
        }

       

      
    }
}