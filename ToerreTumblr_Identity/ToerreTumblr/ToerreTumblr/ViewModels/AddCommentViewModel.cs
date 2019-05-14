using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToerreTumblr.Models;

namespace ToerreTumblr.ViewModels
{
    public class AddCommentViewModel
    {
        public string PostId { get; set; }
        public string SourceId { get; set; }
        public string SharedType { get; set; }
        public Comment Comment { get; set; }
    }
}
