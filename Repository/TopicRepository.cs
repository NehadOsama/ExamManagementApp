using CemexExamApp.ContextData;
using static CemexExamApp.ContextData.ExamManagmentAppContext;
using CemexExamApp.Models;

namespace CemexExamApp.Repository
{
    public class TopicRepository : ICemexManagExam<Topic>
    {
        private readonly ExamManagmentAppContext db;

        public TopicRepository(ExamManagmentAppContext db)
        {
            this.db = db;
        }
        public void Add(Topic entity)
        {
            db.Topics.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var topic = db.Topics.SingleOrDefault(a => a.ID == id);
           // topic.Deleted = true;
            db.SaveChanges();
        }

        public Topic Find(int id)
        {
            var topic = db.Topics.SingleOrDefault(x => x.ID == id);
            return topic;
        }

        public Topic Find(long id)
        {
            throw new NotImplementedException();
        }


        public IList<Topic> List()
        {
           return db.Topics.ToList();
        }

        public IList<Topic> Search(string term)
        {
            throw new NotImplementedException();
        }

        public void UnDelete(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, Topic entity)
        {
            
            db.Topics.Update(entity);
            db.SaveChanges();
        }

        public Topic Find(string Username)
        {
            throw new NotImplementedException();
        }

        public IList<Topic> GetActiveList()
        {
            throw new NotImplementedException();
        }

        public bool IsNamesExist(string EngName , string AraName )
        {
            var data = db.Topics.FirstOrDefault(x => x.EnglishName.ToLower() == EngName.ToLower() || x.ArabicName.ToLower() == AraName.ToLower());
            if (data == null)
                return false;
            else
                return true;
        }

        public bool IsNamesExistBefore(string EngName, string AraName , int RecordId)
        {
            var data = db.Topics.FirstOrDefault(x =>( x.EnglishName.ToLower() == EngName.ToLower() || x.ArabicName.ToLower() == AraName.ToLower())
                                                        && x.ID != RecordId);
            if(data == null)
            return false;
            else
                return true;
        }

        public IList<Topic> Search(string QustionName, string TopicName, int levelId)
        {
            throw new NotImplementedException();
        }

     
        public bool IsNamesExistBefore(string EngName, string AraName, long RecordId)
        {
            var data = db.Topics.FirstOrDefault(x => (x.EnglishName.ToLower() == EngName.ToLower() || x.ArabicName.ToLower() == AraName.ToLower())
                                                        && x.ID != RecordId);
            if (data == null)
                return false;
            else
                return true;
        }
    }
}
