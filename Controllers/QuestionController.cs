using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CemexExamApp.ContextData;
using CemexExamApp.Models;
using CemexExamApp.Repository;
using CemexExamApp.DBCore;
using CemexExamApp.ViewModel;

namespace CemexExamApp.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ExamManagmentAppContext _context;
        private readonly ICemexManagExam<Question> questionDbRepository;

        public QuestionController(ExamManagmentAppContext context , ICemexManagExam<Question> questionDbRepository)
        {
            _context = context;
            this.questionDbRepository = questionDbRepository;
        }

        // GET: Question
        [CustomPrivilege]
        public IActionResult Index()
        {
            return View(questionDbRepository.List().ToList());
        }

        [CustomPrivilege]
        public IActionResult Search()
        {
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            ViewBag.LevelID = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");
        
            return View();
        }

        [HttpPost]
        public IActionResult Search(Question question)
        {
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            ViewBag.LevelID = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");

            if (question.LevelID == -1 && string.IsNullOrEmpty(question.EnglishName) && string.IsNullOrEmpty(question.Topic.EnglishName))
            {
                ModelState.AddModelError("", "Please insert searching data.");
                return View(question);
            }
            // To make isValid should make viewModel to avoid null objects which not allow null at DB Model schema
                IList<Question> questionList = questionDbRepository.Search(question.EnglishName, question.Topic.EnglishName, question.LevelID);
            if (questionList.Count > 0)
            {
                return View("SearchResult", questionList);
            }
            else
            {
                ViewBag.Message = "No Data Found";
                return View("Search", question);
            }
          
          
        }

        [CustomPrivilege]
        public ActionResult Delete(int id)
        {
            try
            {
                // TODO: Add delete logic here
                questionDbRepository.Delete(id);
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
                questionDbRepository.UnDelete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        private bool QuestionExists(long id)
        {
          return _context.Questions.Any(e => e.ID == id);
        }
    }
}
