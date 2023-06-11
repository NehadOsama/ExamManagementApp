using CemexExamApp.ContextData;
using CemexExamApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CemexExamApp.DBCore
{
    public class AdminExam
    {

        ExamManagmentAppContext dc = new ExamManagmentAppContext();

       
        public IEnumerable<Exam> GetExamList()
        {
            return dc.Exams.Include(x => x.training).Include(x=>x.Level);
        } // For Index

        #region DDL
        public IEnumerable<Level> GetLevelList()
        {
            return dc.Levels;
        }
        public IEnumerable<Benchmark> GetBenchmarkList()
        {
            return dc.Benchmarks;
        }
        public IEnumerable<Duration> GetDurationList()
        {
            return dc.Durations;
        }
        public IEnumerable<Language> GetLanguageList()
        {
            return dc.Languages;
        }

        #endregion


        #region Validation
        public bool IsNamesExist(string trainingName, DateTime trainingFromDate, DateTime trainingToDate)
        {
            return dc.training.Any(x => (x.Name.ToLower().Contains(trainingName.ToLower())
            || x.FromDate == trainingFromDate || x.ToDate == trainingToDate));
        }

        public bool IsNamesExistBefore(string trainingName, DateTime trainingFromDate, DateTime trainingToDate, long RecordId)
        {
            return dc.training.Any(x => (x.Name.ToLower()== trainingName.ToLower()
                 || x.FromDate == trainingFromDate || x.ToDate == trainingToDate) && x.ExamID != RecordId);
        }


        #endregion

        public List<Question> GetRandomQuestionsPerTopic(int NoOfQustionPerTopic,int levelId, List<Int32> ToicsIds)
        {
            List<Question> randomQuestions = new List<Question>();

            foreach(Int32 qid in ToicsIds)
            {
                for(int i=0; i < NoOfQustionPerTopic; i++)
                {
                    Random random = new Random();
                    var q = dc.Questions.Where(x => (x.TopicID == qid && x.Active && x.LevelID == levelId)).AsEnumerable();
                    q = q.OrderBy(x=> random.Next()).Take(1);
                    if (randomQuestions.Contains(q.First()))
                    {
                        i = i - 1;
                    }
                    else
                    {
                        randomQuestions.AddRange(q);
                    }
                }
            }
            return randomQuestions;
        } 

        public void SubmitNewExam(Exam exam,training training,List<ExamTopic> examTopics
            ,List<ExamTaker> examTakers, List<ExamQuestion> examQuestions )
        {
            dc.Exams.Add(exam);
            dc.training.Add(training);
            dc.ExamTopics.AddRange(examTopics);
            dc.ExamQuestions.AddRange(examQuestions);
            dc.ExamTakers.AddRange(examTakers);
            dc.SaveChanges();
        }


        public void EditUpdates(List<ExamTopic> existTopics, List<ExamTopic> NewexamTopics
            , List<ExamTaker> existTakers, List<ExamTaker> NewexamTakers
            , List<ExamQuestion> existQuestions, List<ExamQuestion> NewexamQuestions)
        {
            //update exam and training
         
            dc.ExamTopics.RemoveRange(existTopics);
            dc.ExamTopics.AddRange(NewexamTopics);
     
            dc.ExamQuestions.RemoveRange(existQuestions);
            dc.ExamQuestions.AddRange(NewexamQuestions);
            
            dc.ExamTakers.RemoveRange(existTakers);
            dc.ExamTakers.AddRange(NewexamTakers);
            dc.SaveChanges();
        }

        public void EditUpdatesWithoutTakers(List<ExamTopic> existTopics, List<ExamTopic> NewexamTopics
           , List<ExamQuestion> existQuestions, List<ExamQuestion> NewexamQuestions)
        {
            //update exam and training
    
            dc.ExamTopics.RemoveRange(existTopics);
            dc.ExamTopics.AddRange(NewexamTopics);
         
            dc.ExamQuestions.RemoveRange(existQuestions);
            dc.ExamQuestions.AddRange(NewexamQuestions);
            
            dc.SaveChanges();
        }
        public Exam GetExamById(long ID)
        {
          return  dc.Exams.Include(x=>x.training).Include(x=>x.ExamTopics).Include(x=>x.ExamQuestions).ThenInclude(x=>x.Question).ThenInclude(x=>x.QuestionAnswers).Include(x=>x.ExamTakers).Include(x=>x.Benchmark).Include(x=>x.Duration).FirstOrDefault(x => x.ID == ID);
        }

        public training GetTraningByExamId(long ID)
        {
            return dc.training.FirstOrDefault(x => x.ExamID == ID);
        }

        public List<ExamTopic> GetTopicsByExamId(long ID)
        {
            return dc.ExamTopics.Where(x => x.ExamID == ID).ToList();
        }
        public List<ExamTaker> GetTakersByExamId(long ID)
        {
            return dc.ExamTakers.Where(x => x.ExamID == ID).ToList();
        }


        public List<ExamQuestion> GetExamQuestionsByExamId(long ID)
        {

            return dc.ExamQuestions.Where(x => x.ExamID == ID).ToList();

        }
        public List<Question> GetQuestionsByExamId(long ID)
        {
            
                return dc.ExamQuestions.Where(x => x.ExamID == ID).Select(x => x.Question).ToList();
            
        }
        public IList<training> Search( string TraName,DateTime ValFromDate,DateTime ValToDate, int levelId)
        {
            return dc.training.Include(x => x.Exam).ThenInclude(x => x.Level)
                              .Where(x => ((x.Exam.ValidityDateFrom >= ValFromDate && x.Exam.ValidityDateTo <= ValToDate)  &&
                                           (TraName == null || x.Name.ToLower().Contains(TraName.ToLower()) )  &&
                                           (levelId == -1 || x.Exam.LevelID == levelId)
                                          )).ToList();
        }


        public bool CanNotEditTopic(int TopicId)
        {
            return dc.ExamTopics.Any(x => x.TopicID == TopicId && (x.Exam.ValidityDateFrom <= DateTime.Now && x.Exam.ValidityDateTo >= DateTime.Now));
        }

        public List<ExamTaker> GetExamTakerlistByExamId(long ExamId)
        {
           return dc.ExamTakers.Where(x=>x.ExamID == ExamId).ToList();
        }

    }
}
