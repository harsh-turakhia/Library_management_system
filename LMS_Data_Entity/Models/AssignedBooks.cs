namespace Library_management_system.Models
{
    public class AssignedBooks
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public string BookName { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int IssuedBy { get; set; }

        public DateOnly IssuedDate { get; set; }

        public DateOnly ReturnDate { get; set; }

        public DateOnly ReturnedOn { get; set; }

        public int Status { get; set; }

        public string StatusName { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<Status>? StatusList { get; set; }
    }
}
