using AutoMapper;
using AutoMapper.Features;
using Habr.Common.DTOs.Requests;
using Habr.Common.DTOs.Views;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.MapProfiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, DraftListItem>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => s.UpdatedAt));

        CreateMap<Post, PostListItem>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.PublicationDate, opt => opt.MapFrom(s => s.PublicationDate))
            .ForMember(d => d.AuthorEmail, opt => opt.MapFrom(s => s.Creator!.Email));

        CreateMap<Post, PostViewModel>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => s.UpdatedAt))
            .ForMember(d => d.CreatorId, opt => opt.MapFrom(s => s.CreatorId))
            .ForMember(d => d.CreatorEmail, opt => opt.MapFrom(s => s.Creator!.Email))
            .ForMember(d => d.CreatorName, opt => opt.MapFrom(s => s.Creator!.Name))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.IsPublished, opt => opt.MapFrom(s => s.IsPublished))
            .ForMember(d => d.PublicationDate, opt => opt.MapFrom(s => s.PublicationDate));

        CreateMap<Post, PublishedPostViewModel>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text))
            .ForMember(d => d.AuthorsEmail, opt => opt.MapFrom(s => s.Creator!.Email))
            .ForMember(d => d.PublishDate, opt => opt.MapFrom(s => s.PublicationDate))
            .ForMember(d => d.Comments, opt => opt.MapFrom(s =>
                s.Comments.SelectMany(c => new[] { c }.Concat(c.Comments))));

        CreateMap<AddPostRequestModel, Post>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text))
            .ForMember(d => d.IsPublished, opt => opt.MapFrom(s => s.IsPublished));

        CreateMap<UpdatePostRequestModel, Post>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text))
            .ForMember(d => d.IsPublished, opt => opt.MapFrom(s => s.IsPublished));

        CreateMap<Post, UpdatePostRequestModel>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Text, opt => opt.MapFrom(s => s.Text))
            .ForMember(d => d.IsPublished, opt => opt.MapFrom(s => s.IsPublished));

    }
}
