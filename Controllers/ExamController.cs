using ArrayToPdf;
using CemexExamApp.DBCore;
using CemexExamApp.Models;
using CemexExamApp.Repository;
using CemexExamApp.ViewModel;
using IronPdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using NReco.PdfGenerator;
using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Text;
using Wkhtmltopdf.NetCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace CemexExamApp.Controllers
{
    public class ExamController : Controller
    {
        private readonly ICemexManagExam<Topic> topicRepository;
        private readonly ICemexManagExam<Question> questionRepository;
        private readonly IViewRenderService viewRenderService;
        private readonly IWebHostEnvironment hosting;
        private readonly IGeneratePdf generatePdf;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ExamController(ICemexManagExam<Topic> TopicRepository
                                , ICemexManagExam<Question> QuestionRepository
                                 ,IViewRenderService viewRenderService
                                 , IWebHostEnvironment hosting
            ,IGeneratePdf generatePdf
            , IHttpContextAccessor httpContextAccessor)
        {
            topicRepository = TopicRepository;
            questionRepository = QuestionRepository;
            this.viewRenderService = viewRenderService;
            this.hosting = hosting;
            this.generatePdf = generatePdf;
            this.httpContextAccessor = httpContextAccessor;
        }

        [CustomPrivilege]
        public IActionResult Index()
        {
            AdminExam adminExam = new AdminExam();
            return View(adminExam.GetExamList());
        }


        // GET
        [CustomPrivilege]
        public IActionResult Create()
        {
            AdminExam adminExam = new AdminExam();

            ViewData["LanguageID"] = new SelectList(adminExam.GetLanguageList(), "ID", "Name");
            ViewData["BenchmarkID"] = new SelectList(adminExam.GetBenchmarkList(), "ID", "Name");
            var DurationQuery = adminExam.GetDurationList().Select(p => new { ID = p.ID, DisplayText = p.Duration1.ToString() + " MIN"});

            ViewData["DurationID"] = new SelectList(DurationQuery, "ID", "DisplayText");
            ViewData["LevelID"] = new SelectList(adminExam.GetLevelList(), "ID", "Name");
            var TopicQuery = topicRepository.List().Select(p => new { ID = p.ID, DisplayText = p.EnglishName.ToString() + " / " + p.ArabicName });

            ViewData["TopicID"] = new SelectList(TopicQuery, "ID", "DisplayText");
            return View();
        }


        
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ExamViewModel examViewModel)
        {
            AdminExam adminExam= new AdminExam();
            AdminQuestionAnswer adminQuestionAnswer= new AdminQuestionAnswer();


            examViewModel.level = new Level();

            if (adminExam.IsNamesExist(examViewModel.TrainingName, examViewModel.TrainingFromDate, examViewModel.TrainingToDate))
            {
                ModelState.AddModelError("", "MES3 : data already Exist please review data again.");
            }
            if (examViewModel.NumberOfQuestions > adminQuestionAnswer.GetNoOfActiveQuestion())
            {
                ModelState.AddModelError("", "Please revisit bank questions as there are not enough questions to be added.");

            }
            if (examViewModel.TopicsList.Count() != examViewModel.NumberOfTopics)
            {
                ModelState.AddModelError("", "You select Number of Topics :" + examViewModel.NumberOfTopics.ToString() + " but the selected topics count is : " + examViewModel.TopicsList.Count());
            }
            if (examViewModel.NumberOfTopics * examViewModel.NumberOfQuestionsPerTopic != examViewModel.NumberOfQuestions)
            {
                ModelState.AddModelError("", "Please review Number of questions as it should be greater than Number of Questions per each Topic.");
            }
            if (adminQuestionAnswer.IsNoOfQPTEqualExistQ(examViewModel.TopicsList, examViewModel.NumberOfQuestionsPerTopic, examViewModel.LevelID))
            {
                ModelState.AddModelError("", "Please revisit bank questions as there are not enough questions per each topic equal to entered count.");

            }
            if (ModelState.IsValid)
            {

                

                if ((examViewModel.TakersSheet != null) && (examViewModel.TakersSheet.FileName != ""))
                {
                    if (examViewModel.TakersSheet.FileName.EndsWith(".xlsx"))
                    {

                        var fileSize = examViewModel.TakersSheet.Length;
                        if (fileSize > (25 * 1024 * 1024))
                        {

                            ModelState.AddModelError("", "File size is too large. Maximum file size permitted is 25 MB");
                          
                        }

                        DataTable ExcelData = new DataTable();

                        string filePath = examViewModel.TakersSheet.FileName + "_" +
                            /*session.CurrentUsername.Replace(".", "") + "_" +*/ DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
                        var fullPath = Path.Combine(hosting.WebRootPath, "uploads", "Takers", filePath);


                        try
                        {
                            string res = UploadFile(examViewModel.TakersSheet, fullPath);

                            ExcelData = Import(fullPath);
                            
                            if (!ExamTakers_ValidateFile(ExcelData))
                            {
                                ModelState.AddModelError("", "Excel Sheet Not Have Required Colmuns. Please See The Sample.");
                            }
                            else if (ExcelData.Rows.Count > 1000)
                            {
                                ModelState.AddModelError("", "Maximum Number of Record is 1000.");
                            }
                            else if (ExcelData.Rows.Count == 0)
                            {
                                ModelState.AddModelError("", "File is empty.");
                            }
                            else
                            {

                                Exam exam = new Exam()
                                {
                                    ValidityDateFrom = examViewModel.ValidityDateFrom,
                                    ValidityDateTo = examViewModel.ValidityDateTo,
                                    LanguageID = examViewModel.LanguageID,
                                    LevelID = examViewModel.LevelID,
                                    BenchmarkID = examViewModel.BenchmarkID,
                                    DurationID = examViewModel.DurationID,
                                    QuestionsCount = examViewModel.NumberOfQuestions,
                                    TopicsCount = examViewModel.NumberOfTopics,
                                    QuestionPerTopicCount = examViewModel.NumberOfQuestionsPerTopic,
                                    TakersSheetPath = fullPath,
                                    CreateDate = DateTime.Now,
                                    CreatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value

                            };
                                training training = new training()
                                {
                                    Name = examViewModel.TrainingName,
                                    FromDate = examViewModel.TrainingFromDate,
                                    ToDate = examViewModel.TrainingToDate,
                                    Exam = exam
                                };

                                List<ExamTaker> examTakerList = new List<ExamTaker>();
                                foreach (DataRow item in ExcelData.Rows)
                                {

                                    if (string.IsNullOrEmpty(item["Email"].ToString()) || string.IsNullOrEmpty(item["UserName"].ToString()))
                                        break;

                                    ExamTaker examTaker = new ExamTaker()
                                    {

                                        EMail = item["Email"].ToString().Replace("&nbsp;", "").Trim().Replace("&#160;", ""),
                                        Username = item["UserName"].ToString().Replace("&nbsp;", "").Trim().Replace("&#160;", ""),
                                        Exam = exam,
                                    };

                                    examTakerList.Add(examTaker);

                                }

                                List<ExamTopic> examTopics = new List<ExamTopic>();
                                foreach (int topicId in examViewModel.TopicsList)
                                {
                                    ExamTopic examTopic = new ExamTopic()
                                    {
                                        Exam = exam,
                                        TopicID = topicId
                                    };
                                    examTopics.Add(examTopic);
                                }

                              
                                List<ExamQuestion> examQuestionList = new List<ExamQuestion>();
                           
                               
                                List<Question> RandomQuestions = adminExam.GetRandomQuestionsPerTopic(examViewModel.NumberOfQuestionsPerTopic, examViewModel.LevelID, examViewModel.TopicsList);

                                foreach (Question question in RandomQuestions)
                                {
                                    ExamQuestion examQuestion = new ExamQuestion()
                                    {
                                        Question = question,
                                        Exam = exam
                                    };
                                    examQuestionList.Add(examQuestion);
                             }
                                    adminExam.SubmitNewExam(exam, training, examTopics, examTakerList,examQuestionList);
                                ViewBag.Message = "Exam Added Succssfully.";
                               
                            }

                        }
                        catch (Exception ex)
                        {

                            ModelState.AddModelError("", "Fail To Read File. Please Use The sample file. " + GetExceptionMessage(ex));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", " File extension Must Be xlsx.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", " No File Selected.") ;
                   
                }

            }
            else
            {
             ModelState.AddModelError("", "Page Data is not valid");
            }
            ViewData["LanguageID"] = new SelectList(adminExam.GetLanguageList(), "ID", "Name");
            ViewData["BenchmarkID"] = new SelectList(adminExam.GetBenchmarkList(), "ID", "Name");
            var DurationQuery = adminExam.GetDurationList().Select(p => new { ID = p.ID, DisplayText = p.Duration1.ToString() + " MIN" });

            ViewData["DurationID"] = new SelectList(DurationQuery, "ID", "DisplayText");
            ViewData["LevelID"] = new SelectList(adminExam.GetLevelList(), "ID", "Name");
            var TopicQuery = topicRepository.List().Select(p => new { ID = p.ID, DisplayText = p.EnglishName.ToString() + " / " + p.ArabicName });

            ViewData["TopicID"] = new SelectList(TopicQuery, "ID", "DisplayText");
            return View(examViewModel);
        }


        // GET
        [HttpGet]
        [CustomPrivilege]
        public IActionResult Edit(int ID)
        {
            AdminExam adminExam = new AdminExam();
            Exam exam=  adminExam.GetExamById(ID);
            ExamViewModel examViewModel = new ExamViewModel()
            {
                ExamID = exam.ID,
                BenchmarkID = exam.BenchmarkID,
                DurationID = exam.DurationID,
                LanguageID = exam.LanguageID,
                LevelID = exam.LevelID,
                NumberOfQuestions = exam.QuestionsCount,
                NumberOfQuestionsPerTopic = exam.QuestionPerTopicCount,
                NumberOfTopics = exam.TopicsCount,
                ValidityDateFrom = exam.ValidityDateFrom,
                ValidityDateTo = exam.ValidityDateTo,
                TrainingFromDate = exam.training.First().FromDate,
                TrainingToDate = exam.training.First().ToDate,
                TrainingName = exam.training.First().Name,
                TopicsList = exam.ExamTopics.Where(x => x.ExamID == ID).Select(x=>x.TopicID).ToList(),
                
                // Will show exist takers sheet
            };

            ViewData["LanguageID"] = new SelectList(adminExam.GetLanguageList(), "ID", "Name");
            ViewData["BenchmarkID"] = new SelectList(adminExam.GetBenchmarkList(), "ID", "Name");
            var DurationQuery = adminExam.GetDurationList().Select(p => new { ID = p.ID, DisplayText = p.Duration1.ToString() + " MIN" });

            ViewData["DurationID"] = new SelectList(DurationQuery, "ID", "DisplayText");
            ViewData["LevelID"] = new SelectList(adminExam.GetLevelList(), "ID", "Name");
            var TopicQuery = topicRepository.List().Select(p => new { ID = p.ID, DisplayText = p.EnglishName.ToString() + " / " + p.ArabicName });

            ViewData["TopicID"] = new SelectList(TopicQuery, "ID", "DisplayText");
            return View(examViewModel);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int ID,ExamViewModel examViewModel)
        {
            AdminExam adminExam = new AdminExam();
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            string fullPath = null;

            #region Fill DDL

            ViewData["LanguageID"] = new SelectList(adminExam.GetLanguageList(), "ID", "Name");
            ViewData["BenchmarkID"] = new SelectList(adminExam.GetBenchmarkList(), "ID", "Name");
            var DurationQuery = adminExam.GetDurationList().Select(p => new { ID = p.ID, DisplayText = p.Duration1.ToString() + " MIN" });

            ViewData["DurationID"] = new SelectList(DurationQuery, "ID", "DisplayText");
            ViewData["LevelID"] = new SelectList(adminExam.GetLevelList(), "ID", "Name");
            var TopicQuery = topicRepository.List().Select(p => new { ID = p.ID, DisplayText = p.EnglishName.ToString() + " / " + p.ArabicName });

            ViewData["TopicID"] = new SelectList(TopicQuery, "ID", "DisplayText");

            #endregion

            if (ID != examViewModel.ExamID)
            {
                ModelState.AddModelError("", "Please refresh and retry again.");
                return View(examViewModel);
            }

            if (examViewModel.ValidityDateFrom <= DateTime.Now)
            {
                ModelState.AddModelError("", "Can not update this Exam as Validity Date From is started.");
                return View(examViewModel);
            }

            if (adminExam.IsNamesExistBefore(examViewModel.TrainingName, examViewModel.TrainingFromDate, examViewModel.TrainingToDate, examViewModel.ExamID))
            {
                ModelState.AddModelError("", "MES3 : data already Exist please review data again.");
            }
            if (examViewModel.NumberOfQuestions > adminQuestionAnswer.GetNoOfActiveQuestion())
            {
                ModelState.AddModelError("", "Please revisit bank questions as there are not enough questions to be added.");
            }
            if (examViewModel.TopicsList.Count() != examViewModel.NumberOfTopics)
            {
                ModelState.AddModelError("", "You select Number of Topics :" + examViewModel.NumberOfTopics.ToString() + " but the selected topics count is : " + examViewModel.TopicsList.Count());
            }
            if (examViewModel.NumberOfTopics * examViewModel.NumberOfQuestionsPerTopic != examViewModel.NumberOfQuestions)
            {
                ModelState.AddModelError("", "Please review Number of questions as it should be greater than Number of Questions per each Topic.");
            }
            if (adminQuestionAnswer.IsNoOfQPTEqualExistQ(examViewModel.TopicsList, examViewModel.NumberOfQuestionsPerTopic, examViewModel.LevelID))
            {
                ModelState.AddModelError("", "Please revisit bank questions as there are not enough questions per each topic equal to entered count.");

            }
            
           // ModelState.SetModelValue("TakersSheet","","valid");

            if (ModelState.ContainsKey("TakersSheet"))
                ModelState["TakersSheet"].ValidationState = ModelValidationState.Valid;


           // ModelState.SetModelValue("PropertyID", new ValueProviderResult("New value", "", CultureInfo.InvariantCulture));

            if (ModelState.IsValid)
            {
                List<ExamTaker> examTakerList = new List<ExamTaker>();
                List<ExamTaker> ExistTakers = adminExam.GetTakersByExamId(ID);


                DataTable ExcelData = new DataTable();

                if (examViewModel.NeedUpdateSheet)
                {
                    if ((examViewModel.TakersSheet != null) && (examViewModel.TakersSheet.FileName != ""))
                    {
                        if (examViewModel.TakersSheet.FileName.EndsWith(".xlsx"))
                        {

                            var fileSize = examViewModel.TakersSheet.Length;
                            if (fileSize > (25 * 1024 * 1024))
                            {

                                ModelState.AddModelError("", "File size is too large. Maximum file size permitted is 25 MB");
                            }
                            string filePath = examViewModel.TakersSheet.FileName + "_" +
                                /*session.CurrentUsername.Replace(".", "") + "_" +*/ DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx";
                             fullPath = Path.Combine(hosting.WebRootPath, "uploads", "Takers", filePath);


                            try
                            {
                                string res = UploadFile(examViewModel.TakersSheet, fullPath);

                                ExcelData = Import(fullPath);


                                if (!ExamTakers_ValidateFile(ExcelData))
                                {
                                    ModelState.AddModelError("", "Excel Sheet Not Have Required Colmuns. Please See The Sample.");
                                }
                                else if (ExcelData.Rows.Count > 1000)
                                {
                                    ModelState.AddModelError("", "Maximum Number of Record is 1000.");
                                }
                                else if (ExcelData.Rows.Count == 0)
                                {
                                    ModelState.AddModelError("", "File is empty.");
                                }
                                else
                                {
                                 

                                    foreach (DataRow item in ExcelData.Rows)
                                    {

                                        if (string.IsNullOrEmpty(item["Email"].ToString()) || string.IsNullOrEmpty(item["UserName"].ToString()))
                                            break;

                                        ExamTaker examTaker = new ExamTaker()
                                        {

                                            EMail = item["Email"].ToString().Replace("&nbsp;", "").Trim().Replace("&#160;", ""),
                                            Username = item["UserName"].ToString().Replace("&nbsp;", "").Trim().Replace("&#160;", ""),
                                            ExamID = ID
                                        };


                                        examTakerList.Add(examTaker);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                                ModelState.AddModelError("", "Fail To Read File. Please Use The sample file. " + GetExceptionMessage(ex));
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", " File extension Must Be xlsx.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", " No File Selected.");

                    }
                }


                Exam exam = adminExam.GetExamById(ID);

                // update exit exam
                exam.ValidityDateFrom = examViewModel.ValidityDateFrom;
                exam.ValidityDateTo = examViewModel.ValidityDateTo;
                exam.LanguageID = examViewModel.LanguageID;
                exam.LevelID = examViewModel.LevelID;
                exam.BenchmarkID = examViewModel.BenchmarkID;
                exam.DurationID = examViewModel.DurationID;
                exam.QuestionsCount = examViewModel.NumberOfQuestions;
                exam.TopicsCount = examViewModel.NumberOfTopics;
                exam.QuestionPerTopicCount = examViewModel.NumberOfQuestionsPerTopic;
                if (examViewModel.NeedUpdateSheet) exam.TakersSheetPath = fullPath;
                exam.LastUpdatedDate = DateTime.Now;
                exam.LastUpdatedBy = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value;

                // update exit Training
                training training = adminExam.GetTraningByExamId(ID);
                training.Name = examViewModel.TrainingName;
                training.FromDate = examViewModel.TrainingFromDate;
                training.ToDate = examViewModel.TrainingToDate;

               
                //Delete old topics
                var exitTopics = adminExam.GetTopicsByExamId(ID);

                List<ExamTopic> examTopics = new List<ExamTopic>();
                foreach (int topicId in examViewModel.TopicsList)
                {
                    ExamTopic examTopic = new ExamTopic()
                    {
                        Exam = exam,
                        TopicID = topicId
                    };
                    // To inset updated topics
                    examTopics.Add(examTopic);
                }

                //Delete old examQuestions
                var exitExamQuestion = adminExam.GetExamQuestionsByExamId(ID);

                List<ExamQuestion> examQuestionList = new List<ExamQuestion>();
                List<Question> RandomQuestions = adminExam.GetRandomQuestionsPerTopic(examViewModel.NumberOfQuestionsPerTopic, examViewModel.LevelID, examViewModel.TopicsList);

                foreach (Question question in RandomQuestions)
                {
                    ExamQuestion examQuestion = new ExamQuestion()
                    {
                        Question = question,
                        Exam = exam
                    };
                    //to insert new random question list
                    examQuestionList.Add(examQuestion);
                }

                if (examViewModel.NeedUpdateSheet)
                {
                    adminExam.EditUpdates(exitTopics, examTopics, ExistTakers, examTakerList
                   , exitExamQuestion, examQuestionList);
                }
                else
                {
                    adminExam.EditUpdatesWithoutTakers(exitTopics, examTopics , exitExamQuestion, examQuestionList);
                }
               
                ViewBag.Message = "Exam updated Succssfully.";


            }
            else
            {
                ModelState.AddModelError("", "Page Data is not valid");
            }

            return View(examViewModel);
        }

        private bool ExamTakers_ValidateFile(DataTable dataTable)
        {
            string[] ColNames = { "UserName", "Email" };

            bool isvalid = true;

            foreach (string col in ColNames)
            {
                if (!dataTable.Columns.Contains(col))
                {
                    isvalid = false;
                    return false;
                }
            }

            return isvalid;
        }
        private string UploadFile(IFormFile file, string fullPath)
        {
            try
            {
                if (file != null)
                {
                    //string folderPath = Path.Combine(hosting.WebRootPath, "uploads");
                    //string fullPath = Path.Combine(folderPath, file.FileName);
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
        private static string GetExceptionMessage(Exception ex)
        {
            return ex.Message + "." + (ex.InnerException == null ? string.Empty : ex.InnerException.Message) + ".";
        }
        private DataTable Import(string filename)
        {

            string connectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 12.0;", filename);

                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {

                        try
                        {
                            conn.Open();
                            OleDbCommand objCmdSelect = new OleDbCommand("SELECT * FROM [data$]", conn);
                            OleDbDataAdapter objAdb = new OleDbDataAdapter(objCmdSelect);
                            DataSet dt = new DataSet();

                            objAdb.Fill(dt);
                            DataTable dtt = dt.Tables[0];
                            return dtt;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }

            
        }
        

        [CustomPrivilege]
        public async Task<FileResult> DownloadExam(long ID)
        {

            




            AdminExam adminExam = new AdminExam();
            ExamViewModel examViewModel = new ExamViewModel();
            
            // string re= View("search", examViewModel).ToString();

            // viewEngine.FindView(,,)

            

            AssessmentViewModel AssessmentViewModel = new AssessmentViewModel();
            List<QuestionwithAns> questionwithAns = new List<QuestionwithAns>();


            AssessmentViewModel.ExamTakerId = ID;
            AssessmentViewModel.exam = adminExam.GetExamById(ID);


            foreach (ExamQuestion item in AssessmentViewModel.exam.ExamQuestions)
            {
                questionwithAns.Add(new QuestionwithAns()
                {
                    question = item.Question,
                    QquAnswers = item.Question.QuestionAnswers.First()
                });
            }

            AssessmentViewModel.QuestionwithAns = questionwithAns;
           

            string htmllll =  await viewRenderService.RenderToStringAsync("/Views/Exam/AssessmentPDF.cshtml", AssessmentViewModel);
            // var html = GetHtml(Assessment(ID));
            //  var htmlContent = String.Format(htmllll,);
            // await generatePdf.GetPdfViewInHtml(htmllll, AssessmentViewModel);
           
            var pdf = generatePdf.GetPDF(htmllll);
            var pdfStream = new System.IO.MemoryStream();
            pdfStream.Write(pdf, 0, pdf.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf") { FileDownloadName= "Exam.pdf" };

            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
           var buyes= htmlConverter.GeneratePdf(htmllll);

            //Console.WriteLine("Hello World!");
            //var render = new IronPdf.ChromePdfRenderer();
            //using var doc = render.RenderUrlAsPdf("https://www.wikipedia.org/");
            //doc.SaveAs("wiki.pdf");
            //if (System.IO.File.Exists("Exam.html"))
            //{
            //    System.IO.File.Delete("Exam.html");
            //}
            //System.IO.File.WriteAllText("Exam.html", htmllll);
            //var htmll = this.RenderViewAsync(this Controller,"Assessment", AssessmentViewModel);
            //  var htmlContent = String.Format(htmllll, DateTime.Now);
            //var render = new IronPdf.ChromePdfRenderer();
            //using var doc = render.RenderHtmlAsPdf(htmllll);

            // var rrr= doc.SaveAs("HtmlString.pdf");
            //var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            //if (System.IO.File.Exists("export.pdf"))
            //{
            //    System.IO.File.Delete("export.pdf");
            //}
          //  htmlToPdf.GeneratePdfFromFile("HTMLFile.html", null, "export.pdf");
            MemoryStream stream = new MemoryStream(buyes);

            string mimeType = "application/pdf";
            return new FileStreamResult(stream, mimeType)
            {
                FileDownloadName = "Exam.pdf"
            };
            // System.IO.File.Open("export.pdf",FileMode.OpenOrCreate);
            //  return File(doc.Stream.ToArray(), "application/pdf");




        }

        [CustomPrivilege]
        public IActionResult TakersResult(long ID)
        {
            AdminExam adminExam = new AdminExam();
            List<ExamTaker> examTakers = adminExam.GetExamTakerlistByExamId(ID);
            ViewBag.TraningName = adminExam.GetTraningByExamId(ID).Name;
            return View(examTakers);
        }
        //public FileResult DownloadExam(long ID)
        //{
        //    AdminExam adminExam = new AdminExam();
        //    Exam exam = adminExam.GetExamById(ID);
        //    var data = adminExam.GetQuestionsByExamId(ID).ToList();
        //    IEnumerable<string> array = null;
        //    if (exam.LanguageID == 1) //English
        //    {
        //        array = data.Select(x => x.EnglishName).ToArray();
        //    }
        //    else
        //    {
        //        array = data.Select(x => x.ArabicName).ToArray();
        //    }
        //    var byteArray = array.ToPdf();

        //    MemoryStream stream = new MemoryStream(byteArray);

        //    string mimeType = "application/pdf";
        //    return new FileStreamResult(stream, mimeType)
        //    {
        //        FileDownloadName = "Exam.pdf"
        //    };
        //   }

        [HttpGet]
        [CustomPrivilege]
        public IActionResult Search()
        {
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            ViewBag.LevelID = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");

            return View();
        }


        [HttpPost]
        public IActionResult Search(ExamViewModel examViewModel)
        {
            AdminExam adminExam = new AdminExam();
            AdminQuestionAnswer adminQuestionAnswer = new AdminQuestionAnswer();
            DateTime DefaultdateTime = new DateTime();

            DateTime rngMin = (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue;

            DateTime rngMax = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
            if (examViewModel.ValidityDateFrom == DefaultdateTime || examViewModel.ValidityDateFrom == null)
            {
                examViewModel.ValidityDateFrom = rngMin;
            }
            if (examViewModel.ValidityDateTo == DefaultdateTime || examViewModel.ValidityDateTo == null)
            {
                examViewModel.ValidityDateTo = rngMax;
            }

            ViewBag.LevelID = new SelectList(adminQuestionAnswer.GetLevelList(), "ID", "Name");

            if (examViewModel.LevelID == -1 && string.IsNullOrEmpty(examViewModel.TrainingName) && examViewModel.ValidityDateFrom == rngMin && examViewModel.ValidityDateTo == rngMax)
            {
                ModelState.AddModelError("", "Please insert searching data.");
                return View(examViewModel);
            }
            // To make isValid should make viewModel to avoid null objects which not allow null at DB Model schema
            IList<training> examlist = adminExam.Search(examViewModel.TrainingName, examViewModel.ValidityDateFrom, examViewModel.ValidityDateTo, examViewModel.LevelID);
            List<ExamViewModel> exams = new List<ExamViewModel>();
            if (examlist.Count > 0)
            {
               
                foreach (var exam in examlist)
                {
                    ExamViewModel examViewModel1 = new ExamViewModel()
                    {
                        TrainingName = exam.Name,
                        ValidityDateFrom = exam.Exam.ValidityDateFrom,
                        ValidityDateTo = exam.Exam.ValidityDateTo,
                        level = exam.Exam.Level,
                        LevelID = exam.Exam.LevelID
                    };
                    exams.Add(examViewModel1);
                }
            }
            else
            {
                ViewBag.Message = "No Data Found";
                return View("Search", examViewModel);
            }
            return View("SearchResult", exams);

        }
        //public string RenderRazorViewToString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngine.Engines.FindPartialView(ControllerContext,
        //                                                                 viewName);
        //        var viewContext = new ViewContext(ControllerContext, viewResult.View,
        //                                     ViewData, TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}
        public ViewResult Assessment(long ExamId)
        {
            AdminAssessment adminAssessment = new AdminAssessment();
            AdminExam adminExam = new AdminExam();

            AssessmentViewModel AssessmentViewModel = new AssessmentViewModel();
            List<QuestionwithAns> questionwithAns = new List<QuestionwithAns>();

            
            AssessmentViewModel.ExamTakerId = ExamId;
            AssessmentViewModel.exam = adminExam.GetExamById(ExamId);


            foreach (ExamQuestion item in AssessmentViewModel.exam.ExamQuestions)
            {
                questionwithAns.Add(new QuestionwithAns()
                {
                    question = item.Question,
                    QquAnswers = item.Question.QuestionAnswers.First()
                });
            }

            AssessmentViewModel.QuestionwithAns = questionwithAns;
            return View("/Views/ExamTaker/Assessment.cshtml", AssessmentViewModel);
        }


        //private string GetHtml(ViewResult result)
        //{
        //    // Get html from the result.
        //    // https://weblogs.asp.net/ricardoperes/getting-html-for-a-viewresult-in-asp-net-core

        //    var routeData = HttpContext.GetRouteData();
        //    var viewName = result.ViewName ?? routeData.Values["action"] as string;
        //    var actionContext = new ActionContext(HttpContext, routeData, new ControllerActionDescriptor());
        //    var options = HttpContext.RequestServices.GetRequiredService<IOptions<MvcViewOptions>>();
        //    var htmlHelperOptions = options.Value.HtmlHelperOptions;

        //    var viewEngineResult = result.ViewEngine?.FindView(actionContext, viewName, true)
        //                           ?? options.Value.ViewEngines.Select(x => x.FindView(actionContext, viewName, true))
        //                               .First(x => x != null);

        //    var view = viewEngineResult.View;
        //    var builder = new StringBuilder();

        //   using var output = new StringWriter(builder);
        //    var viewContext = new ViewContext(actionContext, view, result.ViewData, result.TempData, output, htmlHelperOptions);
        //    view.RenderAsync(viewContext);

        //    return builder.ToString();
        //}

        //[HttpPost]
        //public ActionResult ExampleView(ExampleModel model)
        //{
           
        //    var html = this.RenderViewAsync("_Example", model);
        //    var ironPdfRender = new IronPdf.ChromePdfRenderer();
        //    using var pdfDoc = ironPdfRender.RenderHtmlAsPdf(html.Result);
        //    return File(pdfDoc.Stream.ToArray(), "application/pdf");
        //}
        //public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        //{
        //    if (string.IsNullOrEmpty(viewName))
        //    {
        //        viewName = controller.ControllerContext.ActionDescriptor.ActionName;
        //    }
        //    controller.ViewData.Model = model;
        //    using (var writer = new StringWriter())
        //    {
        //        IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        //        ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);
        //        if (viewResult.Success == false)
        //        {
        //            return $"A view with the name {viewName} could not be found";
        //        }
        //        ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer, new HtmlHelperOptions());
        //        await viewResult.View.RenderAsync(viewContext);
        //        return writer.GetStringBuilder().ToString();
        //    }
        //}

    }
}
