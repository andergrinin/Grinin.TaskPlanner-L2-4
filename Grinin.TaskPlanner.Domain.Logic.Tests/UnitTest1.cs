using Grinin.TaskPlanner.Domain.Logic;
using Grinin.TaskPlanner.Domain.Models;
using Grinin.TaskPlanner.DataAccess.Abstractions;
using Moq;
using System;
using System.Linq;
using Xunit;

public class SimpleTaskPlannerTests
{
    [Fact]
    public void CreatePlan_SortsTasks_Correctly()
    {
        // Arrange
        var mockRepository = new Mock<IWorkItemsRepository>();
        var tasks = new[]
        {
        new WorkItem { Title = "Task 3", Priority = Priority.Low, DueDate = DateTime.Now.AddDays(3), IsCompleted = false },
        new WorkItem { Title = "Task 1", Priority = Priority.High, DueDate = DateTime.Now.AddDays(1), IsCompleted = false },
        new WorkItem { Title = "Task 2", Priority = Priority.Medium, DueDate = DateTime.Now.AddDays(2), IsCompleted = false },
    };

        mockRepository.Setup(repo => repo.GetAll()).Returns(tasks);

        var planner = new SimpleTaskPlanner(mockRepository.Object);

        // Act
        var result = planner.CreatePlan();

        // Assert
        var expectedTitles = new[] { "Task 1", "Task 2", "Task 3" };

        Assert.Equal(expectedTitles.Length, result.Length);

        for (int i = 0; i < expectedTitles.Length; i++)
        {
            Assert.Equal(expectedTitles[i], result[i].Title);
        }
    }


    [Fact]
    public void CreatePlan_IncludesOnlyNonCompletedTasks()
    {
        // Arrange
        var mockRepository = new Mock<IWorkItemsRepository>();
        var tasks = new[]
        {
            new WorkItem { Title = "Task 1", IsCompleted = false },
            new WorkItem { Title = "Task 2", IsCompleted = true },
            new WorkItem { Title = "Task 3", IsCompleted = false },
        };

        mockRepository.Setup(repo => repo.GetAll()).Returns(tasks);

        var planner = new SimpleTaskPlanner(mockRepository.Object);

        // Act
        var result = planner.CreatePlan();

        // Assert
        Assert.Equal(2, result.Length);
        Assert.All(result, item => Assert.False(item.IsCompleted));
    }

    [Fact]
    public void CreatePlan_ExcludesCompletedTasks()
    {
        // Arrange
        var mockRepository = new Mock<IWorkItemsRepository>();
        var tasks = new[]
        {
        new WorkItem { Title = "Task 1", IsCompleted = false },
        new WorkItem { Title = "Task 2", IsCompleted = true },
        new WorkItem { Title = "Task 3", IsCompleted = false },
    };

        mockRepository.Setup(repo => repo.GetAll()).Returns(tasks);

        var planner = new SimpleTaskPlanner(mockRepository.Object);

        // Act
        var result = planner.CreatePlan();

        // Assert
        Assert.DoesNotContain(result, item => item.IsCompleted);
        Assert.All(result, item => Assert.False(item.IsCompleted));
    }

}
