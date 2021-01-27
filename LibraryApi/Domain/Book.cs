﻿using System;

namespace LibraryApi.Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateTime AddedToInventory { get; set; }
        public bool IsInInventory { get; set; }
    }
}