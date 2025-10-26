namespace API.DTOs
{
    public class ReviewsDTO
    {
        public int DichVuId { get; set; }


        public double Rating { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
