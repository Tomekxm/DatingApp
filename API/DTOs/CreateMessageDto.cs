namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string recipientUsername { get; set; }
        public string content { get; set; }
    }
}