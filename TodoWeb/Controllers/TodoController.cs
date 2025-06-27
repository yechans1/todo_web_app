using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoWeb.Models;
using TodoWeb.Services;

namespace TodoWeb.Controllers
{
    /// <summary>
    /// ToDo CRUD API 컨트롤러 (인증 필요)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 모든 엔드포인트에 JWT 인증 필요
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        /// <summary>
        /// 모든 ToDo 조회
        /// GET api/todo
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
        {
            try
            {
                var todos = await _todoService.GetAllAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "모든 ToDo 조회 중 오류");
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// ID로 ToDo 조회
        /// GET api/todo/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetById(int id)
        {
            try
            {
                var todo = await _todoService.GetByIdAsync(id);
                if (todo == null)
                    return NotFound($"ID {id}인 ToDo를 찾을 수 없습니다.");

                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToDo 조회 중 오류: {Id}", id);
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// 새 ToDo 생성
        /// POST api/todo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TodoItem>> Create([FromBody] TodoItem todoItem)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdTodo = await _todoService.CreateAsync(todoItem);
                return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToDo 생성 중 오류: {Title}", todoItem.Title);
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// ToDo 수정
        /// PUT api/todo/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TodoItem>> Update(int id, [FromBody] TodoItem todoItem)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updatedTodo = await _todoService.UpdateAsync(id, todoItem);
                if (updatedTodo == null)
                    return NotFound($"ID {id}인 ToDo를 찾을 수 없습니다.");

                return Ok(updatedTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToDo 수정 중 오류: {Id}", id);
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// ToDo 삭제
        /// DELETE api/todo/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _todoService.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"ID {id}인 ToDo를 찾을 수 없습니다.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToDo 삭제 중 오류: {Id}", id);
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// 완료된 ToDo 조회
        /// GET api/todo/completed
        /// </summary>
        [HttpGet("completed")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetCompleted()
        {
            try
            {
                var todos = await _todoService.GetCompletedAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "완료된 ToDo 조회 중 오류");
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// 미완료 ToDo 조회
        /// GET api/todo/pending
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetPending()
        {
            try
            {
                var todos = await _todoService.GetPendingAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "미완료 ToDo 조회 중 오류");
                return StatusCode(500, "서버 오류가 발생했습니다.");
            }
        }
    }
}