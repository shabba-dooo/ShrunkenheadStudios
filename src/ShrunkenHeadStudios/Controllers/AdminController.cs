using System;
using System.Web.Mvc;
using System.Web.Security;
using ShrunkenHeadStudios.Core;
using ShrunkenHeadStudios.Core.Objects;
using ShrunkenHeadStudios.Models;
using ShrunkenHeadStudios.Providers;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace ShrunkenHeadStudios.Controllers
{
    //Admin controller that contains actions to return Json for jquery grid and add/edit/delete functions.
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IAuthProvider _authProvider;
        private readonly IBlogRepository _blogRepository;

        public AdminController(IAuthProvider authProvider, IBlogRepository blogRepository = null)
        {
            _authProvider = authProvider;
            _blogRepository = blogRepository;
        }

        // Return the login page.
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (_authProvider.IsLoggedIn)
                return RedirectToUrl(returnUrl);

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        //Login POST action.
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && _authProvider.Login(model.UserName, model.Password))
            {
                return RedirectToUrl(returnUrl);
            }

            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        // Return page to manage posts, categories and tags.
        public ActionResult Manage()
        {
            return View();
        }

        // Logout the user and return the login page.
        public ActionResult Logout()
        {
            _authProvider.Logout();

            return RedirectToAction("Login", "Admin");
        }

        private ActionResult RedirectToUrl(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Manage");
            }
        }

        //Populate Posts Grid
        public ContentResult Posts(JqInViewModel jqParams)
        {
            var posts = _blogRepository.Posts(jqParams.page - 1, jqParams.rows, jqParams.sidx, jqParams.sord == "asc");

            //Total count of all published & unpublished posts.
            var totalPosts = _blogRepository.TotalPosts(false);

            //Data needs to be returned in Json to bind to grid.
            return Content(JsonConvert.SerializeObject(new { page = jqParams.page, records = totalPosts, rows = posts, total = Math.Ceiling(Convert.ToDouble(totalPosts) / jqParams.rows)}, new CustomDateTimeConverter()), "application/json");
        }

        //Handle add post request
        [HttpPost, ValidateInput(false)]
        public ContentResult AddPost(Post post)
        {
            string json;

            ModelState.Clear();

            //Only save if no validation errors
            if (TryValidateModel(post))
            {
                var id = _blogRepository.AddPost(post);
                json = JsonConvert.SerializeObject(new { id = id, success = true, message = "Post added sucessfully!" });
            }
            else
            {
                json = JsonConvert.SerializeObject(new { id = 0, success = false, message = "Failed to add post!" });
            }
            return Content(json, "application/json");
        }

        //Handle edit post request
        [HttpPost, ValidateInput(false)]
        public ContentResult EditPost(Post post)
        {
            string json;

            ModelState.Clear();

            //Only save if no validation errors
            if (TryValidateModel(post))
            {
                _blogRepository.EditPost(post);
                json = JsonConvert.SerializeObject(new { id = post.ID, success = true, message = "Changes saved sucessfully!" });
            }
            else
            {
                json = JsonConvert.SerializeObject(new { id = 0, success = false, message = "Failed to save changes!" });
            }
            return Content(json, "application/json");
        }

        //Handle delete post request
        [HttpPost]
        public ContentResult DeletePost(int id)
        {
            _blogRepository.DeletePost(id);
            var json = JsonConvert.SerializeObject(new { id = 0, success = true, message = "Post deleted sucessfully!" });
            return Content(json, "application/json");
        }

        //Handle get categories request - Return all the categories
        public ContentResult GetCategoriesHtml()
        {
            var categories = _blogRepository.Categories().OrderBy(s => s.Name);

            var sb = new StringBuilder();
            sb.AppendLine(@"<select>");

            foreach (var category in categories)
            {
                sb.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>",
                    category.ID, category.Name));
            }

            sb.AppendLine("<select>");
            return Content(sb.ToString(), "text/html");
        }

        //Populate categories grid
        public ContentResult Categories()
        {
            var categories = _blogRepository.Categories();
            return Content(JsonConvert.SerializeObject(new { page = 1, records = categories.Count, rows = categories, total = 1 }), "application/json");
        }

        //Handle add category request
        //Excluding ID as this can cause issues with jQGrid
        [HttpPost]
        public ContentResult AddCategory([Bind(Exclude = "ID")]Category category)
        {
            string json;

            if (ModelState.IsValid)
            {
                var id = _blogRepository.AddCategory(category);
                json = JsonConvert.SerializeObject(new { id = id, success = true, message = "Category added successfully!" });
            }
            else
            {
                json = JsonConvert.SerializeObject(new { id = 0, success = false, message = "Failed to add the category!" });
            }
            return Content(json, "application/json");
        }

        //Handle edit category request
        [HttpPost]
        public ContentResult EditCategory(Category category)
        {
            string json;

            if (ModelState.IsValid)
            {
                _blogRepository.EditCategory(category);
                json = JsonConvert.SerializeObject(new { id = category.ID, success = true, message = "Changes saved sucessfully!" });
            }
            else
            {
                json = JsonConvert.SerializeObject(new { id = 0, success = false, message = "Failed to save changes!" });
            }
            return Content(json, "application/json");
        }

        //Handle delete category request
        [HttpPost]
        public ContentResult DeleteCategory(int id)
        {
            _blogRepository.DeleteCategory(id);
            var json = JsonConvert.SerializeObject(new { id = 0, success = true, message = "Category deleted sucessfully!" });
            return Content(json, "application/json");
        }

        //Handle get tags request - Return all the tags
        public ContentResult GetTagsHtml()
        {
            var tags = _blogRepository.Tags().OrderBy(s => s.Name);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<select multiple=""multiple"">");

            foreach (var tag in tags)
            {
                sb.AppendLine(string.Format(@"<option value=""{0}"">{1}</option>",
                    tag.ID, tag.Name));
            }

            sb.AppendLine("<select>");
            return Content(sb.ToString(), "text/html");
        }

        //Populate tags grid
        public ContentResult Tags()
        {
            var tags = _blogRepository.Tags();
            return Content(JsonConvert.SerializeObject(new { page = 1, records = tags.Count, rows = tags, total = 1 }), "application/json");
        }

        //Handle add tag request
        //Excluding ID as this can cause issues with jQGrid
        [HttpPost]
        public ContentResult AddTag([Bind(Exclude = "ID")]Tag tag)
        {
            string json;

            if (ModelState.IsValid)
            {
                var id = _blogRepository.AddTag(tag);
                json = JsonConvert.SerializeObject(new { id = id, success = true, message = "Tag added successfully!" });
            }
            else
            {
                json = JsonConvert.SerializeObject(new { id = 0, success = false, message = "Failed to add the tag!" });
            }
            return Content(json, "application/json");
        }

        //Handle edit tag request
        [HttpPost]
        public ContentResult EditTag(Tag tag)
        {
            string json;

            if (ModelState.IsValid)
            {
                _blogRepository.EditTag(tag);
                json = JsonConvert.SerializeObject(new { id = tag.ID, success = true, message = "Changes saved sucessfully!" });
            }
            else
            {
                json = JsonConvert.SerializeObject(new { id = 0, success = false, message = "Failed to save changes!" });
            }
            return Content(json, "application/json");
        }

        //Handle delete tag request
        [HttpPost]
        public ContentResult DeleteTag(int id)
        {
            _blogRepository.DeleteTag(id);
            var json = JsonConvert.SerializeObject(new { id = 0, success = true, message = "Tag deleted sucessfully!" });
            return Content(json, "application/json");
        }
    }
}
