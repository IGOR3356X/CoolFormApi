﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CoolFormApi.Models;

public partial class CoolFormsDbContext : DbContext
{
    public CoolFormsDbContext()
    {
    }

    public CoolFormsDbContext(DbContextOptions<CoolFormsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Correctanswer> Correctanswers { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<FormGroup> FormGroups { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Response> Responses { get; set; }

    public virtual DbSet<ResponseOption> ResponseOptions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum("question_types", new[] { "Text", "CheckBox", "RadioButton" });

        modelBuilder.Entity<Correctanswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("correctanswers_pkey");

            entity.ToTable("correctanswers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Optionid).HasColumnName("optionid");
            entity.Property(e => e.Questionid).HasColumnName("questionid");

            entity.HasOne(d => d.Option).WithMany(p => p.Correctanswers)
                .HasForeignKey(d => d.Optionid)
                .HasConstraintName("correctanswers_optionid_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.Correctanswers)
                .HasForeignKey(d => d.Questionid)
                .HasConstraintName("correctanswers_questionid_fkey");
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("form_pkey");

            entity.ToTable("form");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(2048)
                .HasColumnName("description");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(true)
                .HasColumnName("is_public");
            entity.Property(e => e.MaxScore)
                .HasDefaultValue(0)
                .HasColumnName("max_score");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Forms)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("form_user_id_fkey");
        });

        modelBuilder.Entity<FormGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("form_groups_pkey");

            entity.ToTable("form_groups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");

            entity.HasOne(d => d.Form).WithMany(p => p.FormGroups)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("form_groups_form_id_fkey");

            entity.HasOne(d => d.Group).WithMany(p => p.FormGroups)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("form_groups_group_id_fkey");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("group_pkey");

            entity.ToTable("group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("options_pkey");

            entity.ToTable("options");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OptionText).HasColumnName("option_text");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Options)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("options_question_id_fkey");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questions_pkey");

            entity.ToTable("questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectAnswer).HasColumnName("correct_answer");
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.Points)
                .HasDefaultValue(0)
                .HasColumnName("points");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(25)
                .HasColumnName("question_type");

            entity.HasOne(d => d.Form).WithMany(p => p.Questions)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("questions_form_id_fkey");
        });

        modelBuilder.Entity<Response>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("responses_pkey");

            entity.ToTable("responses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.ScoreId).HasColumnName("score_id");
            entity.Property(e => e.UserAnswer).HasColumnName("user_answer");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Form).WithMany(p => p.Responses)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("responses_form_id_fkey");

            entity.HasOne(d => d.Question).WithMany(p => p.Responses)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("responses_question_id_fkey");

            entity.HasOne(d => d.Score).WithMany(p => p.Responses)
                .HasForeignKey(d => d.ScoreId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("responses_score_id_fkey");
        });

        modelBuilder.Entity<ResponseOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("response_options_pkey");

            entity.ToTable("response_options");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OptionId).HasColumnName("option_id");
            entity.Property(e => e.ResponseId).HasColumnName("response_id");

            entity.HasOne(d => d.Option).WithMany(p => p.ResponseOptions)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("response_options_option_id_fkey");

            entity.HasOne(d => d.Response).WithMany(p => p.ResponseOptions)
                .HasForeignKey(d => d.ResponseId)
                .HasConstraintName("response_options_response_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("scores_pkey");

            entity.ToTable("scores");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.Score1).HasColumnName("score");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Form).WithMany(p => p.Scores)
                .HasForeignKey(d => d.FormId)
                .HasConstraintName("scores_form_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Scores)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("scores_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(1)
                .HasColumnName("role_id");

            entity.HasOne(d => d.Group).WithMany(p => p.Users)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("user_group_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
