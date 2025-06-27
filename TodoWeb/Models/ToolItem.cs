using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace TodoWeb.Models
{
    /// <summary>
    /// ToDo 항목 엔티티
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// 기본키
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 할일 제목 (필수)
        /// </summary>
        [Required(ErrorMessage = "제목은 필수입니다.")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 할일 설명 (선택)
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 완료 여부
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// 생성일시
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 완료일시
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }
}