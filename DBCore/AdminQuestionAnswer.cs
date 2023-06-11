using CemexExamApp.ContextData;
using CemexExamApp.Models;
using MessagePack;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.DBCore
{
    public class AdminQuestionAnswer
    {

        ExamManagmentAppContext dc = new ExamManagmentAppContext();


        public void SubmitNewQuestion(Question question,QuestionAnswer questionAnswer)
        {
            dc.Questions.Add(question);
            dc.QuestionAnswers.Add(questionAnswer);
            dc.SaveChanges();
        }

        public void SaveUpdates()
        {
           
            dc.SaveChanges();
        }

        public QuestionAnswer GetQueAnsByQueId(long QueId)
        {
            return dc.QuestionAnswers.Include(x=>x.Question).FirstOrDefault(x => x.QuestionID == QueId);
        }
        public IEnumerable<Level> GetLevelList()
        {
            return dc.Levels;
        }
        public IEnumerable<AnswerCount> GetAnswerCountList()
        {
            return dc.AnswerCounts.Where(x=>x.Count >= 3);
        }
        public IEnumerable<AnswerCount> GetRightAnswerList()
        {
            return dc.AnswerCounts;
        }
        public IEnumerable<AnswerCount> GetRightAnswerListByAnswerCount(int AnswerCountValue)
        {
            return dc.AnswerCounts.Where(x=>x.Count <= AnswerCountValue);
        }



        public QuestionAnswer GetQuestionAnswer(long Questionid)
        {
          return  dc.QuestionAnswers.Include(x=>x.Question).FirstOrDefault(x => x.QuestionID == Questionid);
        }


        public int GetNoOfActiveQuestion()
        {
            return dc.Questions.Count(x => x.Active);
        }


        public bool IsNoOfQPTEqualExistQ(List<Int32> TpicsIds , int NoOfQPerT , int levelId)
        {
            
            foreach (Int32 qid in TpicsIds)
            {

                int ExistQCount = dc.Questions.Where(x => (x.TopicID == qid && x.Active && x.LevelID == levelId)).Count();
                if(NoOfQPerT > ExistQCount)
                {
                    return true;
                }       
            }
            return false;
        }
    }
}
