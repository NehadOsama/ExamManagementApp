using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CemexExamApp.ContextData;
using CemexExamApp.Models;
using CemexExamApp.DBCore;
using CemexExamApp.Repository;
using CemexExamApp.ViewModel;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CemexExamApp.Controllers
{
    public class QuestionAnswerController : Controller
    {
        private readonly ExamManagmentAppContext _context;
        private readonly ICemexManagExam<Topic> topicRepository;
        private readonly ICemexManagExam<Question> questionRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public QuestionAnswerController(ExamManagmentAppContext context 
            , ICemexManagExam<Topic> TopicRepository
            , ICemexManagExam<Question> QuestionRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            topicRepository = TopicRepository;
            questionRepository = QuestionRepository;
            this.httpContextAccessor = httpContextAccessor;
        }


        // GET: QuestionAnswer/Create
        [CustomPrivilege]
        public IActionResult Create()
        {
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            ViewData["AnswerCountID"] = new SelectList(adminQuestionAnswer.GetAnswerCountList(), "ID", "Count");
            ViewData["CorrectAnswerID"] = new SelectList(adminQuestionAnswer.GetRightAnswerList(), "ID", "Count");
            ViewData["LevelID"] = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");
            var TopicQuery = topicRepository.List().Select(p => new { ID = p.ID, DisplayText = p.EnglishName.ToString() + " / " + p.ArabicName });

            ViewBag.TopicID = new SelectList(TopicQuery, "ID", "DisplayText");
            return View();
        }

        
        // POST: QuestionAnswer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create( QuestionAnswerViewModel questionAnswerViewModel)
        {
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();

            if (questionRepository.IsNamesExist(questionAnswerViewModel.EnglishName, questionAnswerViewModel.ArabicName))
            {
                ModelState.AddModelError("", "MES3 : data already Exist please review data again.");
            }
           
            if (ModelState.IsValid)
            {
               
                Question newquestion = new Question()
                {
                    EnglishName = questionAnswerViewModel.EnglishName,
                    ArabicName = questionAnswerViewModel.ArabicName,
                    Active = questionAnswerViewModel.Active,
                    AnswerCountID = questionAnswerViewModel.AnswerCountID,
                    CorrectAnswerID = questionAnswerViewModel.CorrectAnswerID,
                    LevelID = questionAnswerViewModel.LevelID,
                    TopicID = questionAnswerViewModel.TopicID,
                    CreateDate = DateTime.Now,
                    CreatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value

            };

                if (questionAnswerViewModel.AnswerCountID == 4) 
                {
                    questionAnswerViewModel.EnAns5 = null;
                    questionAnswerViewModel.ArAns5 = null;
                }
                if (questionAnswerViewModel.AnswerCountID == 3)
                {
                    questionAnswerViewModel.EnAns4 = null;
                    questionAnswerViewModel.ArAns4 = null;
                    questionAnswerViewModel.EnAns5 = null;
                    questionAnswerViewModel.ArAns5 = null;
                }

                QuestionAnswer newquestionAnswer = new QuestionAnswer() 
                {
                     Question= newquestion,

                     ArAns1 = questionAnswerViewModel.ArAns1,
                     ArAns2 = questionAnswerViewModel.ArAns2,   
                     ArAns3 = questionAnswerViewModel.ArAns3,
                     ArAns4 = questionAnswerViewModel.ArAns4,
                     ArAns5 = questionAnswerViewModel.ArAns5,

                     EnAns1 = questionAnswerViewModel.EnAns1,
                     EnAns2 = questionAnswerViewModel.EnAns2,
                     EnAns3 = questionAnswerViewModel.EnAns3,
                     EnAns4 = questionAnswerViewModel.EnAns4,
                     EnAns5 = questionAnswerViewModel.EnAns5
                     
                };
                
                adminQuestionAnswer.SubmitNewQuestion(newquestion, newquestionAnswer);

                ViewBag.Message = "Question Added Succssfully.";
            }
            else
            {
                ModelState.AddModelError("", "Answer Count is :"+ questionAnswerViewModel.AnswerCountID +" , You should Enter below Required fields.")    ;
            }
            ViewData["AnswerCountID"] = new SelectList(adminQuestionAnswer.GetAnswerCountList(), "ID", "Count");
            ViewData["CorrectAnswerID"] = new SelectList(adminQuestionAnswer.GetRightAnswerList(), "ID", "Count");
            ViewData["LevelID"] = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");
            var TopicQuery = topicRepository.List().Select(p => new { ID = p.ID, DisplayText = p.EnglishName.ToString() + " / " + p.ArabicName });

            ViewBag.TopicID = new SelectList(TopicQuery, "ID", "DisplayText");
            return View(questionAnswerViewModel);
        }

        // GET: QuestionAnswer/Edit/5
        [CustomPrivilege]
        public IActionResult Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            var ExistquestionAnswer = adminQuestionAnswer.GetQueAnsByQueId(id.Value);
            if (ExistquestionAnswer == null)
            {
                return NotFound();

            }
            ViewData["AnswerCountID"] = new SelectList(adminQuestionAnswer.GetAnswerCountList(), "ID", "Count");
            ViewData["CorrectAnswerID"] = new SelectList(adminQuestionAnswer.GetRightAnswerList(), "ID", "Count");
            ViewData["LevelID"] = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");
            ViewBag.TopicID = new SelectList(topicRepository.List(), "ID", "EnglishName");

            QuestionAnswerViewModel questionAnswerViewModel = new QuestionAnswerViewModel()
            {
                ID = ExistquestionAnswer.ID,
                EnglishName = ExistquestionAnswer.Question.EnglishName,
                ArabicName = ExistquestionAnswer.Question.ArabicName,
                TopicID = ExistquestionAnswer.Question.TopicID,
                LevelID = ExistquestionAnswer.Question.LevelID,
                Active = ExistquestionAnswer.Question.Active,
                AnswerCountID = ExistquestionAnswer.Question.AnswerCountID,
                CorrectAnswerID = ExistquestionAnswer.Question.CorrectAnswerID,
                ArAns1 = ExistquestionAnswer.ArAns1,
                ArAns2 = ExistquestionAnswer.ArAns2,
                ArAns3 = ExistquestionAnswer.ArAns3,
                ArAns4 = ExistquestionAnswer.ArAns4,
                ArAns5 = ExistquestionAnswer.ArAns5,
                EnAns1 = ExistquestionAnswer.EnAns1,
                EnAns2 = ExistquestionAnswer.EnAns2,
                EnAns3 = ExistquestionAnswer.EnAns3,
                EnAns4 = ExistquestionAnswer.EnAns4,
                EnAns5 = ExistquestionAnswer.EnAns5
               
            };


            return View(questionAnswerViewModel);
        }

        // POST: QuestionAnswer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(long id, QuestionAnswerViewModel questionAnswerViewModel)
        {
            if (id != questionAnswerViewModel.ID)
            {
                return NotFound();
            }
            if (questionRepository.IsNamesExistBefore(questionAnswerViewModel.EnglishName, questionAnswerViewModel.ArabicName, questionAnswerViewModel.ID))
            {
                ModelState.AddModelError("", "MES3 : data already Exist please review data again.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
                    ViewData["AnswerCountID"] = new SelectList(adminQuestionAnswer.GetAnswerCountList(), "ID", "Count", questionAnswerViewModel.AnswerCountID);
                    ViewData["CorrectAnswerID"] = new SelectList(adminQuestionAnswer.GetRightAnswerList(), "ID", "Count", questionAnswerViewModel.CorrectAnswerID);
                    ViewData["LevelID"] = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name", questionAnswerViewModel.LevelID);
                    ViewBag.TopicID = new SelectList(topicRepository.List(), "ID", "EnglishName", questionAnswerViewModel.TopicID);

                    QuestionAnswer OldquestionAnswer = adminQuestionAnswer.GetQuestionAnswer(questionAnswerViewModel.ID);
                    OldquestionAnswer.ArAns1 = questionAnswerViewModel.ArAns1;
                    OldquestionAnswer.ArAns2 = questionAnswerViewModel.ArAns2;
                    OldquestionAnswer.ArAns3 = questionAnswerViewModel.ArAns3;
                    OldquestionAnswer.ArAns4 = questionAnswerViewModel.ArAns4;
                    OldquestionAnswer.ArAns5 = questionAnswerViewModel.ArAns5;

                    OldquestionAnswer.EnAns1 = questionAnswerViewModel.EnAns1;
                    OldquestionAnswer.EnAns2 = questionAnswerViewModel.EnAns2;
                    OldquestionAnswer.EnAns3 = questionAnswerViewModel.EnAns3;
                    OldquestionAnswer.EnAns4 = questionAnswerViewModel.EnAns4;
                    OldquestionAnswer.EnAns5 = questionAnswerViewModel.EnAns5;

                    
                    OldquestionAnswer.Question.EnglishName = questionAnswerViewModel.EnglishName;
                    OldquestionAnswer.Question.ArabicName = questionAnswerViewModel.ArabicName;
                    OldquestionAnswer.Question.TopicID = questionAnswerViewModel.TopicID;
                    OldquestionAnswer.Question.LevelID = questionAnswerViewModel.LevelID;
                    OldquestionAnswer.Question.Active = questionAnswerViewModel.Active;
                    OldquestionAnswer.Question.AnswerCountID = questionAnswerViewModel.AnswerCountID;
                    OldquestionAnswer.Question.CorrectAnswerID = questionAnswerViewModel.CorrectAnswerID;
                    OldquestionAnswer.Question.LastUpdatedDate = DateTime.Now;
                    OldquestionAnswer.Question.LastUpdatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                    adminQuestionAnswer.SaveUpdates();
                    ViewBag.Message = "Question Updated Succssfully.";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Exception: " + ex.Message);
                    return View(questionAnswerViewModel);
                }
               
            }
            
            return View(questionAnswerViewModel);
        }

      
        public JsonResult RightAnswerList(int AnswerCountID)
        {
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            ViewData["CorrectAnswerID"] = new SelectList(adminQuestionAnswer.GetRightAnswerListByAnswerCount(AnswerCountID), "ID", "Count");
            return Json(ViewData["CorrectAnswerID"]) ;
       
        }

       
    }
}
