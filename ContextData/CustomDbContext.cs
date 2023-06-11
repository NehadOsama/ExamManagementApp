using CemexExamApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CemexExamApp.ViewModel;
using TheInventory.Models;
using CemexExamApp.ViewModel.VMReport;
using CemexExamApp.ViewModel.VMAccount;

namespace CemexExamApp.ContextData
{
    public partial class ExamManagmentAppContext : DbContext
    {

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Topic>(entity =>
            //{
            //    entity.Property(e => e.ID).ValueGeneratedNever();
            //});
        }
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                IConfigurationRoot configuration = new ConfigurationBuilder()
                          .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                          .AddJsonFile("appsettings.json")
                          .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("ExamDB"));

             }
        }
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
        public DbSet<CemexExamApp.ViewModel.VMReport.GetNumberOfExamPerLevel_ViewModel> GetNumberOfExamPerLevel_ViewModel { get; set; }
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
        public DbSet<CemexExamApp.ViewModel.VMReport.GetNumberOfQuestionPerLevel_ViewModel> GetNumberOfQuestionPerLevel_ViewModel { get; set; }
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
        public DbSet<CemexExamApp.ViewModel.VMReport.GetNumberOfQuestionPerTopic_ViewModel> GetNumberOfQuestionPerTopic_ViewModel { get; set; }
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
        public DbSet<CemexExamApp.ViewModel.VMAccount.ChangePasswordViewModel> ChangePasswordViewModel { get; set; }
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
       
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
      
        //[Table("Topic")]
        //public partial class Topic
        //{
           

        //    [Key]
        //    public int ID { get; set; }

        //    [Required]
        //    [RegularExpression(@"^[a-zA-Z0-9'' ']+_-.", ErrorMessage = "English character , 0:9 , special characters -_. only allowed.")]
        //    public string EnglishName { get; set; } = null!;

        //    [Required]
        //    [RegularExpression(@"^[\u0600-\u06FF*$/'' ']+_-.", ErrorMessage = "Arabic character , 0:9 , special characters -_. only allowed.")]
        //    public string ArabicName { get; set; } = null!;

          
        //}
        //public DbSet<CemexExamApp.ViewModel.ExamViewModel> ExamViewModel { get; set; }


    }
}
