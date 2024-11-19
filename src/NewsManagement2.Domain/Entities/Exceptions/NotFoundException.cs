using NewsManagement2.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Localization;
using Volo.Abp;
using Microsoft.Extensions.Localization;

namespace NewsManagement2.Entities.Exceptions
{
    public class NotFoundException : BusinessException, ILocalizeErrorMessage
    {
        public Type EntityType { get; set; }
        public string EntityCode { get; set; }

        public NotFoundException(Type entityType, string entityCode) : base(NewsManagement2DomainErrorCodes.ResourceNotFound)
        {
            EntityType = entityType;
            EntityCode = entityCode;
        }

        public string LocalizeMessage(LocalizationContext context)
        {
            var localizer = context.LocalizerFactory.Create<NewsManagement2Resource>();

            Data["EntityType"] = localizer[EntityType.Name!].Value;
            Data["EntityCode"] = EntityCode;

            return localizer[Code!, Data["EntityType"], Data["EntityCode"]];
        }
    }
}
