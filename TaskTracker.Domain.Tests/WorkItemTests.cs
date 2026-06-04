using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Tests;

public class WorkItemTests
{
    private static WorkItem CreateWorkItem()
    {
        return new WorkItem(
            "Implement auth",
            "user1",
            1,
            WorkItemPriority.High);
    }

    [Fact]
    public void Constructor_Should_Create_WorkItem()
    {
        var workItem = CreateWorkItem();

        Assert.Equal("Implement auth", workItem.Title);
        Assert.Equal("user1", workItem.CreatedByUserId);
        Assert.Equal(1, workItem.ProjectId);
        Assert.Equal(WorkItemPriority.High, workItem.Priority);
        Assert.Equal(WorkItemStatus.ToDo, workItem.Status);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Title_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                "",
                "user1",
                1,
                WorkItemPriority.High));
    }

    [Fact]
    public void Constructor_Should_Throw_When_CreatedByUserId_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                "Task",
                "",
                1,
                WorkItemPriority.High));
    }

    [Fact]
    public void Constructor_Should_Throw_When_ProjectId_Is_Invalid()
    {
        Assert.Throws<ArgumentException>(() =>
            new WorkItem(
                "Task",
                "user1",
                0,
                WorkItemPriority.High));
    }

    [Fact]
    public void UpdateTitle_Should_Change_Title()
    {
        var workItem = CreateWorkItem();

        workItem.UpdateTitle("New title");

        Assert.Equal("New title", workItem.Title);
    }

    [Fact]
    public void UpdateTitle_Should_Throw_When_Title_Is_Empty()
    {
        var workItem = CreateWorkItem();

        Assert.Throws<ArgumentException>(() =>
            workItem.UpdateTitle(""));
    }

    [Fact]
    public void UpdateComment_Should_Change_Comment()
    {
        var workItem = CreateWorkItem();

        workItem.UpdateComment("Test comment");

        Assert.Equal("Test comment", workItem.Comment);
    }

    [Fact]
    public void UpdateComment_Should_Allow_Null()
    {
        var workItem = CreateWorkItem();

        workItem.UpdateComment(null);

        Assert.Null(workItem.Comment);
    }

    [Fact]
    public void ChangeStatus_Should_Change_Status()
    {
        var workItem = CreateWorkItem();

        workItem.ChangeStatus(WorkItemStatus.InProgress);

        Assert.Equal(WorkItemStatus.InProgress, workItem.Status);
    }

    [Fact]
    public void ChangeStatus_Should_Throw_When_Moving_From_Done_To_ToDo()
    {
        var workItem = CreateWorkItem();

        workItem.ChangeStatus(WorkItemStatus.Done);

        Assert.Throws<InvalidOperationException>(() =>
            workItem.ChangeStatus(WorkItemStatus.ToDo));
    }

    [Fact]
    public void ChangePriority_Should_Change_Priority()
    {
        var workItem = CreateWorkItem();

        workItem.ChangePriority(WorkItemPriority.Low);

        Assert.Equal(WorkItemPriority.Low, workItem.Priority);
    }

    [Fact]
    public void AssignUser_Should_Assign_User()
    {
        var workItem = CreateWorkItem();

        workItem.AssignUser("user2");

        Assert.Equal("user2", workItem.AssignedUserId);
    }

    [Fact]
    public void AssignUser_Should_Throw_When_UserId_Is_Empty()
    {
        var workItem = CreateWorkItem();

        Assert.Throws<ArgumentException>(() =>
            workItem.AssignUser(""));
    }

    [Fact]
    public void UnassignUser_Should_Clear_Assigned_User()
    {
        var workItem = CreateWorkItem();

        workItem.AssignUser("user2");

        workItem.UnassignUser();

        Assert.Null(workItem.AssignedUserId);
    }

    [Fact]
    public void IsAssigned_Should_Return_True_When_User_Assigned()
    {
        var workItem = CreateWorkItem();

        workItem.AssignUser("user2");

        Assert.True(workItem.IsAssigned);
    }

    [Fact]
    public void IsAssigned_Should_Return_False_When_User_Not_Assigned()
    {
        var workItem = CreateWorkItem();

        Assert.False(workItem.IsAssigned);
    }

    [Fact]
    public void IsDone_Should_Return_True_When_Status_Is_Done()
    {
        var workItem = CreateWorkItem();

        workItem.ChangeStatus(WorkItemStatus.Done);

        Assert.True(workItem.IsDone);
    }

    [Fact]
    public void IsDone_Should_Return_False_When_Status_Is_Not_Done()
    {
        var workItem = CreateWorkItem();

        Assert.False(workItem.IsDone);
    }
}