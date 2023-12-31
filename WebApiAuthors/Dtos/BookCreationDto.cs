﻿using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos
{
    public class BookCreationDto
    {
        [Required]
        [MinLength(3)]
        public string Title { get; set; }

        public DateTime DatePublication { get; set; }

        public List<int> AuthorsIds { get; set; }
    }
}
