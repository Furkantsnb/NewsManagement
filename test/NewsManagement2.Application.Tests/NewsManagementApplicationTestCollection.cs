using NewsManagement2.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NewsManagement2
{
    [CollectionDefinition(NewsManagementTestConsts.CollectionDefinitionName)]
    public class NewsManagementApplicationTestCollection : NewsManagementEFCoreTestCollectionFixtureBase
    {
    }
}
