using AutoMapper;
using Neighbor.Contract.DTOs.FeedbackDTOs;
using Neighbor.Domain.Entities;

namespace Neighbor.Application.Mapper;

public class FeedbackProfile : Profile
{
    public FeedbackProfile()
    {
        CreateMap<Feedback, FeedbackDTO>();
    }
}
