using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.WebRequestMethods;
using System.Configuration;

namespace SendLinkExam
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:7253/ExamTaker/SendMailExamURL");


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)768;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            string RootURL = ConfigurationManager.AppSettings["RootURL"];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RootURL + "/ExamTaker/SendMailExamURL");
            request.Method = "GET";
            //specify other request properties
            Console.WriteLine("End");
            try
            {
              var  response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}
