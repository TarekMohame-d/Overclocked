using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string TagName { get; set; }

        // Navigations
        public ICollection<TagProduct> TagProducts { get; set; }
    }
}
