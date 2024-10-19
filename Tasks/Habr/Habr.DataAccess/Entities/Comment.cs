#nullable disable
namespace Habr.DataAccess.Entities
{
    public class Comment : Declaration
    {
        public bool IsPostReply { get; set; }
        public int ParrentId { get; set; }
        public Declaration Parrent { get; set; }
    }
}
