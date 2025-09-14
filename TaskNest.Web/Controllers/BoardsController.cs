namespace TaskNest.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController(IBoardService boardService, IMapper mapper) : ControllerBase
{
    private readonly IBoardService _boardService = boardService;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoardDto>>> GetBoards()
    {
        var boards = await _boardService.GetAllAsync();
        var boardDtos = _mapper.Map<IEnumerable<BoardDto>>(boards);
        return Ok(boardDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BoardDto>> GetBoard(Guid id)
    {
        var board = await _boardService.GetByIdAsync(id);
        if (board == null)
        {
            return NotFound();
        }

        var boardDto = _mapper.Map<BoardDto>(board);
        return Ok(boardDto);
    }

    [HttpPost]
    public async Task<ActionResult<BoardDto>> CreateBoard(BoardDto dto)
    {
        var board = _mapper.Map<Board>(dto);
        board.Id = Guid.NewGuid();

        var created = await _boardService.CreateAsync(board);
        var createdDto = _mapper.Map<BoardDto>(created);

        return CreatedAtAction(nameof(GetBoard), new { id = createdDto.Id }, createdDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(Guid id, BoardDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest("ID mismatch");
        }

        var entity = _mapper.Map<Board>(dto);
        var updated = await _boardService.UpdateAsync(entity);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        var deleted = await _boardService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}