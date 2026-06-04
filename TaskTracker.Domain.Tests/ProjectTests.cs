using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Tests;

public class ProjectTests
{
    private static Project CreateProject()
    {
        return new Project(
            "CRM",
            "Customer",
            "Executor",
            DateTime.Today,
            DateTime.Today.AddDays(10),
            ProjectPriority.High,
            null);
    }

    [Fact]
    public void Constructor_Should_Create_Project()
    {
        var project = CreateProject();

        Assert.Equal("CRM", project.Title);
        Assert.Equal(ProjectPriority.High, project.Priority);
        Assert.Equal(ProjectStatus.Active, project.Status);
    }

    [Fact]
    public void Constructor_Should_Throw_When_Title_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Project(
                "",
                "Customer",
                "Executor",
                DateTime.Today,
                DateTime.Today.AddDays(1),
                ProjectPriority.High,
                null));
    }

    [Fact]
    public void Constructor_Should_Throw_When_CustomerCompany_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Project(
                "CRM",
                "",
                "Executor",
                DateTime.Today,
                DateTime.Today.AddDays(1),
                ProjectPriority.High,
                null));
    }

    [Fact]
    public void Constructor_Should_Throw_When_ExecutorCompany_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Project(
                "CRM",
                "Customer",
                "",
                DateTime.Today,
                DateTime.Today.AddDays(1),
                ProjectPriority.High,
                null));
    }

    [Fact]
    public void Constructor_Should_Throw_When_StartDate_After_EndDate()
    {
        Assert.Throws<ArgumentException>(() =>
            new Project(
                "CRM",
                "Customer",
                "Executor",
                DateTime.Today.AddDays(10),
                DateTime.Today,
                ProjectPriority.High,
                null));
    }

    [Fact]
    public void Update_Should_Change_Project_Data()
    {
        var project = CreateProject();

        project.Update(
            "New Title",
            "New Customer",
            "New Executor",
            DateTime.Today,
            DateTime.Today.AddDays(30),
            ProjectPriority.Low);

        Assert.Equal("New Title", project.Title);
        Assert.Equal("New Customer", project.CustomerCompany);
        Assert.Equal("New Executor", project.ExecutorCompany);
        Assert.Equal(ProjectPriority.Low, project.Priority);
    }

    [Fact]
    public void Update_Should_Throw_When_Title_Is_Empty()
    {
        var project = CreateProject();

        Assert.Throws<ArgumentException>(() =>
            project.Update(
                "",
                "Customer",
                "Executor",
                DateTime.Today,
                DateTime.Today.AddDays(1),
                ProjectPriority.High));
    }

    [Fact]
    public void Update_Should_Throw_When_Dates_Are_Invalid()
    {
        var project = CreateProject();

        Assert.Throws<ArgumentException>(() =>
            project.Update(
                "CRM",
                "Customer",
                "Executor",
                DateTime.Today.AddDays(10),
                DateTime.Today,
                ProjectPriority.High));
    }

    [Fact]
    public void AddMember_Should_Add_New_Member()
    {
        var project = CreateProject();

        project.AddMember("user1");

        Assert.Single(project.Members);
    }

    [Fact]
    public void AddMember_Should_Not_Add_Duplicate_Member()
    {
        var project = CreateProject();

        project.AddMember("user1");
        project.AddMember("user1");

        Assert.Single(project.Members);
    }

    [Fact]
    public void AddMember_Should_Throw_When_UserId_Is_Empty()
    {
        var project = CreateProject();

        Assert.Throws<ArgumentException>(() =>
            project.AddMember(""));
    }

    [Fact]
    public void RemoveMember_Should_Remove_Member()
    {
        var project = CreateProject();

        project.AddMember("user1");

        project.RemoveMember("user1");

        Assert.Empty(project.Members);
    }

    [Fact]
    public void ChangeManager_Should_Change_Manager()
    {
        var project = CreateProject();

        project.ChangeManager("manager1");

        Assert.Equal("manager1", project.ManagerUserId);
    }

    [Fact]
    public void ChangeManager_Should_Throw_When_UserId_Is_Empty()
    {
        var project = CreateProject();

        Assert.Throws<ArgumentException>(() =>
            project.ChangeManager(""));
    }

    [Fact]
    public void ChangePriority_Should_Change_Priority()
    {
        var project = CreateProject();

        project.ChangePriority(ProjectPriority.Low);

        Assert.Equal(ProjectPriority.Low, project.Priority);
    }

    [Fact]
    public void ChangeStatus_Should_Change_Status()
    {
        var project = CreateProject();

        project.ChangeStatus(ProjectStatus.Completed);

        Assert.Equal(ProjectStatus.Completed, project.Status);
    }

    [Fact]
    public void HasMember_Should_Return_True()
    {
        var project = CreateProject();

        project.AddMember("user1");

        Assert.True(project.HasMember("user1"));
    }

    [Fact]
    public void HasMember_Should_Return_False()
    {
        var project = CreateProject();

        Assert.False(project.HasMember("user1"));
    }
}