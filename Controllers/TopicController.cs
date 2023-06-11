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
using System.Security.Claims;
using CemexExamApp.ViewModel;

namespace CemexExamApp.Controllers
{
    public class TopicController : Controller
    {
        private readonly ICemexManagExam<Topic> topicRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TopicController(ICemexManagExam<Topic> TopicRepository, IHttpContextAccessor httpContextAccessor)
        {
            topicRepository = TopicRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        // GET: Topic
        [CustomPrivilege]
        public IActionResult Index()
        {
            return View(topicRepository.List());
        }



        // GET: Topic/Create
        [CustomPrivilege]
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Topic/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TopicViewModel topic)
        {
            try
            {
                if (topicRepository.IsNamesExist(topic.EnglishName, topic.ArabicName))
                {
                    ModelState.AddModelError("", "MES3 : data already Exist please review data again.");
                }

                if (ModelState.IsValid)
                {
                    Topic topic1 = new Topic();
                    topic1.EnglishName = topic.EnglishName;
                    topic1.ArabicName = topic.ArabicName;
                    topic1.CreateDate = DateTime.Now;
                    topic1.CreatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                    topicRepository.Add(topic1);
                    ViewBag.Message = "Topic added Successfully";
                    return View(nameof(Index), topicRepository.List());
                }
                return View(topic);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return View(topic);
            }
        }

        // GET: Topic/Edit/5
        [CustomPrivilege]
        public IActionResult Edit(int? id)
        {
            AdminExam adminExam = new AdminExam();
        
            if (id == null)
            {
                return NotFound();
            }

            var topic = topicRepository.Find(id.Value);
            if (topic == null)
            {
                return NotFound();
            }
            else
            {
                if (adminExam.CanNotEditTopic(id.Value))
                {
                    ModelState.AddModelError("", "This Topic cannot be edited, There is an available Exam contains this topic.");
                    return View("Index", topicRepository.List());
                }

                TopicViewModel topicViewModel = new TopicViewModel() { EnglishName = topic.EnglishName, ArabicName= topic.ArabicName,ID= topic.ID };
                return View(topicViewModel);
            }
           
        }

        // POST: Topic/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id,TopicViewModel topicViewModel)
        {
            try
            {
                if (id != topicViewModel.ID)
                {
                    return NotFound();
                }
                if (topicRepository.IsNamesExistBefore(topicViewModel.EnglishName, topicViewModel.ArabicName, topicViewModel.ID))
                {
                    ModelState.AddModelError("", "MES3 : data already Exist please review data again.");
                }
                if (ModelState.IsValid)
                {

                    Topic topic1 = topicRepository.Find(topicViewModel.ID);
                    topic1.EnglishName = topicViewModel.EnglishName;
                    topic1.ArabicName = topicViewModel.ArabicName;
                    topic1.LastUpdatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;
                    topic1.LastUpdatedDate = DateTime.Now;
                    topicRepository.Update(id, topic1);
                    ViewBag.Message = "Topic Updated Successfully";
                    return View(nameof(Index), topicRepository.List());
                }
                return View(topicViewModel);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Exception: " + ex.Message);
                return View(topicViewModel);
            }

        }

        

    }
}
