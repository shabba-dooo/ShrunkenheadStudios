using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace ShrunkenHeadStudios.Core.Objects
{
    //Validation added for admin backend purposes
    public class Post
    {
        [Required(ErrorMessage = "ID is required")]
        public virtual int ID
        { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, ErrorMessage = "Title should not exceed 500 characters")]
        public virtual string Title
        { get; set; }

        [Required(ErrorMessage = "Short description is required")]
        public virtual string ShortDescription
        { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public virtual string Description
        {get; set;}

        [Required(ErrorMessage = "Metadata is required")]
        [StringLength(1000, ErrorMessage = "Metadata should not exceed 1000 characters")]
        public virtual string Meta
        { get; set; }

        [Required(ErrorMessage = "UrlSlug is required")]
        [StringLength(50, ErrorMessage = "UrlSlug should not exceed 50 characters")]
        public virtual string UrlSlug
        { get; set; }

        public virtual bool Published
        { get; set; }

        [Required(ErrorMessage = "Posted on is required")]
        public virtual DateTime PostedOn
        { get; set; }

        public virtual DateTime? Modified
        { get; set; }

        public virtual Category Category
        { get; set; }

        public virtual IList<Tag> Tags
        { get; set; }

        public virtual string BannerImage
        { get; set; }
    }
}
