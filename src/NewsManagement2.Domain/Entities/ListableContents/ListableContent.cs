using NewsManagement2.Entities.ListableContentRelations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.ListableContents
{
    public class ListableContent : FullAuditedAggregateRoot<int>, IMultiTenant
    {
        public string Title { get; set; }
        public string Spot { get; set; }
        public Guid? ImageId { get; set; }
        public Guid? TenantId { get; set; }
        public int ViewsCount { get; set; }
        public StatusType Status { get; set; }
        public DateTime? PublishTime { get; set; }
        public ListableContentType ListableContentType { get; set; }
        public List<ListableContentTag> ListableContentTags { get; set; }
        public List<ListableContentCity> ListableContentCities { get; set; }
        public List<ListableContentCategory> ListableContentCategories { get; set; }


        public List<ListableContentRelation>? ListableContentRelations { get; set; }

        public ListableContent()
        {
            ListableContentRelations = new List<ListableContentRelation>();
        }


    }
}
