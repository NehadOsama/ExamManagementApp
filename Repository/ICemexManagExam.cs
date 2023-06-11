namespace CemexExamApp.Repository
{
    public interface ICemexManagExam<TEntity>
    {

        void Add(TEntity entity);
        void Update(int id, TEntity entity);
        void Delete(int id);
        void UnDelete(int id);
        IList<TEntity> List();
        IList<TEntity> GetActiveList();

        TEntity Find(int id);
        TEntity Find(long id);
        TEntity Find(string Username);
        IList<TEntity> Search(string QustionName, string TopicName, int levelId);
        bool IsNamesExist(string EngName, string AraName);
        bool IsNamesExistBefore(string EngName, string AraName, long RecordId);

    }
}
