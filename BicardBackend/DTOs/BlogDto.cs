﻿namespace BicardBackend.Models
{
    public class BlogDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int AuthorId { get; set; }
        public IFormFile? Photo { get; set; }

    }
}
