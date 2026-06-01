using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Common;
using TaskTracker.Application.Documents.AddDocument;
using TaskTracker.Application.Documents.DeleteDocument;
using TaskTracker.Application.Documents.GetProjectDocuments;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly AddDocumentUseCase _add;
    private readonly DeleteDocumentUseCase _delete;
    private readonly GetProjectDocumentsUseCase _get;

    public DocumentsController(
        AddDocumentUseCase add,
        DeleteDocumentUseCase delete,
        GetProjectDocumentsUseCase get)
    {
        _add = add;
        _delete = delete;
        _get = get;
    }

    // ================= GET BY PROJECT =================
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var result = await _get.Execute(new GetProjectDocumentsRequest
        {
            ProjectId = projectId
        });

        return Ok(ApiResponse.Ok(result));
    }

    // ================= ADD =================
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] AddDocumentRequest request)
    {
        await _add.Execute(request);

        return Ok(ApiResponse.Ok("Document uploaded"));
    }

    // ================= DELETE =================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _delete.Execute(new DeleteDocumentRequest
        {
            DocumentId = id
        });

        return Ok(ApiResponse.Ok("Document deleted"));
    }
}