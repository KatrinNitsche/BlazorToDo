using CollaborateSoftware.ToDo.Backend.Data;
using CollaborateSoftware.ToDo.Backend.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Xunit;

namespace CollaborateSoftware.ToDo.Tests
{
    //public class ToDoServiceShould
    //{
    //    [Fact]
    //    public async void ReturnAListOfTasks()
    //    {
    //        // Arrange
    //        var options = new DbContextOptionsBuilder<DataContext>()
    //           .UseInMemoryDatabase("InMemoryData")
    //           .Options;
    //        ToDoService sut = new ToDoService();
    //        List<ToDoListEntry> expected = new List<ToDoListEntry>();

    //        // Act
    //        var actual = await sut.GetAll();

    //        // Assert
    //        Assert.Equal(expected, actual);
    //    }

    //    [Fact]
    //    public async void AddAnewTask()
    //    {
    //        // Arrange
    //        ToDoService sut = new ToDoService();
    //        var expected = new ToDoListEntry() { Title = "new task", Date = new System.DateTime(2022, 3, 22), Done = false };

    //        // Act
    //        var actual = await sut.Add(new ToDoListEntry() { Title = "new task", Date = new System.DateTime(2022, 3, 22), Done = false });

    //        // Assert
    //        Assert.NotNull(actual);
    //        Assert.Equal(expected.Title, actual.Title);
    //        Assert.Equal(expected.Date, actual.Date);
    //        Assert.Equal(expected.Done, actual.Done);
    //    }
    //}
}
