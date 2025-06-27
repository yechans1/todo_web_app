using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Models;

namespace TodoWeb.Data
{
    /// <summary>
    /// ToDo 데이터베이스 컨텍스트
    /// </summary>
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        /// <summary>
        /// ToDo 항목 테이블
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }

        /// <summary>
        /// 모델 구성
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TodoItem 설정
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("datetime('now')");
            });
        }

        /// <summary>
        /// 저장 시 자동으로 완료일시 설정
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<TodoItem>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    // 완료 상태 변경 시 완료일시 자동 설정
                    if (entry.Property(e => e.IsCompleted).IsModified)
                    {
                        entry.Entity.CompletedAt = entry.Entity.IsCompleted ? DateTime.Now : null;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}