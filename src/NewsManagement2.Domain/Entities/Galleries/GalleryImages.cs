using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.Galleries
{
    public class GalleryImage : Entity, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public int Order { get; set; }
        public string NewsContent { get; set; }
        public Guid ImageId { get; set; }
        public int GalleryId { get; set; }
        public Gallery Gallery { get; set; }


        public GalleryImage() { }

        public override object[] GetKeys()
        {
            return new object[] { GalleryId, ImageId };
        }
    }
}
