using NewsManagement2.Entities.Cities;
using NewsManagement2.Entities.ListableContents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.ListableContentRelations
{
    public class ListableContentCity : Entity, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
        public int ListableContentId { get; set; }
        public ListableContent ListableContent { get; set; }


        internal ListableContentCity() { }

        public override object[] GetKeys()
        {
            return new object[] { CityId, ListableContentId };
        }
    }
}
