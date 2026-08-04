using Travel.Web.Entities.Common;

namespace Travel.Web.Entities
{
    public class Banner : BaseEntity
    {
        public string Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
