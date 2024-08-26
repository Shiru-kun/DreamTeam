using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamAPI.Models;

public partial class DreamTeamContext : DbContext
{
    public DreamTeamContext()
    {
    }

    public DreamTeamContext(DbContextOptions<DreamTeamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Team> Teams { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Team");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Fullname)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.JobTitle)
                .IsUnicode(false)
                .HasColumnName("job_title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
