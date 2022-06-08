using TodoWebapp.Models;

namespace TodoWebapp.Services;

public interface ITodoService
{
    Task<IEnumerable<Todo>?> GetTodosAsync();
    Task<Todo?> GetTodoAsync(int id);
    Task<Todo?> AddTodoAsync(Todo todo);
    Task<Todo?> UpdateTodoAsync(int id, Todo todo);
    Task<Todo?> DeleteTodoAsync(int id);
}
