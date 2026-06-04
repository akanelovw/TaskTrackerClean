using Moq;
using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Projects.ChangeProjectStatus;
using TaskTracker.Application.Projects.GetProjectDetails;
using TaskTracker.Application.Projects.GetProjectsList;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Tests.Projects;

public class ProjectApplicationTests
{
    private readonly Mock<IProjectRepository> _repo = new();
    private readonly Mock<IUserService> _userService = new();

    private ChangeProjectStatusUseCase CreateChangeStatusSut()
        => new ChangeProjectStatusUseCase(
            _repo.Object,
            _userService.Object);

    [Fact]
    public async Task ChangeStatus_Should_Work_When_Admin()
    {
        var project = CreateProject();

        _repo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);

        _userService.Setup(x => x.IsInRole(Roles.Admin))
            .Returns(true);

        var sut = CreateChangeStatusSut();

        await sut.Execute(new ChangeProjectStatusRequest
        {
            ProjectId = 1,
            Status = ProjectStatus.Completed
        });

        Assert.Equal(ProjectStatus.Completed, project.Status);
    }

    [Fact]
    public async Task ChangeStatus_Should_Throw_When_Not_Found()
    {
        _repo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Project?)null);

        _userService.Setup(x => x.IsInRole(It.IsAny<string>()))
            .Returns(true);

        var sut = CreateChangeStatusSut();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Execute(new ChangeProjectStatusRequest
            {
                ProjectId = 1,
                Status = ProjectStatus.Completed
            }));
    }

    [Fact]
    public async Task ChangeStatus_Should_Throw_When_Forbidden()
    {
        var project = CreateProject();

        _repo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);

        _userService.Setup(x => x.IsInRole(It.IsAny<string>()))
            .Returns(false);

        var sut = CreateChangeStatusSut();

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            sut.Execute(new ChangeProjectStatusRequest
            {
                ProjectId = 1,
                Status = ProjectStatus.Completed
            }));
    }
    private GetProjectDetailsUseCase CreateDetailsSut()
        => new GetProjectDetailsUseCase(
            _repo.Object,
            _userService.Object);

    [Fact]
    public async Task GetDetails_Should_Return_Project()
    {
        var project = CreateProject();

        _repo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(project);

        _userService.Setup(x => x.IsInRole(Roles.Admin))
            .Returns(true);

        var sut = CreateDetailsSut();

        var result = await sut.Execute(1);

        Assert.Equal(project.Title, result.Title);
        Assert.Equal(project.CustomerCompany, result.CustomerCompany);
    }

    [Fact]
    public async Task GetDetails_Should_Throw_When_Not_Found()
    {
        _repo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Project?)null);

        var sut = CreateDetailsSut();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            sut.Execute(1));
    }

    private GetProjectsListUseCase CreateListSut()
        => new GetProjectsListUseCase(
            _repo.Object,
            _userService.Object);

    [Fact]
    public async Task GetList_Should_Return_Projects()
    {
        var projects = new List<Project>
        {
            CreateProject()
        };

        _repo.Setup(x => x.Query())
            .Returns(projects.AsQueryable());

        _userService.Setup(x => x.IsInRole(It.IsAny<string>()))
            .Returns(true);

        var sut = CreateListSut();

        var result = await sut.Execute(new GetProjectsListRequest
        {
            Page = 1,
            PageSize = 10
        });

        Assert.Single(result);
    }

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
}