using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ApplicationCore.DataAccess
{
	public class DefaultContext : IdentityDbContext<User>
	{
		public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
		{
		}
		public DbSet<UploadFile> UploadFiles { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<OAuth> OAuth { get; set; }
		public DbSet<Exam> Exams { get; set; }
		//public DbSet<ExamQuestion> ExamQuestions { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Option> Options { get; set; }
		public DbSet<Resolve> Resolves { get; set; }
		public DbSet<Subject> Subjects { get; set; }
		

		public DbSet<Term> Terms { get; set; }
		public DbSet<TermQuestion> TermQuestions { get; set; }
		public DbSet<Recruit> Recruits { get; set; }
		public DbSet<RecruitQuestion> RecruitQuestions { get; set; }

		public DbSet<ReviewRecord> ReviewRecords { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>(ConfigureUser);

			builder.Entity<RecruitQuestion>(ConfigureRecruitQuestion);

			builder.Entity<TermQuestion>(ConfigureTermQuestion);
		}

		private void ConfigureUser(EntityTypeBuilder<User> builder)
		{
			builder.HasOne(u => u.RefreshToken)
					.WithOne(rt => rt.User)
					.HasForeignKey<RefreshToken>(rt => rt.UserId);
		}
		private void ConfigureRecruitQuestion(EntityTypeBuilder<RecruitQuestion> builder)
		{
			builder.HasKey(item => new { item.RecruitId, item.QuestionId });

			builder.HasOne<Recruit>(item => item.Recruit)
				.WithMany(item => item.RecruitQuestions)
				.HasForeignKey(item => item.RecruitId);


			builder.HasOne<Question>(item => item.Question)
				.WithMany(item => item.RecruitQuestions)
				.HasForeignKey(item => item.QuestionId);

		}

		private void ConfigureTermQuestion(EntityTypeBuilder<TermQuestion> builder)
		{
			builder.HasKey(item => new { item.TermId, item.QuestionId });

		}

	}
}
