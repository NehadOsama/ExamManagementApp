using CemexExamApp.DBCore;
using CemexExamApp.Models;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Pdf.Content.Objects;

namespace CemexExamApp.Controllers
{
    public class ReportController : Controller
    {
        [CustomPrivilege]
        public IActionResult Index()
        {
            return View();
        }

        [CustomPrivilege]
        public IActionResult ExamPerLevels()
        {   
            AdminReport adminReport = new AdminReport();
            return View(adminReport.GetNumberOfExamPerLevel());
        }

        [CustomPrivilege]
        public IActionResult TopicsCount()
        {
            AdminReport adminReport = new AdminReport();
            ViewBag.Count = adminReport.GetNumberOfTopic();
            return View();
        }

        [CustomPrivilege]
        public IActionResult QuestionsPerLevels()
        {
            AdminReport adminReport = new AdminReport();
            return View(adminReport.GetNumberOfQuestionPerLevel());
        }

        [CustomPrivilege]
        public IActionResult QuestionsPerTopic()
        {
            AdminReport adminReport = new AdminReport();
            return View(adminReport.GetNumberOfQuestionPerTopic());
        }
    }
}
