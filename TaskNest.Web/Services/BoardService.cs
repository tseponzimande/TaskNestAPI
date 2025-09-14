namespace TaskNest.Web.Services
{
    public class BoardService(IGenericRepository<Board> repository) : IBoardService
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

        public Task<Board?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

        public Task<Board> CreateAsync(Board board) => _repository.CreateAsync(board);

        public Task<bool> UpdateAsync(Board board) => _repository.UpdateAsync(board);

        public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    }
}
