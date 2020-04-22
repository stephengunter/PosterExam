using Infrastructure.Entities.Mongo;

namespace ApplicationCore.Models.Mongo
{
    [BsonCollection("TermNotes")]
    public class TermNotes : BaseDocument
    {
        public int SubjectId { get; set; }

        public int TermId { get; set; }
    }
}
