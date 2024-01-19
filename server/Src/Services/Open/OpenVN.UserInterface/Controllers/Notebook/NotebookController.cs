using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Domain;
using SharedKernel.Auth;

namespace OpenVN.Api
{

    public class NotebookController : BaseController<Note>
    {
        public NotebookController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        #region Note
        [AllowAnonymous]
        [HttpGet("note-by-id/{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetNoteByIdQuery(id), cancellationToken);
            if (result == null)
            {
                var response = new SimpleDataResult
                {
                    Status = "error",
                    Error = new Error
                    {
                        Code = 403,
                        Message = "Not permission!"
                    }
                };
                return Ok(response);
            }

            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("notes")]
        public async Task<IActionResult> GetNotes(string query, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetNotesQuery(query), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpPost("add-note")]
        public async Task<IActionResult> Post(NoteDto note, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CreateNoteCommand(note), cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpPut("update-note")]
        public async Task<IActionResult> Put(ChangeNoteDto note, int type, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new UpdateNoteCommand(note, type), cancellationToken);
            return Ok(new SimpleDataResult());
        }

        [HttpDelete("delete-note")]
        public async Task<IActionResult> Delete(List<string> ids, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteNoteCommand(ids), cancellationToken);
            return Ok(new SimpleDataResult());
        }
        #endregion

        #region NoteCategory
        [HttpGet("categories")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllNoteCategoryQuery(), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> Post(NoteCategoryDto note, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CreateNoteCategoryCommand(note), cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpPut("update-category")]
        public async Task<IActionResult> Put(NoteCategoryDto note, CancellationToken cancellationToken = default)
        {
            var command = new UpdateNoteCategoryCommand(note);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpDelete("delete-category")]
        public async Task<IActionResult> DeleteCategory(List<string> ids, CancellationToken cancellationToken = default)
        {
            var command = new DeleteNoteCategoryCommand(ids);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }
        #endregion
    }
}
