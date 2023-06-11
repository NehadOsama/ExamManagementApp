using CemexExamApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel.VMReport
{
    public class GetNumberOfQuestionPerTopic_ViewModel
    {
        [Key]
        public int Count { get; set; }
        public Topic Topic { get; set; } = null!;

    }
}
