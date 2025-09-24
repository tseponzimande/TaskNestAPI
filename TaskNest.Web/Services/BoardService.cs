namespace TaskNest.Web.Services
{
    public class BoardService(IGenericRepository<Board> repository, IHttpContextAccessor _httpContextAccessor) : IBoardService
    {
       private readonly IGenericRepository<Board> _repository = repository;

        public async Task<IEnumerable<Board>> GetAllAsync()
        {
            if (_repository is GenericRepository<Board> repo)
            {
                return await repo.Query().Include(b => b.Tasks).ToListAsync();
            }
            return await _repository.GetAllAsync();
        }

        public async Task<Board?> GetByIdAsync(Guid id)
        {
            // If your GenericRepository exposes Query(), use it to include navigation props
            if (_repository is GenericRepository<Board> repo)
            {
                return await repo.Query()
                                 .Include(b => b.BoardColumns)
                                 .Include(b => b.Tasks)
                                 .FirstOrDefaultAsync(b => b.Id == id);
            }

            // fallback - repository's GetByIdAsync (no includes)
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Board> CreateAsync(Board board)
        {
            // Set the current user as the owner of the board
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                board.ApplicationUserId = userId;
            }
            return await _repository.CreateAsync(board);
        }


        //public Task<Board> CreateAsync(Board board) => _repository.CreateAsync(board);

        public Task<bool> UpdateAsync(Board board) => _repository.UpdateAsync(board);

        public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    }
}
