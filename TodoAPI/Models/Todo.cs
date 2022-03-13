using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models;

public class Todo
{
    public int Id { get; set; }
    [Required]
    public string? Title { get; set; }
    public bool IsComplete { get; set; }
}
