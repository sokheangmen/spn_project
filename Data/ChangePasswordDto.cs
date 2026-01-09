using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class ChangePasswordDto
    {
        [Required, MinLength(6)]
        public string NewPassword { get; set; }
    }
}
