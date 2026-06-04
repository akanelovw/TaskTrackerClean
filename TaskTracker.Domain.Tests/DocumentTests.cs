using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Tests;

public class DocumentTests
{
    private static Document CreateDocument()
    {
        return new Document(
            "file.pdf",
            "/files/file.pdf",
            1);
    }

    [Fact]
    public void Constructor_Should_Create_Document()
    {
        var doc = CreateDocument();

        Assert.Equal("file.pdf", doc.FileName);
        Assert.Equal("/files/file.pdf", doc.FilePath);
        Assert.Equal(1, doc.ProjectId);
    }

    [Fact]
    public void Constructor_Should_Throw_When_FileName_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Document(
                "",
                "/files/file.pdf",
                1));
    }

    [Fact]
    public void Constructor_Should_Throw_When_FilePath_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Document(
                "file.pdf",
                "",
                1));
    }

    [Fact]
    public void Constructor_Should_Throw_When_ProjectId_Is_Invalid()
    {
        Assert.Throws<ArgumentException>(() =>
            new Document(
                "file.pdf",
                "/files/file.pdf",
                0));
    }

    [Fact]
    public void Rename_Should_Change_FileName()
    {
        var doc = CreateDocument();

        doc.Rename("new.pdf");

        Assert.Equal("new.pdf", doc.FileName);
    }

    [Fact]
    public void Rename_Should_Throw_When_FileName_Is_Empty()
    {
        var doc = CreateDocument();

        Assert.Throws<ArgumentException>(() =>
            doc.Rename(""));
    }

    [Fact]
    public void ChangePath_Should_Change_FilePath()
    {
        var doc = CreateDocument();

        doc.ChangePath("/new/path.pdf");

        Assert.Equal("/new/path.pdf", doc.FilePath);
    }

    [Fact]
    public void ChangePath_Should_Throw_When_FilePath_Is_Empty()
    {
        var doc = CreateDocument();

        Assert.Throws<ArgumentException>(() =>
            doc.ChangePath(""));
    }
}