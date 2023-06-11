using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CemexExamApp.Models;

namespace CemexExamApp.ContextData
{
    public partial class ExamManagmentAppContext : DbContext
    {
        public ExamManagmentAppContext()
        {
        }

        public ExamManagmentAppContext(DbContextOptions<ExamManagmentAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AnswerCount> AnswerCounts { get; set; } = null!;
        public virtual DbSet<Benchmark> Benchmarks { get; set; } = null!;
        public virtual DbSet<ControllerAction> ControllerActions { get; set; } = null!;
        public virtual DbSet<Duration> Durations { get; set; } = null!;
        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<ExamQuestion> ExamQuestions { get; set; } = null!;
        public virtual DbSet<ExamTaker> ExamTakers { get; set; } = null!;
        public virtual DbSet<ExamTakerAnswer> ExamTakerAnswers { get; set; } = null!;
        public virtual DbSet<ExamTopic> ExamTopics { get; set; } = null!;
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; } = null!;
        public virtual DbSet<Language> Languages { get; set; } = null!;
        public virtual DbSet<Level> Levels { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RoleRouting> RoleRoutings { get; set; } = null!;
        public virtual DbSet<SecUser> SecUsers { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<training> training { get; set; } = null!;

     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnswerCount>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Benchmark>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<ControllerAction>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Duration>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasOne(d => d.Benchmark)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.BenchmarkID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Benchmark");

                entity.HasOne(d => d.Duration)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.DurationID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Duration");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.LanguageID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Language");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Exams)
                    .HasForeignKey(d => d.LevelID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_Level");
            });

            modelBuilder.Entity<ExamQuestion>(entity =>
            {
                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamQuestions)
                    .HasForeignKey(d => d.ExamID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamQuestion_Exam");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ExamQuestions)
                    .HasForeignKey(d => d.QuestionID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamQuestion_Question");
            });

            modelBuilder.Entity<ExamTaker>(entity =>
            {
                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamTakers)
                    .HasForeignKey(d => d.ExamID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamTaker_Exam");
            });

            modelBuilder.Entity<ExamTakerAnswer>(entity =>
            {
                entity.HasOne(d => d.ExamTaker)
                    .WithMany(p => p.ExamTakerAnswers)
                    .HasForeignKey(d => d.ExamTakerID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamTakerAnswer_ExamTaker");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ExamTakerAnswers)
                    .HasForeignKey(d => d.QuestionID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamTakerAnswer_Question");
            });

            modelBuilder.Entity<ExamTopic>(entity =>
            {
                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.ExamTopics)
                    .HasForeignKey(d => d.ExamID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamTopic_Exam");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.ExamTopics)
                    .HasForeignKey(d => d.TopicID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ExamTopic_Topic");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasOne(d => d.AnswerCount)
                    .WithMany(p => p.QuestionAnswerCounts)
                    .HasForeignKey(d => d.AnswerCountID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_AnswerCount");

                entity.HasOne(d => d.CorrectAnswer)
                    .WithMany(p => p.QuestionCorrectAnswers)
                    .HasForeignKey(d => d.CorrectAnswerID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_AnswerCount1");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.LevelID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Level");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TopicID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Question_Topic");
            });

            modelBuilder.Entity<QuestionAnswer>(entity =>
            {
                entity.HasOne(d => d.Question)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.QuestionID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuestionAnswer_Question");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.ID).ValueGeneratedNever();
            });

            modelBuilder.Entity<RoleRouting>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.ControllerActionId });

                entity.HasOne(d => d.ControllerAction)
                    .WithMany(p => p.RoleRoutings)
                    .HasForeignKey(d => d.ControllerActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleRouting_ControllerActions");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleRoutings)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleRouting_Role");
            });

            modelBuilder.Entity<SecUser>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SecUsers)
                    .HasForeignKey(d => d.RoleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SecUser_Role");
            });

            modelBuilder.Entity<training>(entity =>
            {
                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.training)
                    .HasForeignKey(d => d.ExamID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Training_Exam");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
