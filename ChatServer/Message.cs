using System;
using System.Collections.Generic;

namespace ChatServer
{
    public partial class Message
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
