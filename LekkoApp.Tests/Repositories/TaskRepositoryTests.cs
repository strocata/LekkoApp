using FluentAssertions;
using LekkoApp.Data;
using LekkoApp.Models;
using LekkoApp.Models.Enums;
using LekkoApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Task = LekkoApp.Models.PomodoroTask;
using TaskStatus = LekkoApp.Models.Enums.TaskStatus;

namespace LekkoApp.Tests.Repositories;

public class TaskRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TaskRepository _repository;
    private readonly Mock<ILogger<TaskRepository>> _loggerMock;
    private readonly ApplicationUser _testUser;

    public TaskRepositoryTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<TaskRepository>>();
        _repository = new TaskRepository(_context, _loggerMock.Object);

        // Create test user
        _testUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser@example.com",
            Email = "testuser@example.com"
        };
        
        _context.Users.Add(_testUser);
        _context.SaveChanges();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_ShouldCreateTask_WithValidData()
    {
        // Arrange
        var task = new Task
        {
            Title = "Test Task",
            Description = "Test Description",
            EstimatedPomodoros = 3,
            Status = TaskStatus.NotStarted,
            Priority = Priority.High,
            User = null
        };

        // Act
        var result = await _repository.CreateAsync(task, _testUser);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Title.Should().Be("Test Task");
        result.User.Should().Be(_testUser);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.CompletedPomodoros.Should().Be(0);

        var savedTask = await _context.PomodoroTasks.FindAsync(result.Id);
        savedTask.Should().NotBeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_ShouldThrowException_WhenUserIsNull()
    {
        // Arrange
        var task = new Task
        {
            Title = "Test Task",
            User = null
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _repository.CreateAsync(task, null));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var task = await CreateTestTask("Find Me Task");

        // Act
        var result = await _repository.GetByIdAsync(task.Id, includeRelated: false);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(task.Id);
        result.Title.Should().Be("Find Me Task");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), includeRelated: false);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByUserAsync_ShouldReturnOnlyUserTasks()
    {
        // Arrange
        await CreateTestTask("User 1 Task 1");
        await CreateTestTask("User 1 Task 2");
        
        var otherUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "other@example.com",
            Email = "other@example.com"
        };
        _context.Users.Add(otherUser);
        await _context.SaveChangesAsync();
        
        var otherTask = new Task
        {
            Id = Guid.NewGuid(),
            Title = "Other User Task",
            User = otherUser,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.PomodoroTasks.Add(otherTask);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetByUserAsync(_testUser, includeRelated: false);

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(t => t.User == _testUser);
        results.Should().NotContain(t => t.Title == "Other User Task");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ShouldUpdateTask_WithNewValues()
    {
        // Arrange
        var task = await CreateTestTask("Original Title");
        var originalUpdatedAt = task.UpdatedAt;
        
        await System.Threading.Tasks.Task.Delay(100); // Ensure time difference

        task.Title = "Updated Title";
        task.Status = TaskStatus.InProgress;
        task.Priority = Priority.Low;

        // Act
        var result = await _repository.UpdateAsync(task);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("Updated Title");
        result.Status.Should().Be(TaskStatus.InProgress);
        result.Priority.Should().Be(Priority.Low);
        result.UpdatedAt.Should().BeAfter(originalUpdatedAt!.Value);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var nonExistentTask = new Task
        {
            Id = Guid.NewGuid(),
            Title = "Non-existent",
            User = _testUser
        };

        // Act
        var result = await _repository.UpdateAsync(nonExistentTask);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ShouldRemoveTask_WhenTaskExists()
    {
        // Arrange
        var task = await CreateTestTask("Task to Delete");

        // Act
        var result = await _repository.DeleteAsync(task.Id);

        // Assert
        result.Should().BeTrue();
        var deletedTask = await _context.PomodoroTasks.FindAsync(task.Id);
        deletedTask.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetOverdueTasksAsync_ShouldReturnOnlyOverdueTasks()
    {
        // Arrange
        var overdueTask1 = await CreateTestTask("Overdue 1", dueDate: DateTime.UtcNow.AddDays(-2));
        var overdueTask2 = await CreateTestTask("Overdue 2", dueDate: DateTime.UtcNow.AddDays(-1));
        var futureTask = await CreateTestTask("Future", dueDate: DateTime.UtcNow.AddDays(1));
        var completedOverdueTask = await CreateTestTask("Completed Overdue", 
            dueDate: DateTime.UtcNow.AddDays(-3), 
            status: TaskStatus.Completed);

        // Act
        var results = await _repository.GetOverdueTasksAsync(_testUser);

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(t => t.Id == overdueTask1.Id);
        results.Should().Contain(t => t.Id == overdueTask2.Id);
        results.Should().NotContain(t => t.Id == futureTask.Id);
        results.Should().NotContain(t => t.Id == completedOverdueTask.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByStatusAsync_ShouldFilterTasksByStatus()
    {
        // Arrange
        await CreateTestTask("Not Started", status: TaskStatus.NotStarted);
        await CreateTestTask("In Progress 1", status: TaskStatus.InProgress);
        await CreateTestTask("In Progress 2", status: TaskStatus.InProgress);
        await CreateTestTask("Completed", status: TaskStatus.Completed);

        // Act
        var results = await _repository.GetByStatusAsync(_testUser, TaskStatus.InProgress);

        // Assert
        results.Should().HaveCount(2);
        results.Should().OnlyContain(t => t.Status == TaskStatus.InProgress);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetStatisticsAsync_ShouldReturnCorrectStats()
    {
        // Arrange
        await CreateTestTask("Task 1", status: TaskStatus.NotStarted, estimatedPomodoros: 3);
        await CreateTestTask("Task 2", status: TaskStatus.InProgress, estimatedPomodoros: 4);
        await CreateTestTask("Task 3", status: TaskStatus.Completed, estimatedPomodoros: 2);
        await CreateTestTask("Task 4", status: TaskStatus.Completed, estimatedPomodoros: 5);
        await CreateTestTask("Overdue", status: TaskStatus.NotStarted, 
            dueDate: DateTime.UtcNow.AddDays(-1), estimatedPomodoros: 3);

        // Act
        var stats = await _repository.GetStatisticsAsync(_testUser);

        // Assert
        stats.TotalTasks.Should().Be(5);
        stats.CompletedTasks.Should().Be(2);
        stats.InProgressTasks.Should().Be(1);
        stats.NotStartedTasks.Should().Be(2);
        stats.OverdueTasks.Should().Be(1);
        stats.EstimatedPomodoros.Should().Be(17);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExistsAsync_ShouldReturnTrue_WhenTaskExists()
    {
        // Arrange
        var task = await CreateTestTask("Existing Task");

        // Act
        var result = await _repository.ExistsAsync(task.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task ExistsAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
    {
        // Act
        var result = await _repository.ExistsAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    // Helper method to create test tasks
    private async Task<Task> CreateTestTask(
        string title, 
        TaskStatus status = TaskStatus.NotStarted,
        DateTime? dueDate = null,
        int estimatedPomodoros = 1)
    {
        var task = new Task
        {
            Id = Guid.NewGuid(),
            Title = title,
            User = _testUser,
            Status = status,
            DueDate = dueDate,
            EstimatedPomodoros = estimatedPomodoros,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CompletedPomodoros = 0
        };

        _context.PomodoroTasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}