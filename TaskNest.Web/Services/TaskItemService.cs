namespace TaskNest.Web.Services
{
    public class TaskItemService(
        IGenericRepository<TaskItem> repository,
        IEmailService emailService,
        IHttpContextAccessor httpContextAccessor) : ITaskItemService
    {
        private readonly IGenericRepository<TaskItem> _repository = repository;
        private readonly IEmailService _emailService = emailService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            if (_repository is GenericRepository<TaskItem> repo)
            {
                return await repo.Query().Include(t => t.Board).ToListAsync();
            }
            return await _repository.GetAllAsync();
        }

        public Task<TaskItem?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

        public async Task<IEnumerable<TaskItem>> GetByColumnAsync(Guid columnId)
        {
            if (_repository is GenericRepository<TaskItem> repo)
            {
                return await repo.Query()
                    .Where(t => t.ColumnId == columnId)
                    .OrderBy(t => t.Position)
                    .ToListAsync();
            }
            return Enumerable.Empty<TaskItem>();
        }

        public async Task<TaskItem> CreateAsync(TaskItem taskItem)
        {
            try
            {
                // Verify the user owns the board
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (_repository is GenericRepository<TaskItem> repo)
                {
                    // Include the User navigation property
                    var board = await repo.Context.Boards
                        .Include(b => b.User)
                        .FirstOrDefaultAsync(b => b.Id == taskItem.BoardId);

                    if (board?.ApplicationUserId != userId)
                    {
                        throw new UnauthorizedAccessException("You do not own this board.");
                    }

                    taskItem.Id = Guid.NewGuid();
                    taskItem.CreatedAt = DateTime.UtcNow;

                    var maxPosition = await repo.Query().Where(t => t.ColumnId == taskItem.ColumnId).MaxAsync(t => (int?)t.Position) ?? -1;
                    taskItem.Position = maxPosition + 1;

                    var createdTask = await _repository.CreateAsync(taskItem);

                    // Send email to board owner
                    if (board?.User?.Email is string userEmail)
                    {
                        var subject = $"New Task Created: {taskItem.Title}";
                        var body = $"A new task '{taskItem.Title}' was created in your board: '{board.Name}'.";
                        await _emailService.SendEmailAsync(userEmail, subject, body);
                    }

                    return createdTask;
                }
                return await _repository.CreateAsync(taskItem);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating task item", ex);
            }
        }

        public Task<bool> UpdateAsync(TaskItem taskItem) => _repository.UpdateAsync(taskItem);

        public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    }
}