using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string Time { get; set; } = null!;
        public override string ToString()
        {
            return Time + " " + UserName + ": " + Text;
        }
    }
}
