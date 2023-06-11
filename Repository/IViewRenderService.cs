namespace CemexExamApp.Repository
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string ViewName, object model); 
    }
}
