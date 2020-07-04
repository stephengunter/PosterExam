using Infrastructure.Entities.Mongo;

namespace ApplicationCore.Models.Mongo
{
    [BsonCollection("NoteParams")]
    public class NoteParams : BaseDocument
    {
        public string UserId { get; set; }
    }
}
