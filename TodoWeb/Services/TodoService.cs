using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using TodoWeb.Data;
using TodoWeb.Models;

namespace TodoWeb.Services
{
    /// <summary>
    /// ToDo 서비스 구현
    /// 비즈니스 로직과 데이터 액세스 담당
    /// </summary>
    public class TodoService : ITodoService
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoService> _logger;

        public TodoService(TodoContext context, ILogger<TodoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 모든 ToDo 조회 (최신순)
        /// </summary>
        public async Task<IEnumerable<TodoItem>> GetAllAsync()
        {
            _logger.LogInformation("모든 ToDo 조회");
            
            return await _context.TodoItems
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// ID로 ToDo 조회
        /// </summary>
        public async Task<TodoItem?> GetByIdAsync(int id)
        {
            _logger.LogInformation("ToDo 조회: {Id}", id);
            
            return await _context.TodoItems.FindAsync(id);
        }

        /// <summary>
        /// ToDo 생성
        /// </summary>
        public async Task<TodoItem> CreateAsync(TodoItem todoItem)
        {
            _logger.LogInformation("ToDo 생성: {Title}", todoItem.Title);
            
            // 자동 설정 값들
            todoItem.Id = 0; // EF Core가 자동 생성
            todoItem.CreatedAt = DateTime.Now;
            todoItem.CompletedAt = null;

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        /// <summary>
        /// ToDo 수정
        /// </summary>
        public async Task<TodoItem?> UpdateAsync(int id, TodoItem todoItem)
        {
            _logger.LogInformation("ToDo 수정: {Id}", id);
            
            var existingTodo = await _context.TodoItems.FindAsync(id);
            if (existingTodo == null)
                return null;

            // 수정 가능한 필드만 업데이트
            existingTodo.Title = todoItem.Title;
            existingTodo.Description = todoItem.Description;
            existingTodo.IsCompleted = todoItem.IsCompleted;

            await _context.SaveChangesAsync();
            return existingTodo;
        }

        /// <summary>
        /// ToDo 삭제
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation("ToDo 삭제: {Id}", id);
            
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null)
                return false;

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 완료된 ToDo 조회
        /// </summary>
        public async Task<IEnumerable<TodoItem>> GetCompletedAsync()
        {
            _logger.LogInformation("완료된 ToDo 조회");
            
            return await _context.TodoItems
                .Where(t => t.IsCompleted)
                .OrderByDescending(t => t.CompletedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 미완료 ToDo 조회
        /// </summary>
        public async Task<IEnumerable<TodoItem>> GetPendingAsync()
        {
            _logger.LogInformation("미완료 ToDo 조회");
            
            return await _context.TodoItems
                .Where(t => !t.IsCompleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}