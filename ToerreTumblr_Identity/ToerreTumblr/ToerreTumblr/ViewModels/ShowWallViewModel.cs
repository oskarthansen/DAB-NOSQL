using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToerreTumblr.Models;

namespace ToerreTumblr.ViewModels
{
    public class ShowWallViewModel
    {
        public List<Post> Posts { get; set; }
        public User User { get; set; }
        public Boolean isFollowing { get; set; }
    }
}
