using NewsManagement2.Entities.ListableContents;
using NewsManagement2.EntityConsts.ListableContentConsts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace NewsManagement2.Entities.BackgroundJobs
{
    /// <summary>
    /// Zamanlanmış içeriklerin durumlarını kontrol edip, yayınlanmış (`Published`) duruma geçiren bir arka plan işi.
    /// </summary>
    public class UpdateScheduledStatusJob : AsyncBackgroundJob<int>, ITransientDependency
    {
        private readonly IListableContentRepository _contentRepository; // İçeriklerin yönetimi için repository.
        private readonly IDataFilter<IMultiTenant> _tenantFilter; // Kiracı desteği yönetimi için kullanılan filtre.

       
        public UpdateScheduledStatusJob(IListableContentRepository contentRepository, IDataFilter<IMultiTenant> tenantFilter)
        {
            _contentRepository = contentRepository;
            _tenantFilter = tenantFilter;
        }

        /// <summary>
        /// Arka plan işini gerçekleştirir.
        /// </summary>
        /// <param name="args">İşle ilgili argümanlar. Bu sınıfta kullanılmamaktadır.</param>
        public override async Task ExecuteAsync(int args)
        {
            // Kiracı filtresini devre dışı bırakır.
            using (_tenantFilter.Disable())
            {
                // Yayın zamanı gelmiş ve "Scheduled" durumundaki içerikleri sorgular.
                var scheduledContents = await _contentRepository.GetListAsync(content =>
                    content.Status == StatusType.Scheduled && // Durumu "Scheduled" olan içerikler.
                    content.PublishTime <= DateTime.Now       // Yayınlanma zamanı geçmiş veya şu an olan içerikler.
                );

                // İçeriklerin durumlarını "Published" olarak günceller.
                foreach (var content in scheduledContents)
                {
                    content.Status = StatusType.Published; // Durumu "Published" olarak değiştirir.
                    content.PublishTime = DateTime.Now;    // Yayınlanma zamanını şu an olarak ayarlar.

                    await _contentRepository.UpdateAsync(content, autoSave: true); // Güncellemeyi kaydeder.
                }
            }
        }
    }
}
