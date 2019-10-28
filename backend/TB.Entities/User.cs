using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TB.Entities
{
    public class User: IdentityUser<int>
    {
        [Required]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(25)]
        public string LastName { get; set; }

        [Required]
        public bool IsEnabled { get; set; }
    }
}
