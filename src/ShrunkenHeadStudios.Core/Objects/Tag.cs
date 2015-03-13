using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShrunkenHeadStudios.Core.Objects
{
    //Validation added for admin backend purposes
    public class Tag
    {
        public virtual int ID
        { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(500, ErrorMessage = "Name should not exceed 500 characters")]
        public virtual string Name
        { get; set; }

        [Required(ErrorMessage = "UrlSlug is required")]
        [StringLength(500, ErrorMessage = "UrlSlug should not exceed 500 characters")]
        public virtual string UrlSlug
        { get; set; }

        public virtual string Description
        { get; set; }

        [JsonIgnore]
        public virtual IList<Post> Posts
        { get; set; }
    }
}
