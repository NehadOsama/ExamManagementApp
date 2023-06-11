using CemexExamApp.ContextData;
using CemexExamApp.Models;
using CemexExamApp.ViewModel.VMReport;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.DBCore
{
    public class AdminReport
    {

        ExamManagmentAppContext dc = new ExamManagmentAppContext();

        #region Exam
        
        public List<GetNumberOfExamPerLevel_ViewModel> GetNumberOfExamPerLevel()
        {
            var results = dc.Exams.Include(q => q.Level).GroupBy(q => q.LevelID)
                                       .Select(q => new GetNumberOfExamPerLevel_ViewModel
                                       {
                                           Count = q.Count(),
                                           level = q.Select(x => x.Level).First()
                                       }).ToList();

            return results;
        }

        //public List<GetNumberOfExamPerLevel_Result> GetNumberOfExamPerLevel()
        //{
        //  var results = dc.Exams
        //               .GroupBy(e => new
        //               {
        //                   Year = e.CreateDate.Value.Year,
        //                   Month = e.CreateDate.Value.Month,
        //                   Day = e.CreateDate.Value.Day,
        //                   Hour = e.CreateDate.Value.Hour
        //               })
        //               .Select(e => new
        //               {
        //                   Count = e.Count(),
        //                   Detection = e.OrderByDescending(det => det.TimeStamp).First(),
        //               })
        //               .OrderByDescending(det => det.TimeStamp)
        //               .ToList();
        //}

        #endregion


        #region question

        public List<GetNumberOfQuestionPerTopic_ViewModel> GetNumberOfQuestionPerTopic()
        {
           var results = dc.Questions.Include(q =>q.Topic).GroupBy(q => q.TopicID)
                                      .Select(q => new GetNumberOfQuestionPerTopic_ViewModel
                                      {
                                                 Count = q.Count(),
                                                 Topic = q.Select(x=>x.Topic).First()
                                             }).ToList();

            return results;
        }
        public int GetNumberOfTopic()
        {
            int count = dc.Topics.Count();

            return count;
        }

        public List<GetNumberOfQuestionPerLevel_ViewModel> GetNumberOfQuestionPerLevel()
        {
            var results = dc.Questions.Include(q => q.Level).GroupBy(q => q.LevelID)
                                       .Select(q => new GetNumberOfQuestionPerLevel_ViewModel
                                       {
                                           Count = q.Count(),
                                           level = q.Select(x => x.Level).First()
                                       }).ToList();

            return results;
        }

        #endregion
    }
}
