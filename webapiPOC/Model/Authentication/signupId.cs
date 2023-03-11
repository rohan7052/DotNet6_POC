using System.ComponentModel.DataAnnotations;

namespace webapiPOC.Model.Authentication
{
    public class signupId
    {
        [Required(ErrorMessage ="user name is required")]
        public string?  Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "email is required")]
        public string?  Email { get; set; }

        [Required(ErrorMessage = "password is required")]
        public string? Password { get; set; }
    }
}
