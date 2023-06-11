using CemexExamApp.ContextData;
using CemexExamApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CemexExamApp.Repository
{
    public class QuestionRepository : ICemexManagExam<Question>
    {
        private readonly ExamManagmentAppContext db;
        public QuestionRepository(ExamManagmentAppContext db)
        {
            this.db = db;
        }

        public void Add(Question entity)
        {
            db.Questions.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var question = db.Questions.SingleOrDefault(a => a.ID == id);
            question.Active = false;
            db.SaveChanges();
        }

        public Question Find(long id)
        {
            var question = db.Questions.SingleOrDefault(x => x.ID == id);
            return question;
        }

        public Question Find(string Username)
        {
            throw new NotImplementedException();
        }

        public Question Find(int id)
        {
            throw new NotImplementedException();
        }

        public IList<Question> GetActiveList()
        {
            return db.Questions.Where(x=>x.Active).ToList();
        }

        public bool IsNamesExist(string EngName, string AraName)
        {
           return db.Questions.Any(x => (x.EnglishName.ToLower().Contains(EngName.ToLower()) || x.ArabicName.ToLower().Contains(AraName.ToLower())));
        }

        public bool IsNamesExistBefore(string EngName, string AraName, int RecordId)
        {
            return db.Questions.Any(x => (x.EnglishName.ToLower().Contains(EngName.ToLower()) || x.ArabicName.ToLower().Contains(AraName.ToLower())) && x.ID != RecordId);
        }

        public bool IsNamesExistBefore(string EngName, string AraName, long RecordId)
        {
            return db.Questions.Any(x => (x.EnglishName.ToLower().Contains(EngName.ToLower()) || x.ArabicName.ToLower().Contains(AraName.ToLower())) && x.ID != RecordId);

        }

        public IList<Question> List()
        {
            return db.Questions.Include(q => q.AnswerCount).Include(q => q.CorrectAnswer).Include(q => q.Level).Include(q => q.Topic).ToList();
        }

        public IList<Question> Search(string QustionName , string TopicName , int levelId)
        {
            return db.Questions.Include(x=>x.Level).Include(x=>x.Topic).Where(x => (QustionName == null || x.EnglishName.ToLower().Contains(QustionName.ToLower()) ||
                                           x.ArabicName.ToLower().Contains(QustionName.ToLower()) )&&
                                           (TopicName == null || x.Topic.EnglishName.ToLower().Contains(TopicName.ToLower()) ||
                                           x.Topic.ArabicName.ToLower().Contains(TopicName.ToLower()) )&&
                                           (levelId == -1 || x.LevelID == levelId)).ToList();
        }

        public void UnDelete(int id)
        {
            var question = db.Questions.SingleOrDefault(a => a.ID == id);
            question.Active = true;
            db.SaveChanges();
        }

        public void Update(int id, Question entity)
        {
            //var oldquestion = Find(id);
            //oldquestion = entity;
            //oldquestion.EnglishName = entity.EnglishName;
            //oldquestion.ArabicName = entity.ArabicName;
            //db.Questions.Update(oldquestion);
            //db.SaveChanges();
        }
    }
}
