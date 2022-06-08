using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TodoAPI.Models;
using Xunit;

namespace TodoAPI.Tests;

public class TodoAPITests
{
    private readonly TodoApplication _app;
    public TodoAPITests() {
        _app = new TodoApplication();
    }

    [Fact]
    public async Task GetTodos()
    {
        var client = _app.CreateClient();
        var todos = await client.GetFromJsonAsync<List<Todo>>("/todos");

        Assert.Empty(todos);
    }

    [Fact]
    public async Task AddTodo()
    {
        Todo newTodo = new Todo { Title = "First Todo" };

        var client = _app.CreateClient();
        var response = await client.PostAsJsonAsync("/todos", newTodo);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
 
        var todos = await client.GetFromJsonAsync<List<Todo>>("/todos");
    
        Assert.Single(todos);
        Assert.Equal("First Todo", todos?[0].Title);
        Assert.False(todos?[0].IsComplete);
    }

    [Fact]
    public async Task GetOneTodo()
    {
        Todo newTodo = new Todo { Title = "Second Todo" };

        var client = _app.CreateClient();
        var response = await client.PostAsJsonAsync("/todos", newTodo);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var addedTodo = await response.Content.ReadFromJsonAsync<Todo>();
 
        var todo = await client.GetFromJsonAsync<Todo>("/todos/" + addedTodo?.Id);
    
        Assert.Equal("Second Todo", todo?.Title);
    }

    [Fact]
    public async Task GetNotFoundTodo()
    {
        var client = _app.CreateClient();
        var response = await client.GetAsync("/todos/1000");
    
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MarkTodoAsComplete()
    {
        Todo newTodo = new Todo { Title = "Third Todo" };

        var client = _app.CreateClient();
        var response = await client.PostAsJsonAsync("/todos", newTodo);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
 
        var addedTodo = await response.Content.ReadFromJsonAsync<Todo>();
        addedTodo!.IsComplete = true;

        var updateResponse = await client.PutAsJsonAsync("/todos/" + addedTodo.Id, addedTodo);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedTodo = await updateResponse.Content.ReadFromJsonAsync<Todo>();
    
        Assert.Equal("Third Todo", updatedTodo!.Title);
        Assert.True(updatedTodo.IsComplete);
    }

    [Fact]
    public async Task UpdateTodoTitle()
    {
        Todo newTodo = new Todo { Title = "Fourth Todo" };

        var client = _app.CreateClient();
        var response = await client.PostAsJsonAsync("/todos", newTodo);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
 
        var addedTodo = await response.Content.ReadFromJsonAsync<Todo>();
        addedTodo!.Title = "Updated Fourth Todo";

        var updateResponse = await client.PutAsJsonAsync("/todos/" + addedTodo.Id, addedTodo);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedTodo = await updateResponse.Content.ReadFromJsonAsync<Todo>();
    
        Assert.Equal("Updated Fourth Todo", updatedTodo!.Title);
        Assert.False(updatedTodo.IsComplete);
    }

    [Fact]
    public async Task UpdateTodoBadRequest()
    {
        Todo newTodo = new Todo { Title = "Update Bad Request Todo" };

        var client = _app.CreateClient();
        var response = await client.PostAsJsonAsync("/todos", newTodo);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
 
        var addedTodo = await response.Content.ReadFromJsonAsync<Todo>();
        addedTodo!.Title = "Updated Fourth Todo";

        var updateResponse = await client.PutAsJsonAsync("/todos/" + 1000, addedTodo);
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo()
    {
        Todo newTodo = new Todo { Title = "Second Todo" };

        var client = _app.CreateClient();
        var response = await client.PostAsJsonAsync("/todos", newTodo);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var addedTodo = await response.Content.ReadFromJsonAsync<Todo>();
 
        var deletedResponse = await client.DeleteAsync("/todos/" + addedTodo?.Id);
    
        Assert.Equal(HttpStatusCode.OK, deletedResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteNotFoundTodo()
    {
        var client = _app.CreateClient();
        var deletedResponse = await client.DeleteAsync("/todos/1000");
    
        Assert.Equal(HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }
}