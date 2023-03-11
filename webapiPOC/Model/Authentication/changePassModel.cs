using System.ComponentModel.DataAnnotations;

namespace webapiPOC.Model.Authentication
{
    public class changePassModel
    {
        [Required(ErrorMessage = "current email is required")]
        [EmailAddress]
        public string email { get; set; }

        [Required(ErrorMessage ="current password is required")]
        public string currentPass { get; set; }

        [Required(ErrorMessage ="password required ")]
        [DataType(DataType.Password)]
        public string newPass { get; set; }
    }
}
