using Habr.Common.DTOs.Requests;
using Habr.Common.Exceptions;
using Habr.Common.Enums;
using Habr.DataAccess.Repositories.Interfaces;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Interfaces.UserFeatures;
using Habr.BusinessLogic.Implementations.PostFeatures;
using AutoMapper;
using Moq;
using Habr.Common.Resources;

namespace Habr.Tests;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUserManager> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PostService _postService;
    private readonly Dictionary<string, string> exceptionMessageDictionary;

    public PostServiceTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _userManagerMock = new Mock<IUserManager>();
        _mapperMock = new Mock<IMapper>();
        _postService = new PostService(_postRepositoryMock.Object, _userManagerMock.Object, _mapperMock.Object);

        exceptionMessageDictionary = new Dictionary<string, string>()
        {
            { nameof(ExceptionMessageResource.PublishedPostDeletion), ExceptionMessageResource.PublishedPostDeletion },
            { nameof(ExceptionMessageResource.NotFoundDefaultMessage), ExceptionMessageResource.NotFoundDefaultMessage },
            { nameof(ExceptionMessageResource.AccessDeniedMessage), ExceptionMessageResource.AccessDeniedMessage },
            { nameof(ExceptionMessageResource.PostWithCommentsMoving), ExceptionMessageResource.PostWithCommentsMoving },
            { nameof(ExceptionMessageResource.PublishedPostUpdate), ExceptionMessageResource.PublishedPostUpdate },
        };
    }

    [Fact]
    public async Task AddAsync_Post_ShouldCallRepositoryAddAsync()
    {
        // Arrange
        var model = new AddPostRequestModel();
        int? postId = 1;
        Post? post = GeneratePost(ref postId, 1, true);
        _mapperMock.Setup(m => m.Map(
            It.IsAny<AddPostRequestModel>(),
            It.IsAny<Action<IMappingOperationOptions<AddPostRequestModel, Post>>>())
        )
        .Returns(post!);

        _postRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Post>()))
                           .ReturnsAsync(1);

        // Act
        var result = await _postService.AddAsync(model);

        // Assert
        _mapperMock.Verify(m => m.Map(
            It.IsAny<AddPostRequestModel>(),
            It.IsAny<Action<IMappingOperationOptions<AddPostRequestModel, Post>>>()),
            Times.Once);
        _postRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Post>()), Times.Once);
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task DeleteByIdAsync_ProperData_SuccessfullDeletion()
    {
        // Arrange
        var postId = 1;
        var creatorId = 1;
        var post = new Post { Id = postId, IsPublished = false, CreatorId = creatorId };

        _userManagerMock.Setup(manager => manager.UserId).Returns(creatorId);
        _postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId, GetPostOptions.None))
                           .ReturnsAsync(post);

        // Act
        await _postService.DeleteByIdAsync(postId);

        // Assert
        _userManagerMock.Verify(manager => manager.UserId, Times.AtLeastOnce);
        _postRepositoryMock.Verify(repo => repo.DeleteByIdAsync(postId), Times.Once);
    }

    [Theory]
    [InlineData(1, 1, true, 1, nameof(ExceptionMessageResource.PublishedPostDeletion))]
    [InlineData(null, 1, false, 1, nameof(ExceptionMessageResource.NotFoundDefaultMessage))]
    [InlineData(1, 1, false, 2, nameof(ExceptionMessageResource.AccessDeniedMessage))]
    public async Task DeleteByIdAsync_NotProperData_ThrowDomainException(
        int? postId,
        int creatorId,
        bool isPublished,
        int userId,
        string expectedExceptionMessageName)
    {
        var expectedExceptionMessage = exceptionMessageDictionary[expectedExceptionMessageName];
        Post? post = GeneratePost(ref postId, creatorId, isPublished);

        _userManagerMock.Setup(manager => manager.UserId).Returns(userId);
        _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), GetPostOptions.None)).ReturnsAsync(post);

        //Act & Assert
        try
        {
            await _postService.DeleteByIdAsync(postId!.Value);
        }
        catch (DomainException e)
        {
            Assert.Equal(expectedExceptionMessage, e.ProblemDetails.Detail);
        }

        _postRepositoryMock.Verify(repo => repo.DeleteByIdAsync(postId!.Value), Times.Never);
    }

    [Fact]
    public async Task MoveToDraftsAsync_ProperData_SuccessfullUpdate()
    {
        // Arrange
        int? postId = 1;
        var userId = 1;
        Post? post = GeneratePost(ref postId, userId, true);

        _userManagerMock.Setup(manager => manager.UserId).Returns(userId);
        _postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId!.Value, GetPostOptions.IncludeComments)).ReturnsAsync(post);

        // Act
        await _postService.MoveToDraftsAsync(postId!.Value);

        // Assert
        _postRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Post>()), Times.Once);
        Assert.False(post!.IsPublished);
    }

    [Theory]
    [InlineData(null, 1, false, false, 1, nameof(ExceptionMessageResource.NotFoundDefaultMessage))]
    [InlineData(1, 1, false, false, 2, nameof(ExceptionMessageResource.AccessDeniedMessage))]
    [InlineData(1, 1, false, true, 1, nameof(ExceptionMessageResource.PostWithCommentsMoving))]
    public async Task MoveToDraftsAsync_NotProperData_ThrowsDomainException(
        int? postId,
        int creatorId,
        bool isPublished,
        bool hasComments,
        int userId,
        string expectedExceptionMessageName)
    {
        // Arrange
        var expectedExceptionMessage = exceptionMessageDictionary[expectedExceptionMessageName];
        Post? post = GeneratePost(ref postId, creatorId, isPublished, hasComments);

        _postRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>(), GetPostOptions.IncludeComments))
            .ReturnsAsync(post);
        _userManagerMock.Setup(manager => manager.UserId).Returns(userId);

        // Act & Assert
        try
        {
            await _postService.MoveToDraftsAsync(postId!.Value);
        }
        catch (DomainException e)
        {
            Assert.Equal(expectedExceptionMessage, e.ProblemDetails.Detail);
        }

        _postRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Post>()), Times.Never);
    }

    [Fact]
    public async Task PublishPostAsync_ProperData_SuccessfullUpdate()
    {
        // Arrange
        int? postId = 1;
        int userId = 1;
        var post = GeneratePost(ref postId, userId, false);
        var updateModel = new UpdatePostRequestModel { Id = postId!.Value, IsPublished = true };

        _userManagerMock.Setup(manager => manager.UserId).Returns(userId);
        _postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId.Value, It.IsAny<GetPostOptions>())).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map(It.IsAny<Post>(), It.IsAny<Action<IMappingOperationOptions<Post, UpdatePostRequestModel>>>()))
                   .Returns(updateModel);

        // Act
        await _postService.PublishPostAsync(postId.Value);

        // Assert
        _postRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }

    [Theory]
    [InlineData(null, 1, false, 1, nameof(ExceptionMessageResource.NotFoundDefaultMessage))]
    [InlineData(1, 1, false, 2, nameof(ExceptionMessageResource.AccessDeniedMessage))]
    [InlineData(1, 1, true, 1, nameof(ExceptionMessageResource.PublishedPostUpdate))]
    public async Task PublishPostAsync_NotProperData_ThrowsDomainException(
        int? postId,
        int creatorId,
        bool isPublished,
        int userId,
        string expectedExceptionMessageName)
    {
        // Arrange
        var post = GeneratePost(ref postId, creatorId, isPublished);
        var updateModel = new UpdatePostRequestModel { Id = postId!.Value, IsPublished = isPublished };
        var expectedExceptionMessage = exceptionMessageDictionary[expectedExceptionMessageName];

        _userManagerMock.Setup(manager => manager.UserId).Returns(userId);
        _postRepositoryMock.Setup(repo => repo.GetByIdAsync(postId.Value, It.IsAny<GetPostOptions>())).ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map(It.IsAny<Post>(), It.IsAny<Action<IMappingOperationOptions<Post, UpdatePostRequestModel>>>()))
                   .Returns(updateModel);

        //Act & Assert
        try
        {
            await _postService.PublishPostAsync(postId.Value);
        }
        catch (DomainException e)
        {
            Assert.Equal(expectedExceptionMessage, e.ProblemDetails.Detail);
        }

        _postRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Post>()), Times.Never);
    }

    private static Post? GeneratePost(
        ref int? postId,
        int creatorId,
        bool isPublished,
        bool withComments = false)
    {
        Post? post;

        if (postId is null)
        {
            post = null;
            postId = 1;
        }
        else
        {
            post = new Post
            {
                Id = postId.Value,
                IsPublished = isPublished,
                CreatorId = creatorId,
                Comments = withComments ? [new()] : new List<Comment>()
            };
        }

        return post;
    }
}
