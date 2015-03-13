using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShrunkenHeadStudios.Core;
using ShrunkenHeadStudios.Core.Objects;
using ShrunkenHeadStudios.Models;

namespace ShrunkenHeadStudios.Controllers
{
    //Home controller that contains actions to return list/view pages and others.
    public class BlogController: Controller
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public ViewResult Index()
        {
            //return the latest post
            var LatestPost = _blogRepository.LatestPost();
            return View("Home", LatestPost);
        }

        public ViewResult Posts(int p = 1)
        {
            //return the list page with latest posts.
            //P = Pagination Number

            var viewModel = new ListViewModel(_blogRepository, p);
            ViewBag.Title = "Latest Posts";
            ViewBag.Description = "Latest blog posts";
            return View("List", viewModel);
        }

        //Return collection of posts belong to a category
        public ViewResult Category(string category, int p = 1)
        {
            var viewModel = new ListViewModel(_blogRepository, category, "Category", p);

            if (viewModel.Category == null)
                throw new HttpException(404, "Category not found");

            ViewBag.Title = String.Format(@"Displaying latest posts for category ""{0}""", viewModel.Category.Name);
            ViewBag.Description = String.Format(@"Latest posts for category: {0}", viewModel.Category.Name);
            return View("List", viewModel);
        }

        //Return collection of posts belong to a tag
        public ViewResult Tag(string tag, int p = 1)
        {
            var viewModel = new ListViewModel(_blogRepository, tag, "Tag", p);

            if(viewModel.Tag == null)
                throw new HttpException(404, "Tag not found");

            ViewBag.Title = String.Format(@"Displaying latest posts for tag ""{0}""", viewModel.Tag.Name);
            ViewBag.Description = String.Format(@"Latest posts for tag: {0}", viewModel.Tag.Name);
            return View("List", viewModel);
        }

        //Return a single post related to month,year and url slug
        public ViewResult Post(int year, int month, string title)
        {
            var post = _blogRepository.Post(year, month, title);

            if (post == null)
                throw new HttpException(404, "Post not found");

            if (post.Published == false && User.Identity.IsAuthenticated == false)
                throw new HttpException(401, "This post is not published");

            return View("Post", post);
        }
    }
}
