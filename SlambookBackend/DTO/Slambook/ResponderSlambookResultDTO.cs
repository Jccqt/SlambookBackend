using SlambookBackend.DTO.Profile;

namespace SlambookBackend.DTO.Slambook
{
    public class ResponderSlambookResultDTO
    {
        public MiniProfileDTO Responder { get; set; } = new MiniProfileDTO();
        public List<QuestionAnswerDTO> Answers { get; set; } = new(); 
    }
}
