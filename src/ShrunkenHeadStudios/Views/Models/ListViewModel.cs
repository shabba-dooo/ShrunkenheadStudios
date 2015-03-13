using ShrunkenHeadStudios.Core;
using ShrunkenHeadStudios.Core.Objects;
using System.Collections.Generic;

namespace ShrunkenHeadStudios.Models
{
    public class ListViewModel
    {
        public ListViewModel(IBlogRepository blogRepository, int p)
        {
            Posts = blogRepository.Posts(p - 1, 3);
            TotalPosts = blogRepository.TotalPosts();
        }

        public ListViewModel(IBlogRepository blogRepository, string Text, string type, int p)
        {
            switch (type)
            {
                case "Category":
                    Posts = blogRepository.PostsForCategory(Text, p - 1, 3);
                    TotalPosts = blogRepository.TotalPostsForCategory(Text);
                    Category = blogRepository.Category(Text);
                    break;

                case "Tag":
                    Posts = blogRepository.PostsForTag(Text, p - 1, 3);
                    TotalPosts = blogRepository.TotalPostsForTag(Text);
                    Tag = blogRepository.Tag(Text);
                    break;
                default:
                    Posts = blogRepository.PostsForCategory(Text, p - 1, 3);
                    TotalPosts = blogRepository.TotalPostsForCategory(Text);
                    Category = blogRepository.Category(Text);
                    break;
            }
        }

        public IList<Post> Posts { get; private set; }
        public int TotalPosts { get; private set; }
        public Category Category { get; private set; }
        public Tag Tag { get; private set; }

    }
}