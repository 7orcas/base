using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Search.Base
{
    public class UserSearch : _BaseSearch
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Email);
        }
    }

}
