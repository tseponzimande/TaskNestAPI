namespace TaskNest.Web.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController(ITaskItemService taskService, IMapper mapper, IBoardService _boardService, IBoardUserService _boardUserService) : ControllerBase
    {
        private readonly ITaskItemService _taskService = taskService;
        private readonly IMapper _mapper = mapper;
        private readonly IBoardService _boardService = _boardService;
        private readonly IBoardUserService _boardUserService = _boardUserService;

        /// <summary>
        /// Get all task items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTaskItems()
        {
            var taskItems = await _taskService.GetAllAsync();

            var taskItemDtos = _mapper.Map<IEnumerable<TaskItemDto>>(taskItems);

            return Ok(taskItemDtos);
        }


        /// <summary>
        /// Get a specific task item by ID
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskItem(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            var taskDto = _mapper.Map<TaskItemDto>(task);
            return Ok(taskDto);
        }


        /// <summary>
        /// Get tasks by column ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("byColumn/{columnId}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksByColumn(Guid columnId)
        {
            var tasks = await _taskService.GetByColumnAsync(columnId);

            var taskDtos = _mapper.Map<IEnumerable<TaskItemDto>>(tasks);

            return Ok(taskDtos);
        }


        /// <summary>
        /// Create a new task item
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskItem(TaskItemDto taskItemDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isUserInBoard = await _boardUserService.IsUserInBoardAsync(taskItemDto.BoardId, userId!);
            if (!isUserInBoard)
            {
                return Forbid("You are not a member of this board.");
            }

            var taskItem = _mapper.Map<TaskItem>(taskItemDto);
            var createdTask = await _taskService.CreateAsync(taskItem);
            var createdDto = _mapper.Map<TaskItemDto>(createdTask);
            return CreatedAtAction(nameof(GetTaskItem), new { id = createdDto.Id }, createdDto);
        }


        //[HttpPost]
        //public async Task<ActionResult<TaskItemDto>> CreateTaskItem(TaskItemDto taskItemDto)
        //{
        //    // Check if the current user owns the board
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var board = await _boardService.GetByIdAsync(taskItemDto.BoardId);

        //    if (board?.ApplicationUserId != userId)
        //    {
        //        return Forbid("You do not own this board.");
        //    }

        //    var taskItem = _mapper.Map<TaskItem>(taskItemDto);

        //    var createdTask = await _taskService.CreateAsync(taskItem);

        //    var createdDto = _mapper.Map<TaskItemDto>(createdTask);

        //    return CreatedAtAction(nameof(GetTaskItem), new { id = createdDto.Id }, createdDto);
        //}


        /// <summary>
        /// Update an existing task item
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(Guid id, TaskItemDto taskItemDto)
        {
            if (id != taskItemDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            // Check if the current user owns the board
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var board = await _boardService.GetByIdAsync(taskItemDto.BoardId);
            if (board?.ApplicationUserId != userId)
            {
                return Forbid("You do not own this board.");
            }

            var taskItem = _mapper.Map<TaskItem>(taskItemDto);
            var updated = await _taskService.UpdateAsync(taskItem);

            if (updated == null)
            {
                return NotFound();
            }

            return NoContent();
        }


        /// <summary>
        /// Delete a task item
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Check if the current user owns the board
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var board = await _boardService.GetByIdAsync(task.BoardId);

            if (board?.ApplicationUserId != userId)
            {
                return Forbid("You do not own this board.");
            }

            var deleted = await _taskService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
