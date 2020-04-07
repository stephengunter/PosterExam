using Infrastructure.Entities.Mongo;

namespace ApplicationCore.Models.Mongo
{
    [BsonCollection("SubjectQuestions")]
    public class SubjectQuestions : BaseDocument
    {
        public int SubjectId { get; set; }
    }
}
