namespace TaskNest.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController(ITaskItemService taskService, IMapper mapper) : ControllerBase
    {
        private readonly ITaskItemService _taskService = taskService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTaskItems()
        {
            var taskItems = await _taskService.GetAllAsync();
            var taskItemDtos = _mapper.Map<IEnumerable<TaskItemDto>>(taskItems);
            return Ok(taskItemDtos);
        }

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

        [HttpGet("byColumn/{columnId}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksByColumn(Guid columnId)
        {
            var tasks = await _taskService.GetByColumnAsync(columnId);
            var taskDtos = _mapper.Map<IEnumerable<TaskItemDto>>(tasks);
            return Ok(taskDtos);
        }

        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskItem(TaskItemDto taskItemDto)
        {
            var taskItem = _mapper.Map<TaskItem>(taskItemDto);
            var createdTask = await _taskService.CreateAsync(taskItem);
            var createdDto = _mapper.Map<TaskItemDto>(createdTask);

            return CreatedAtAction(nameof(GetTaskItem), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(Guid id, TaskItemDto taskItemDto)
        {
            if (id != taskItemDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var taskItem = _mapper.Map<TaskItem>(taskItemDto);
            var updated = await _taskService.UpdateAsync(taskItem);

            if (updated == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(Guid id)
        {
            var deleted = await _taskService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
