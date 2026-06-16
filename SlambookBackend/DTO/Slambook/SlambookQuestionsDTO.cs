namespace SlambookBackend.DTO.Slambook
{
    public class SlambookQuestionsDTO
    {
        public int SlambookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuestionItemDTO> Questions { get; set; } = new List<QuestionItemDTO>();
    }
}
