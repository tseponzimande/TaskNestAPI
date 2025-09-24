using AutoMapper;

namespace TaskNest.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Board, BoardDto>().ReverseMap();
            CreateMap<BoardColumn, BoardColumnDto>().ReverseMap(); // names now match
            CreateMap<TaskItem, TaskItemDto>().ReverseMap();
            // other mappings...
        }
    }
}
