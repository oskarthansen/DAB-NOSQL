using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToerreTumblr.Models;

namespace ToerreTumblr.ViewModels
{
    public class EditCommentViewModel
    {
        public Post Post { get; set; }
        public Comment Comment { get; set; }
    }
}
