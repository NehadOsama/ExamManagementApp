using CemexExamApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CemexExamApp.ViewModel.VMReport
{
    public class GetNumberOfExamPerLevel_ViewModel
    {
        [Key]
        public int Count { get; set; }
        public Level level { get; set; } = null!;

    }
}
