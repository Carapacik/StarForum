using System.Globalization;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;
using StarForum.WebApi.Models.Forum;
using StarForum.WebApi.Models.Post;

namespace StarForum.WebApi.Controllers;

public class ForumController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IForum _forumService;
    private readonly IPost _postService;
    private readonly IUpload _uploadService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ForumController(IForum forumService, IPost postService, IUpload uploadService,
        IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        _forumService = forumService;
        _postService = postService;
        _uploadService = uploadService;
        _configuration = configuration;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var forums = _forumService.GetAll()
            .Select(async forum => new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                Description = forum.Description,
                NumberOfPosts = forum.Posts.Count,
                NumberOfUsers = (await _forumService.GetActiveUsers(forum.Id)).Count(),
                ImageUrl = forum.ImageUrl,
                HasRecentPosts = await _forumService.HasRecentPost(forum.Id)
            }).Select(r => r.Result);

        var model = new ForumIndexModel
        {
            ForumList = forums.OrderBy(f => f.Name)
        };
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        var model = new AddForumModel();
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateForum(AddForumModel model)
    {
        var imageUri = "/images/theme/defaultForum.png";
        if (model.ImageUpload != null)
            //Azure
            //var blockBlob = UploadForumImageForAzure(model.ImageUpload);
            //imageUri = blockBlob.Uri.AbsoluteUri;  
            imageUri = UploadForumImage(model.ImageUpload);

        var forum = new Forum
        {
            Title = model.Title,
            Description = model.Description,
            Created = DateTime.Now,
            ImageUrl = imageUri
        };

        await _forumService.Create(forum);
        return RedirectToAction("Index", "Forum");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var forum = await _forumService.GetById(id);
        if (forum == null) return RedirectToAction("Error", "Home");

        var model = new EditForumModel
        {
            Id = id,
            Description = forum.Description,
            Title = forum.Title
        };
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditForum(EditForumModel model)
    {
        var imageUri = "";

        if (model.ImageUpload != null)
            //Azure
            //var blockBlob = UploadForumImageForAzure(model.ImageUpload);
            //imageUri = blockBlob.Uri.AbsoluteUri;
            imageUri = UploadForumImage(model.ImageUpload);

        var forum = new Forum
        {
            Id = model.Id,
            Title = model.Title,
            Description = model.Description,
            ImageUrl = imageUri
        };

        await _forumService.Edit(forum);
        return RedirectToAction("Index", "Forum");
    }

    public async Task<IActionResult> Topic(int id, string searchQuery)
    {
        var forum = await _forumService.GetById(id);
        if (forum == null) return RedirectToAction("Error", "Home");
        var posts = _postService.GetFilteredPosts(forum, searchQuery).ToList();
        var emptySearchResults = posts.Count == 0 && string.IsNullOrEmpty(searchQuery);
        var postListings = posts.Select(post => new PostListingModel
        {
            Id = post.Id,
            AuthorId = post.User.Id,
            AuthorRating = post.User.Rating,
            Author = post.User.UserName,
            Title = post.Title,
            DatePosted = post.Created.ToString(CultureInfo.InvariantCulture),
            RepliesCount = post.Replies.Count,
            Forum = BuildForumListing(post),
            IsPostArchived = post.IsArchived
        });

        var model = new ForumTopicModel
        {
            Posts = postListings,
            Forum = BuildForumListing(forum),
            EmptySearchResults = emptySearchResults,
            SearchQuery = searchQuery
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult Search(int id, string searchQuery)
    {
        return RedirectToAction("Topic", new {id, searchQuery});
    }

    private static ForumListingModel BuildForumListing(Post post)
    {
        var forum = post.Forum;
        return BuildForumListing(forum);
    }

    private static ForumListingModel BuildForumListing(Forum forum)
    {
        return new ForumListingModel
        {
            Id = forum.Id,
            Name = forum.Title,
            Description = forum.Description,
            ImageUrl = forum.ImageUrl
        };
    }

    private CloudBlockBlob UploadForumImageForAzure(IFormFile file)
    {
        if (file.Length > 4 * 1024 * 1024 && !file.ContentType.Contains("image"))
            return null;
        var connectionString = _configuration.GetConnectionString("AzureStorageAccount");
        var container = _uploadService.GetBlobContainer(connectionString, "forum-images");
        var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
        if (contentDisposition.FileName == null) return null;
        var blockBlob =
            container.GetBlockBlobReference(Guid.NewGuid() +
                                            Path.GetExtension(contentDisposition.FileName.Trim('"')));
        blockBlob.UploadFromStreamAsync(file.OpenReadStream()).Wait();

        return blockBlob;
    }

    private string UploadForumImage(IFormFile file)
    {
        if (file.Length > 4 * 1024 * 1024 && !file.ContentType.Contains("image")) return null;
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
        if (contentDisposition.FileName == null) return null;
        var filename = contentDisposition.FileName.Trim('"');
        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(filename);
        var path = Path.Combine(wwwRootPath + "/images/forum-images/", uniqueFileName);
        using var fileStream = new FileStream(path, FileMode.Create);
        file.CopyTo(fileStream);

        return "/images/forum-images/" + uniqueFileName;
    }
}