using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using CemexExamApp.ContextData;
using CemexExamApp.Models;

namespace CemexExamApp.DBCore
{
    public static class AdminHelper
    {
        public static string getFromAppSetting(string Key , object datatype)
        {
            var configuration = new ConfigurationBuilder()
                          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                          .AddJsonFile("appsettings.json")
                          .Build();
            return configuration.GetValue<string>(Key);
        }

       public static class ConfigurationManager
        {
            public static IConfiguration AppSetting { get; }
            static ConfigurationManager()
            {
                AppSetting = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();
            }
        }
        public static void SendMail(string from, string to, string cc, string subject, string body, string attachmentFilePath)
        {

            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                //mail.To.Add("karim.fawzy@tedata.net");
                // var addto = Dts.Variables["User::mailto"].Value.ToString();
                foreach (var addressto in to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.To.Add(addressto);
                }

                //  var addcc = Dts.Variables["User::mailcc"].Value.ToString();
                if (cc != null)
                {
                    foreach (var addresscc in cc.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        mail.CC.Add(addresscc);
                    }
                }

                mail.Subject = subject;
                mail.Body = body;
                //  mail.From = new MailAddress(from);
                mail.IsBodyHtml = true;
                mail.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSetting["MailServerFromMAil"].ToString());
                // mail.Attachments.Add(new System.Net.Mail.Attachment("E:\\Applications\\SSISPackage\\OMSPackges\\OMSFiles\\Simba_Start_Date _Cpe_Inventory.xlsx"));
                // mail.Attachments.Add(new System.Net.Mail.Attachment("E:\\Applications\\SSISPackage\\OMSPackges\\OMSFiles\\Simba_Start_Date _Welcome_Calls.xlsx"));
                System.Net.Mail.SmtpClient Srv = new System.Net.Mail.SmtpClient();
                /////// Srv.Host = "10.19.254.30";
                Srv.Host = ConfigurationManager.AppSetting["MailServerIP"].ToString();
                Srv.Port = int.Parse(ConfigurationManager.AppSetting["MailServerPort"].ToString());
                Srv.Credentials = new NetworkCredential(ConfigurationManager.AppSetting["MailServerUser"].ToString(),
                    ConfigurationManager.AppSetting["MailServerPass"].ToString());
                Srv.EnableSsl = false;
                if (!string.IsNullOrEmpty(attachmentFilePath))
                {
                    mail.Attachments.Add(new Attachment(attachmentFilePath));
                }
                Srv.Send(mail);
                // Dts.Variables["User::MailFail"].Value = 100;
            }
            catch (Exception)
            {
                // Dts.Variables["User::MailFail"].Value = int.Parse(Dts.Variables["User::MailFail"].Value.ToString()) + 1;

            }
        }

        public static string UploadFile(IFormFile file, string fullPath)
        {
            try
            {
                if (file != null)
                {
                    var n = new FileStream(fullPath, FileMode.Create);
                    file.CopyTo(n);
                    n.Flush();
                    n.Close();

                    return file.FileName;
                }
                return "Please set a file to upload.";
            }
            catch (Exception ex)
            {
                return "False" + ex.Message;
            }
        }
        public static bool IsHashValid(string Value, string hashValue)
        {
            bool result = true;

            byte[] hashBytes = Convert.FromBase64String(hashValue);
            byte[] salt = new byte[20];
            Array.Copy(hashBytes, 0, salt, 0, 20);
            var pbkdf2 = new Rfc2898DeriveBytes(Value, salt, 100000);
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
        public static string GetHashPassword(string Value)
        {
            string hashPass = string.Empty;

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[20]);

            var pbkdf2 = new Rfc2898DeriveBytes(Value, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(44);

            byte[] hashBytes = new byte[64];
            Array.Copy(salt, 0, hashBytes, 0, 20);
            Array.Copy(hash, 0, hashBytes, 20, 44);

            hashPass = Convert.ToBase64String(hashBytes);

            return hashPass;
        }

        public static void LogException(ExceptionLog exceptionLog)
        {
            ExamManagmentAppContext dc = new ExamManagmentAppContext();
            dc.ExceptionLogs.Add(exceptionLog);
            dc.SaveChanges();
        }

    }
}
