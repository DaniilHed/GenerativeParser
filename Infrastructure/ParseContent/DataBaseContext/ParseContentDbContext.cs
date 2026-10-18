using Contracts.DTOs;
using Infrastructure.ParseContent.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Infrastructure.ParseContent.DataBaseContext
{
	public class ParseContentDbContext:DbContext
	{
		public ParseContentDbContext (DbContextOptions<ParseContentDbContext> options): base(options) { }

		public DbSet<ParseContentJob> ParseJobs => Set<ParseContentJob>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ParseContentJob>(entity =>
			{
				entity.HasKey(e => e.Id);

				// Map the Result record to a PostgreSQL JSONB column 
				entity.Property(e => e.Result)
					  .HasColumnType("jsonb")
					  .HasConversion(
						  v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
						  v => JsonSerializer.Deserialize<ParseContentResult>(v, (JsonSerializerOptions?)null)
					  );
			});
		}
	}
}
