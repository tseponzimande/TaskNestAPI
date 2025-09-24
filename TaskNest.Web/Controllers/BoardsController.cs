namespace TaskNest.Web.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController(IBoardService boardService, IMapper mapper, IBoardUserService _boardUserService) : ControllerBase
    {
        private readonly IBoardService _boardService = boardService;
        private readonly IMapper _mapper = mapper;
        private readonly IBoardUserService _boardUserService = _boardUserService;

        /// <summary>
        /// Get all boards
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardDto>>> GetBoards()
        {
            var boards = await _boardService.GetAllAsync();
            var boardDtos = _mapper.Map<IEnumerable<BoardDto>>(boards);
            return Ok(boardDtos);
        }

        /// <summary>
        /// Get a specific board by ID
        /// </summary>
        /// <returns></returns>

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


        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            try
            {
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<BoardDto>> CreateBoard(BoardDto dto)
        {
            try
            {
                var board = _mapper.Map<Board>(dto);
                board.Id = Guid.NewGuid();
                var created = await _boardService.CreateAsync(board);

                // Get the user's ID and email from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

                if (string.IsNullOrEmpty(userEmail))
                {
                    // Fallback: Fetch the user's email from the database using the user ID
                    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.FindByIdAsync(userId);
                    userEmail = user?.Email;
                }

                await _boardUserService.AddUserToBoardAsync(created.Id, userEmail, BoardUserRole.Admin);

                var createdDto = _mapper.Map<BoardDto>(created);
                return CreatedAtAction(nameof(GetBoard), new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing board
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Delete a board by ID
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(Guid id)
        {
            var deleted = await _boardService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}