using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ToerreTumblr.Models;

namespace ToerreTumblr.ViewModels
{
    public class CircleViewModel
    {
        public List<string> Users { get; set; }
        public List<string> UsersLogin { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
