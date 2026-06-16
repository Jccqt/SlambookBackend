namespace SlambookBackend.DTO.Slambook
{
    public class QuestionAnswerDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
    }
}
