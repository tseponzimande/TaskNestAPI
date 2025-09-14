using AutoMapper;
using TaskNest.Domain.Entities;
using TaskNest.Web.DTOs;

namespace TaskNest.Web.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Board, BoardDto>().ReverseMap();
            CreateMap<TaskItem, TaskItemDto>().ReverseMap();
            CreateMap<BoardColumn, BoardColumnDto>().ReverseMap();


        }
    }
}
