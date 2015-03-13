using ShrunkenHeadStudios.Core.Objects;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Linq;

namespace ShrunkenHeadStudios.Core
{
    public class BlogRepository : IBlogRepository
    {
        /// <summary>
        /// NHibernate's session object required for all db actions.
        /// </summary>
        private readonly ISession _session;

        public BlogRepository(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Return the latest published post.
        /// </summary>
        public Post LatestPost()
        {
            var query = _session.Query<Post>()
                .OrderByDescending(p => p.ID)
                .Fetch(p => p.Category);

            query.FetchMany(p => p.Tags).ToFuture();

            return query.ToFuture().FirstOrDefault();
        }

        /// <summary>
        /// Return collection of posts based on pagination parameters.
        /// </summary>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> Posts(int pageNo, int pageSize)
        {
            var posts = _session.Query<Post>()
                .Where(p => p.Published)
                .OrderByDescending(p => p.PostedOn)
                .Skip(pageNo * pageSize)
                .Take(pageSize)
                .Fetch(p => p.Category)
                .ToList();

            var postIds = posts.Select(p => p.ID).ToList();

            return _session.Query<Post>()
                .Where(p => postIds.Contains(p.ID))
                .OrderByDescending(p => p.PostedOn)
                .FetchMany(p => p.Tags)
                .ToList();
        }

        /// <summary>
        /// Return total no. of all or published posts.
        /// </summary>
        public int TotalPosts(bool checkIsPublished = true)
        {
            return _session.Query<Post>().Where(p => checkIsPublished || p.Published == true).Count();
        }

        /// <summary>
        /// Return collection of posts belongs to a particular category.
        /// </summary>
        /// <param name="categorySlug">Category's url slug</param>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> PostsForCategory(string categorySlug, int PageNo, int PageSize)
        {
            var posts = _session.Query<Post>()
                .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                .OrderByDescending(p => p.PostedOn)
                .Skip(PageNo * PageSize)
                .Take(PageSize)
                .Fetch(p => p.Category)
                .ToList();

            var postIds = posts.Select(p => p.ID).ToList();

            return _session.Query<Post>()
                .Where(p => postIds.Contains(p.ID))
                .OrderByDescending(p => p.PostedOn)
                .FetchMany(p => p.Tags)
                .ToList();
        }

        /// <summary>
        /// Return total no. of posts belongs to a particular category.
        /// </summary>
        /// <param name="categorySlug">Category's url slug</param>
        /// <returns></returns>
        public int TotalPostsForCategory(string categorySlug)
        {
            return _session.Query<Post>()
                .Where(p => p.Published && p.Category.UrlSlug.Equals(categorySlug))
                .Count();
        }

        /// <summary>
        /// Return category based on url slug.
        /// </summary>
        /// <param name="categorySlug">Category's url slug</param>
        /// <returns></returns>
        public Category Category(string categorySlug)
        {
            return _session.Query<Category>().FirstOrDefault(t => t.UrlSlug.Equals(categorySlug));
        }

        /// <summary>
        /// Return collection of posts belongs to a particular tag.
        /// </summary>
        /// <param name="tagSlug">Tag's url slug</param>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IList<Post> PostsForTag(string tagSlug, int PageNo, int PageSize)
        {
            var posts = _session.Query<Post>()
            .Where(p => p.Published && p.Tags.Any(t => t.UrlSlug.Equals(tagSlug)))
            .OrderByDescending(p => p.PostedOn)
            .Skip(PageNo * PageSize)
            .Take(PageSize)
            .Fetch(p => p.Category)
            .ToList();

            var postIds = posts.Select(p => p.ID).ToList();

            return _session.Query<Post>()
                .Where(p => postIds.Contains(p.ID))
                .OrderByDescending(p => p.PostedOn)
                .FetchMany(p => p.Tags)
                .ToList();
        }

        /// <summary>
        /// Return total no. of posts belongs to a particular tag.
        /// </summary>
        /// <param name="tagSlug">Tag's url slug</param>
        /// <returns></returns>
        public int TotalPostsForTag(string tagSlug)
        {
            return _session.Query<Post>()
                .Where(p => p.Published && p.Tags.Any(t => t.UrlSlug.Equals(tagSlug)))
                .Count();
        }

        /// <summary>
        /// Return tag based on url slug.
        /// </summary>
        /// <param name="tagSlug"></param>
        /// <returns></returns>
        public Tag Tag(string tagSlug)
        {
            return _session.Query<Tag>().FirstOrDefault(t => t.UrlSlug.Equals(tagSlug));
        }

        /// <summary>
        /// Return post based on the published year, month and title slug.
        /// </summary>
        /// <param name="year">Published year</param>
        /// <param name="month">Published month</param>
        /// <param name="titleSlug">Post's url slug</param>
        /// <returns></returns>
        public Post Post(int year, int month, string titleSlug)
        {
            var query = _session.Query<Post>()
                .Where(p => p.PostedOn.Year == year && p.PostedOn.Month == month && p.UrlSlug.Equals(titleSlug))
                .Fetch(P => P.Category);

            query.FetchMany(p => p.Tags).ToFuture();

            return query.ToFuture().Single();
        }

        /// <summary>
        /// Return posts based on pagination and sorting parameters - USED BY ADMIN PANEL
        /// Switch statement used to perform sorting for different properties, dependant on grid selection. TODO: Think of better solution.
        /// </summary>
        /// <param name="pageNo">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sortColumn">Sort column name</param>
        /// <param name="sortByAscending">True to sort by ascending</param>
        /// <returns></returns>
        public IList<Post> Posts(int pageNo, int pageSize, string sortColumn, bool sortByAscending)
        {
            IList<Post> posts;
            IList<int> postIds;

            switch (sortColumn)
            {
                case "Title":
                    if (sortByAscending)
                    {
                        posts = _session.Query<Post>()
                                        .OrderBy(p => p.Title)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderBy(p => p.Title)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    else
                    {
                        posts = _session.Query<Post>()
                                        .OrderByDescending(p => p.Title)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderByDescending(p => p.Title)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    break;
                case "Published":
                    if (sortByAscending)
                    {
                        posts = _session.Query<Post>()
                                        .OrderBy(p => p.Published)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderBy(p => p.Published)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    else
                    {
                        posts = _session.Query<Post>()
                                        .OrderByDescending(p => p.Published)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderByDescending(p => p.Published)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    break;
                case "PostedOn":
                    if (sortByAscending)
                    {
                        posts = _session.Query<Post>()
                                        .OrderBy(p => p.PostedOn)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderBy(p => p.PostedOn)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    else
                    {
                        posts = _session.Query<Post>()
                                        .OrderByDescending(p => p.PostedOn)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                        .Where(p => postIds.Contains(p.ID))
                                        .OrderByDescending(p => p.PostedOn)
                                        .FetchMany(p => p.Tags)
                                        .ToList();
                    }
                    break;
                case "Modified":
                    if (sortByAscending)
                    {
                        posts = _session.Query<Post>()
                                        .OrderBy(p => p.Modified)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderBy(p => p.Modified)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    else
                    {
                        posts = _session.Query<Post>()
                                        .OrderByDescending(p => p.Modified)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderByDescending(p => p.Modified)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    break;
                case "Category":
                    if (sortByAscending)
                    {
                        posts = _session.Query<Post>()
                                        .OrderBy(p => p.Category.Name)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderBy(p => p.Category.Name)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    else
                    {
                        posts = _session.Query<Post>()
                                        .OrderByDescending(p => p.Category.Name)
                                        .Skip(pageNo * pageSize)
                                        .Take(pageSize)
                                        .Fetch(p => p.Category)
                                        .ToList();

                        postIds = posts.Select(p => p.ID).ToList();

                        posts = _session.Query<Post>()
                                         .Where(p => postIds.Contains(p.ID))
                                         .OrderByDescending(p => p.Category.Name)
                                         .FetchMany(p => p.Tags)
                                         .ToList();
                    }
                    break;
                default:
                    posts = _session.Query<Post>()
                                    .OrderByDescending(p => p.PostedOn)
                                    .Skip(pageNo * pageSize)
                                    .Take(pageSize)
                                    .Fetch(p => p.Category)
                                    .ToList();

                    postIds = posts.Select(p => p.ID).ToList();

                    posts = _session.Query<Post>()
                                     .Where(p => postIds.Contains(p.ID))
                                     .OrderByDescending(p => p.PostedOn)
                                     .FetchMany(p => p.Tags)
                                     .ToList();
                    break;
            }

            return posts;
        }

        ///<summary>
        ///ADMIN CONTROLS
        /// </summary>

        /// <summary>
        /// Adds a new post and returns the id.
        /// </summary>
        /// <param name="post"></param>
        /// <returns>Newly added post id</returns>
        public int AddPost(Post post)
        {
            using (var tran = _session.BeginTransaction())
            {
                _session.Save(post);
                tran.Commit();
                return post.ID;
            }
        }

        /// <summary>
        /// Update an existing post.
        /// </summary>
        /// <param name="post"></param>
        public void EditPost(Post post)
        {
            using (var tran = _session.BeginTransaction())
            {
                _session.SaveOrUpdate(post);
                tran.Commit();
            }
        }

        /// <summary>
        /// Delete the post permanently from database.
        /// </summary>
        /// <param name="id"></param>
        public void DeletePost(int id)
        {
            using (var tran = _session.BeginTransaction())
            {
                var post = _session.Get<Post>(id);
                _session.Delete(post);
                tran.Commit();
            }
        }

        /// <summary>
        /// Return all the categories.
        /// </summary>
        /// <returns></returns>
        public IList<Category> Categories()
        {
            return _session.Query<Category>().OrderBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Return category based on id.
        /// </summary>
        /// <param name="id">Category id</param>
        /// <returns></returns>
        public Category Category(int id)
        {
            return _session.Query<Category>().FirstOrDefault(t => t.ID == id);
        }

        /// <summary>
        /// Add a new category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int AddCategory(Category category)
        {
            using (var tran = _session.BeginTransaction())
            {
                _session.Save(category);
                tran.Commit();
                return category.ID;
            }
        }

        /// <summary>
        /// Update an existing category.
        /// </summary>
        /// <param name="category"></param>
        public void EditCategory(Category category)
        {
            using (var tran = _session.BeginTransaction())
            {
                _session.SaveOrUpdate(category);
                tran.Commit();
            }
        }

        /// <summary>
        /// Delete a category permanently.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCategory(int id)
        {
            using (var tran = _session.BeginTransaction())
            {
                var category = _session.Get<Category>(id);
                _session.Delete(category);
                tran.Commit();
            }
        }
        
        /// <summary>
        /// Return all the tags.
        /// </summary>
        /// <returns></returns>
        public IList<Tag> Tags()
        {
            return _session.Query<Tag>().OrderBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Return tag based on id.
        /// </summary>
        /// <param name="id">Tag id</param>
        /// <returns></returns>
        public Tag Tag(int id)
        {
            return _session.Query<Tag>().FirstOrDefault(t => t.ID == id);
        }

        /// <summary>
        /// Add a new tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public int AddTag(Tag tag)
        {
            using (var tran = _session.BeginTransaction())
            {
                _session.Save(tag);
                tran.Commit();
                return tag.ID;
            }
        }

        /// <summary>
        /// Edit an existing tag.
        /// </summary>
        /// <param name="tag"></param>
        public void EditTag(Tag tag)
        {
            using (var tran = _session.BeginTransaction())
            {
                _session.SaveOrUpdate(tag);
                tran.Commit();
            }
        }

        /// <summary>
        /// Delete an existing tag permanently.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteTag(int id)
        {
            using (var tran = _session.BeginTransaction())
            {
                var tag = _session.Get<Tag>(id);
                _session.Delete(tag);
                tran.Commit();
            }
        }
    }

}
