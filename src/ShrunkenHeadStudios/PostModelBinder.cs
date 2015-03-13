using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ShrunkenHeadStudios.Core;
using ShrunkenHeadStudios.Core.Objects;
using Ninject;
using System.Web.Mvc;

namespace ShrunkenHeadStudios
{
    /// <summary>
    /// Bind POST model to actions - Jquery Grid
    /// </summary>
    public class PostModelBinder : DefaultModelBinder
    {
        private readonly IKernel _kernel;

        public PostModelBinder(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var post = (Post)base.BindModel(controllerContext, bindingContext);

            var blogRepository = _kernel.Get<IBlogRepository>();

            if (post.Category != null)
                post.Category = blogRepository.Category(post.Category.ID);

            var tags = bindingContext.ValueProvider.GetValue("Tags").AttemptedValue.Split(',');

            if (tags.Length > 0)
            {
                post.Tags = new List<Tag>();

                foreach (var tag in tags)
                {
                    post.Tags.Add(blogRepository.Tag(int.Parse(tag.Trim())));
                }
            }

            if (bindingContext.ValueProvider.GetValue("oper").AttemptedValue.Equals("edit"))
                post.Modified = DateTime.UtcNow; // dates are stored in UTC timezone.
            else
                post.PostedOn = DateTime.UtcNow;

            return post;
        }
    }
}