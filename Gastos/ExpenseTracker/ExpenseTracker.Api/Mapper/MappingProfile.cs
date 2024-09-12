using AutoMapper;
using ExpenseTracker.Api.Dto;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, FullUserDto>();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<BaseCategoryDto, Category>().ReverseMap();
            CreateMap<BaseExpenseDto, Expense>().ReverseMap();

            CreateMap<Expense, ExpenseDto>().ReverseMap();
        }
    }
}
