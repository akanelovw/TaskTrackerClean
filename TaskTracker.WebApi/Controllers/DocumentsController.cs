using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Common;
using TaskTracker.Application.Documents.AddDocument;
using TaskTracker.Application.Documents.DeleteDocument;
using TaskTracker.Application.Documents.GetDocument;
using TaskTracker.Application.Documents.GetProjectDocuments;
using TaskTracker.WebApi.Requests;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/project/documents")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly GetProjectDocumentsUseCase _getDocuments;
    private readonly AddDocumentUseCase _addDocument;
    private readonly DeleteDocumentUseCase _deleteDocument;
    private readonly GetDocumentUseCase _getDocument;

    public DocumentsController(
        GetProjectDocumentsUseCase getDocuments,
        AddDocumentUseCase addDocument,
        DeleteDocumentUseCase deleteDocument,
        GetDocumentUseCase getDocument)
    {
        _getDocuments = getDocuments;
        _addDocument = addDocument;
        _deleteDocument = deleteDocument;
        _getDocument = getDocument;
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _getDocuments.Execute(
            new GetProjectDocumentsRequest
            {
                ProjectId = projectId
            });

        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost("{projectId}")]
    public async Task<IActionResult> Add(
    int projectId,
    [FromForm] UploadDocumentRequest request)
    {
        await using var stream = request.File.OpenReadStream();

        await _addDocument.Execute(new AddDocumentRequest
        {
            ProjectId = projectId,
            FileName = request.File.FileName,
            FileStream = stream
        });

        return Ok(ApiResponse.Ok());
    }

    [HttpDelete("{projectId}/{documentId}")]
    public async Task<IActionResult> Delete(
        int projectId,
        int documentId)
    {
        await _deleteDocument.Execute(
            new DeleteDocumentRequest
            {
                ProjectId = projectId,
                DocumentId = documentId
            });

        return Ok(ApiResponse.Ok());
    }

    [HttpGet("download/{documentId}")]
    public async Task<IActionResult> Download(
    int documentId)
    {
        var document =
            await _getDocument.Execute(
                new GetDocumentRequest
                {
                    DocumentId = documentId
                });

        var fullPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            document.FilePath.TrimStart('/'));

        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        var stream = System.IO.File.OpenRead(fullPath);

        return File(
            stream,
            "application/octet-stream",
            document.FileName);
    }

}