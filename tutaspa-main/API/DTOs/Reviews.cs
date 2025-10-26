namespace API.DTOs
{
    public class Review
    {
        public int Rate { get; set; }

        public string? Content { get; set; }

        public string Name { get; set; } = "Ẩn danh"; 

        public DateTime CreatedDate { get; set; }


    }
}
