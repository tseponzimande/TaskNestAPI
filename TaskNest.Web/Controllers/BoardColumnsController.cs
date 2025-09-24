namespace TaskNest.Web.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardColumnsController(IBoardColumnService columnService, IMapper mapper) : ControllerBase
    {
        private readonly IBoardColumnService _columnService = columnService;
        private readonly IMapper _mapper = mapper;


        /// <summary>
        /// Get columns by board ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("byBoard/{boardId}")]
        public async Task<ActionResult<IEnumerable<BoardColumnDto>>> GetColumns(Guid boardId)
        {
            var columns = await _columnService.GetColumnsAsync(boardId);
            var columnsDto = _mapper.Map<IEnumerable<BoardColumnDto>>(columns);
            return Ok(columnsDto);
        }

        /// <summary>
        /// Get a specific column by ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardColumnDto>> GetColumn(Guid id)
        {
            var column = await _columnService.GetByIdAsync(id);
            if (column == null)
            {
                return NotFound();
            }

            var columnDto = _mapper.Map<BoardColumnDto>(column);
            return Ok(columnDto);
        }

        /// <summary>
        /// Create a new column
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<BoardColumnDto>> Create(BoardColumnDto columnDto)
        {
            var column = _mapper.Map<BoardColumn>(columnDto);
            column.Id = Guid.NewGuid();

            var createdColumn = await _columnService.CreateAsync(column);
            var createdDto = _mapper.Map<BoardColumnDto>(createdColumn);

            return CreatedAtAction(nameof(GetColumn), new { id = createdDto.Id }, createdDto);
        }

        /// <summary>
        /// Update an existing column
        /// </summary>
        /// <returns></returns>

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, BoardColumnDto columnDto)
        {
            if (id != columnDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var column = _mapper.Map<BoardColumn>(columnDto);
            var updated = await _columnService.UpdateAsync(column);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Delete a column
        /// </summary>
        /// <returns></returns>

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _columnService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}