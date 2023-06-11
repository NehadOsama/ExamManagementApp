using CemexExamApp.ContextData;
using CemexExamApp.Models;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Security.Cryptography;
using TheInventory.Models;
using System.Net.Mail;

namespace CemexExamApp.DBCore
{
    public class AdminUser
    {

        ExamManagmentAppContext dc = new ExamManagmentAppContext();
        public List<RoleRouting> FindControllerActionByGroupId(int Roleid)
        {
            var perList = dc.RoleRoutings.Include(x => x.ControllerAction).Where(x => x.RoleId == Roleid).ToList().ToList();
            return perList;
        }

        public bool LoginAD(string username, string password)
        {
            PrincipalContext pc1;

            bool result = false;

            try
            {
                pc1 = new PrincipalContext(ContextType.Domain, System.Configuration.ConfigurationManager.AppSettings["TelecomEgyptDomain"], "DC=Cairo,DC=TelecomEgypt,DC=corp");
                result = pc1.ValidateCredentials(username, password);
            }
            catch (Exception)
            {
                
            }

            return result;
        }

        public bool IsValidPassword(string password, string hashPass)
        {
            bool result = true;

            byte[] hashBytes = Convert.FromBase64String(hashPass);
            byte[] salt = new byte[20];
            Array.Copy(hashBytes, 0, salt, 0, 20);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(44);

            for (int i = 0; i < 44; i++)
            {
                if (hashBytes[i + 20] != hash[i])
                {
                    //throw new UnauthorizedAccessException();
                    result = false;
                }
            }

            return result;
        }
        public SecUser Find(string Username)
        {
            var secUser = dc.SecUsers.Include(x => x.Role).SingleOrDefault(x => x.Username.ToLower() == Username.Trim().ToLower() && x.Active);
            return secUser;
        }
        public SecUser Find(int id)
        {
            var secUser = dc.SecUsers.Include(x => x.Role).SingleOrDefault(x => x.ID == id);
            return secUser;
        }
        public void Update(int id, SecUser newentity)
        {

            dc.SecUsers.Update(newentity);
            dc.SaveChanges();
        }

        public SecUser GetUserByUserName(string username)
        {
            var data = dc.SecUsers.FirstOrDefault(x => x.Username.ToLower() == username.ToLower());
            return data;
        }

        public SecUser GetUserByEmail(string Email)
        {
            var data = dc.SecUsers.FirstOrDefault(x => x.Email.ToLower() == Email.ToLower());
            return data;
        }

        public SecUser GetActiveUserByEmail(string Email)
        {
            var data = dc.SecUsers.FirstOrDefault(x => x.Email.ToLower() == Email.ToLower() && x.Active && !x.AD);
            return data;
        }

        public SecUser GetUserByUserNameBefore(string username, int RecordId)
        {
            var data = dc.SecUsers.FirstOrDefault(x => x.Username.ToLower() == username.ToLower() && x.ID != RecordId);
            return data;
        }

        public SecUser GetUserByEmailBefore(string Email, int RecordId)
        {
            var data = dc.SecUsers.FirstOrDefault(x => x.Email.ToLower() == Email.ToLower() && x.ID != RecordId);
            return data;
        }
        public IList<SecUser> Search(string FirstName, string lastName, int RoleId, string Email,string Mob)
        {
            return dc.SecUsers.Include(x => x.Role)
                              .Where(x => (
                                           (FirstName == null || x.FirstName.ToLower().Contains(FirstName.ToLower())) &&
                                           (lastName == null || x.LastName.ToLower().Contains(lastName.ToLower())) &&
                                           (Email == null || x.Email.ToLower().Contains(Email.ToLower())) &&
                                           (Mob == null || x.Mobile.ToLower().Contains(Mob.ToLower())) &&
                                           (RoleId == -1 || x.RoleID == RoleId)
                                          )).ToList();
        }
        public List<Role> GetRoles()
        {
           return  dc.Roles.ToList();
        }

        public string GetRandomPassword(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789~`!@#$%^&*()-_=+[]"; ;
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        public string GetHashPassword(string password)
        {
            string hashPass = string.Empty;

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[20]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(44);

            byte[] hashBytes = new byte[64];
            Array.Copy(salt, 0, hashBytes, 0, 20);
            Array.Copy(hash, 0, hashBytes, 20, 44);

            hashPass = Convert.ToBase64String(hashBytes);

            return hashPass;
        }
   
       
        public SecUser GetUserAD(string domain, string Email)
        {
            PrincipalContext PrincipalContextObj = new PrincipalContext(ContextType.Domain, domain);
            DirectoryEntry DirectoryEntryObj = new DirectoryEntry("LDAP://" + domain);
            DirectorySearcher DirectorySearcherObj = new DirectorySearcher(DirectoryEntryObj, "(&(objectCategory=Person)(objectClass=user)(mail=" + Email + "))");
            DirectorySearcherObj.PageSize = 100000;
            SearchResult item = DirectorySearcherObj.FindOne();

            if (item != null)
            {
                
                DirectoryEntry userEntry = item.GetDirectoryEntry();

                return new SecUser()
                    {
                        Username = userEntry.Properties["samaccountName"].Value.ToString(),
                        FirstName = userEntry.Properties["givenname"].Value.ToString(),
                    LastName = userEntry.Properties["displayname"].Value.ToString(),
                        Email = userEntry.Properties["mail"].Value.ToString()
                        
                      };
                
            }

            return null;
        }
    }
}
