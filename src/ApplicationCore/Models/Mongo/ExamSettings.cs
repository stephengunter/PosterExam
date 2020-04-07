using Infrastructure.Entities.Mongo;

namespace ApplicationCore.Models.Mongo
{
    [BsonCollection("ExamSettings")]
    public class ExamSettings : BaseDocument
    {
        public int SubjectId { get; set; }
    }
}
