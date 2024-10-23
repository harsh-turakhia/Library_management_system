namespace Library_management_system.Models
{
    public class BookDetails
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public int AuthorName { get; set; }

        public int Publication { get; set; }

        public int Copies { get; set; }

        public int? Price { get; set; }

        public int Language { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int IsDeleted { get; set; }
    }
}
