using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CemexExamApp.ContextData;
using CemexExamApp.Repository;
using System.Security.Cryptography;

namespace CemexExamApp.Models.Repositories
{
    public class UserDbRepository : ICemexManagExam<SecUser>
    {
        ExamManagmentAppContext db;

        public UserDbRepository(ExamManagmentAppContext _db)
        {
            db = _db;
        }

        public UserDbRepository()
        {
            db = new ExamManagmentAppContext();
        }

        public void Add(SecUser entity)
        {
            db.SecUsers.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var secUser = db.SecUsers.SingleOrDefault(a => a.ID == id);
            secUser.Active = false;
            db.SaveChanges();
        }
        public void UnDelete(int id)
        {
            var secUser = db.SecUsers.SingleOrDefault(a => a.ID == id);
            secUser.Active = true;
            db.SaveChanges();
        }
     

        public SecUser Find(string Username)
        {
            var secUser = db.SecUsers.Include(x => x.Role).SingleOrDefault(x => x.Username.ToLower() == Username.Trim().ToLower() && x.Active && x.AD);
            return secUser;
        }

        public IList<SecUser> GetActiveList()
        {
            return db.SecUsers.OrderBy(x => x.Username).Where(x => x.Active).ToList();
        }

        public IList<SecUser> List()
        {
            return db.SecUsers.Include(x => x.Role).ToList();
        }

        public IList<SecUser> Search(string term)
        {
            return db.SecUsers.Where(b => b.Username.Contains(term)
            || b.Email.Contains(term)
            ).ToList();
        }

        public void Update(int id, SecUser newentity)
        {
            
            db.SecUsers.Update(newentity);
            db.SaveChanges();
        }

  

        public SecUser Find(long id)
        {
            throw new NotImplementedException();
        }

        public IList<SecUser> Search(string QustionName, string TopicName, int levelId)
        {
            throw new NotImplementedException();
        }

        public bool IsNamesExist(string EngName, string AraName)
        {
            throw new NotImplementedException();
        }

        public bool IsNamesExistBefore(string EngName, string AraName, long RecordId)
        {
            throw new NotImplementedException();
        }

        public SecUser Find(int id)
        {
            var secUser = db.SecUsers.SingleOrDefault(x => x.ID== id && x.Active);
            return secUser;
        }
    }
}
