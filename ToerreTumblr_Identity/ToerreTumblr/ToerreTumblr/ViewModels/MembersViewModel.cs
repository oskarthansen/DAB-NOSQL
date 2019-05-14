using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ToerreTumblr.ViewModels
{
    public class helpclass
    {
        public string users { get; set; }
        public string usersId { get; set; }
    }

    public class MembersViewModel
    {
        public List<helpclass> Users { get; set; }
    }

    
}
