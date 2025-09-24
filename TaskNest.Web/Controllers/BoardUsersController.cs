namespace TaskNest.Web.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [ApiController]
    [Route("api/boards/{boardId}/users")]
    public class BoardUsersController(
        IBoardUserService boardUserService,
        IBoardService boardService,
        UserManager<ApplicationUser> userManager) : ControllerBase
    {
        private readonly IBoardUserService _boardUserService = boardUserService;
        private readonly IBoardService _boardService = boardService;
        private readonly UserManager<ApplicationUser> _userManager = userManager; 

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardUserDto>>> GetBoardUsers(Guid boardId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Forbid("Authentication required.");

                var isUserInBoard = await _boardUserService.IsUserInBoardAsync(boardId, userId);
                if (!isUserInBoard)
                {
                    return Forbid("You are not a member of this board.");
                }

                var boardUsers = await _boardUserService.GetBoardUsersAsync(boardId);
                var boardUserDtos = boardUsers.Select(bu => new BoardUserDto
                {
                    Id = bu.Id,
                    BoardId = bu.BoardId,
                    ApplicationUserId = bu.ApplicationUserId,
                    Role = bu.Role,
                    UserEmail = bu.ApplicationUser?.Email
                });

                return Ok(boardUserDtos);
            }
            catch (Exception ex)
            {
                // do not expose internal exception objects in production - return message
                return StatusCode(500, new { message = "Failed to get board users.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BoardUserDto>> AddUserToBoard(Guid boardId, AddBoardUserDto addBoardUserDto)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return Forbid("Authentication required.");

                var currentUserRole = await _boardUserService.GetUserRoleInBoardAsync(boardId, currentUserId);
                if (currentUserRole != BoardUserRole.Admin)
                {
                    return Forbid("Only board admins can add users.");
                }

                // Basic validation
                if (string.IsNullOrWhiteSpace(addBoardUserDto.UserEmail))
                    return BadRequest(new { message = "UserEmail is required." });

                var boardUser = await _boardUserService.AddUserToBoardAsync(boardId, addBoardUserDto.UserEmail, addBoardUserDto.Role);

                var applicationUser = await _userManager.FindByIdAsync(boardUser.ApplicationUserId);

                var boardUserDto = new BoardUserDto
                {
                    Id = boardUser.Id,
                    BoardId = boardUser.BoardId,
                    ApplicationUserId = boardUser.ApplicationUserId,
                    Role = boardUser.Role,
                    UserEmail = applicationUser?.Email
                };

                return CreatedAtAction(nameof(GetBoardUsers), new { boardId }, boardUserDto);
            }
            catch (Exception ex)
            {
                // service throws helpful messages (e.g. "User not found" or "Already a member")
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateBoardUserRole(Guid boardId, string userId, [FromBody] BoardUserRole role)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return Forbid("Authentication required.");

                var currentUserRole = await _boardUserService.GetUserRoleInBoardAsync(boardId, currentUserId);
                if (currentUserRole != BoardUserRole.Admin)
                {
                    return Forbid("Only board admins can update user roles.");
                }

                var success = await _boardUserService.UpdateBoardUserRoleAsync(boardId, userId, role);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUserFromBoard(Guid boardId, string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                    return Forbid("Authentication required.");

                var currentUserRole = await _boardUserService.GetUserRoleInBoardAsync(boardId, currentUserId);
                if (currentUserRole != BoardUserRole.Admin)
                {
                    return Forbid("Only board admins can remove users.");
                }

                var success = await _boardUserService.RemoveUserFromBoardAsync(boardId, userId);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
