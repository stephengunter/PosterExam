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

		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<OAuth> OAuth { get; set; }
		public DbSet<Exam> Exams { get; set; }
		public DbSet<ExamQuestion> ExamQuestions { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Subject> Subjects { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Term> Terms { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>(ConfigureUser);

		}

		private void ConfigureUser(EntityTypeBuilder<User> builder)
		{
			builder.HasOne(u => u.RefreshToken)
					.WithOne(rt => rt.User)
					.HasForeignKey<RefreshToken>(rt => rt.UserId);
		}


	}
}
