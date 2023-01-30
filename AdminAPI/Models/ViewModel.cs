namespace AdminAPI.Models
{
    public class ViewModel
    {
        public string[] Errors { get; set; }
        public string[] Successes { get; set; }
        public dynamic Data { get; set; }
        public Customer Customer { get; set; }
    }
}
