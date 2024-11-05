using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.Newses
{
    public class NewsDetailImage : Entity, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid DetailImageId { get; set; }
        public int NewsId { get; set; }
        public News News { get; set; }

        public NewsDetailImage() { }

        public override object[] GetKeys()
        {
            return new object[] { NewsId, DetailImageId };
        }
    }
}
