using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TodoWeb.Models;

namespace TodoWeb.Services
{
    /// <summary>
    /// ToDo 서비스 인터페이스
    /// CRUD 작업을 추상화
    /// </summary>
    public interface ITodoService
    {
        /// <summary>
        /// 모든 ToDo 조회
        /// </summary>
        Task<IEnumerable<TodoItem>> GetAllAsync();

        /// <summary>
        /// ID로 ToDo 조회
        /// </summary>
        Task<TodoItem?> GetByIdAsync(int id);

        /// <summary>
        /// ToDo 생성
        /// </summary>
        Task<TodoItem> CreateAsync(TodoItem todoItem);

        /// <summary>
        /// ToDo 수정
        /// </summary>
        Task<TodoItem?> UpdateAsync(int id, TodoItem todoItem);

        /// <summary>
        /// ToDo 삭제
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// 완료된 ToDo 조회
        /// </summary>
        Task<IEnumerable<TodoItem>> GetCompletedAsync();

        /// <summary>
        /// 미완료 ToDo 조회
        /// </summary>
        Task<IEnumerable<TodoItem>> GetPendingAsync();
    }
}