using CemexExamApp.ContextData;
using CemexExamApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CemexExamApp.DBCore
{
    public class AdminAssessment
    {

        ExamManagmentAppContext dc = new ExamManagmentAppContext();


        public bool CheckTakerEmail(string Email,long emamId)
        {
          return dc.ExamTakers.Any(x => (x.EMail.ToLower() == Email  && x.ExamID == emamId ));
        }
        public bool IsOpendBefore(string Email, long emamId)
        {
            var res = dc.ExamTakers.Where(x => (x.EMail.ToLower() == Email && x.ExamID == emamId && x.OpenedBefore)).ToList();
            if (res.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool CheckTakerExamValDate( long emamId)
        {
            return dc.ExamTakers.Any(x => x.ExamID == emamId && x.Exam.ValidityDateFrom <= DateTime.Now.Date && x.Exam.ValidityDateTo >= DateTime.Now.Date);
        }

        public List<Exam> GetTodayExamTakerslist()
        {
           return dc.Exams.Include(x=>x.ExamTakers).Where(x=> x.ValidityDateFrom <= DateTime.Now.Date && x.ValidityDateTo >= DateTime.Now.Date).ToList();
        }

        public ExamTaker GetExamTakeByID(long ExamTakerID)
        {
            return dc.ExamTakers.Include(x => x.Exam).ThenInclude(x => x.ExamQuestions).ThenInclude(x => x.Question).ThenInclude(x => x.QuestionAnswers).FirstOrDefault(x => x.ID == ExamTakerID);

        }

        public void SubmitAssessment(List<ExamTakerAnswer> examTakerAnswers)
        {

            dc.ExamTakerAnswers.AddRange(examTakerAnswers);
            dc.SaveChanges();
        }


        public void SaveChange()
        {
            dc.SaveChanges();
        }
    }
}
