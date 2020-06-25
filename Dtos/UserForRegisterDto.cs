using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HOME.FINANCEMENT.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength =4,ErrorMessage ="you must specify password between 4 and 8 Characters")]
        public string Password { get; set; }

    }
}
