using AutoMapper;
using Habr.Common.DTOs.Requests;
using Habr.Common.DTOs.Views;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.MapProfiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentViewModel>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.ParrentId, opt => opt.MapFrom(s => s.ParrentId))
            .ForMember(d => d.IsPostReply, opt => opt.MapFrom(s => s.IsPostReply))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => s.UpdatedAt))
            .ForMember(d => d.CreatorId, opt => opt.MapFrom(s => s.CreatorId))
            .ForMember(d => d.CreatorEmail, opt => opt.MapFrom(s => s.Creator!.Email))
            .ForMember(d => d.CreatorName, opt => opt.MapFrom(s => s.Creator!.Name));

        CreateMap<WriteCommentRequestModel, Comment>()
            .ForMember(d => d.ParrentId, opt => opt.MapFrom(s => s.DeclarationId))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text));

    }
}
