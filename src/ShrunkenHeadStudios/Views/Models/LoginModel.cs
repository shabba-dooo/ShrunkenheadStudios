using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ShrunkenHeadStudios.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Incorrect username")]
        [Display(Name = "Username:")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Incorrect password")]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}