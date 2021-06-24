using System;

namespace TodoCli
{
    public class Todo
    {
        public long Id { get; init; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; init; }
        
    }
}