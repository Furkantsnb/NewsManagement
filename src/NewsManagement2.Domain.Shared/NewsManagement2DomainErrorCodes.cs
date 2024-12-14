namespace NewsManagement2;

public static class NewsManagement2DomainErrorCodes
{
    //100 Serisi: Bilgilendirme(Informational Responses)
    //200 Serisi: Başarılı Yanıtlar (Success Responses)
    //300 Serisi: Yönlendirme (Redirection Responses)
    //400 Serisi: İstemci Hataları (Client Errors)
    //500 Serisi: Sunucu Hataları (Server Errors)

 


    // Genel Hatalar
    public const string ResourceNotFound = "NewsManagement2:404"; // Kaynak bulunamadı
    public const string ResourceAlreadyExists = "NewsManagement2:409"; // Çakışma (Conflict)
    public const string InvalidFilterCriteria = "NewsManagement2:400"; // Hatalı filtreleme

    // Kategori ve Alt Kategori Hataları
    public const string MaxSubcategoryLimitReached = "NewsManagement2:420"; // Alt kategori limiti aşıldı
    public const string MainCategoryModificationRestricted = "NewsManagement2:421"; // Ana kategori değiştirilemez
    public const string SubcategoryNameConflict = "NewsManagement2:422"; // Alt kategori ismi çakışıyor
    public const string ParentCategoryRequired = "NewsManagement2:423"; // Üst kategori gerekli
    public const string SelfReferenceError = "NewsManagement2:424"; // Kendi kendine referans verilemez 
    public const string MustHaveTheSameContentType = "NewsManagement2:425"; // Alt kategori ve ana kategori aynı içerik türüne sahip olmalıdır.


    // Şehir Kodları Hataları
    public const string DuplicateCityCode = "NewsManagement2:470"; // Şehir kodu zaten var

    // Durum (Status) Hataları
    public const string InvalidStatusTransition = "NewsManagement2:430"; // Geçersiz durum geçişi
    public const string MissingPublishTimeForSelectedStatus = "NewsManagement2:431"; // Yayın zamanı eksik
    public const string InvalidDraftPublishTime = "NewsManagement2:432"; // Taslak için yayın zamanı olamaz
    public const string InvalidScheduledPublishTime = "NewsManagement2:433"; // Planlanan zaman gelecekte olmalı
    public const string InvalidPublishedTime = "NewsManagement2:434"; // Yayınlanan durum "şu an" olmalı
    public const string DeletedStatusCannotBeModified = "NewsManagement2:435"; // Silinmiş durum düzenlenemez
    public const string RepeatedDataError = "NewsManagement:436";


    // Video ve Link Hataları
    public const string UrlRequiredForLinkContent = "NewsManagement2:440"; // Link türü içeriklerde URL gerekli
    public const string VideoIdRequiredForVideoContent = "NewsManagement2:441"; // Video türü içeriklerde VideoId gerekli
    public const string UrlNotAllowedForVideoContent = "NewsManagement2:442"; // Video türü içeriklerde URL olamaz
    public const string VideoIdNotAllowedForLinkContent = "NewsManagement2:443"; // Link türü içeriklerde VideoId olamaz

    // Sıralama ve Filtreleme Hataları
    public const string SortingParameterInvalid = "NewsManagement2:450"; // Geçersiz sıralama parametresi
    public const string TooManyItemsSkipped = "NewsManagement2:451"; // Çok fazla öğe atlandı

    // Doğrulama Hataları (Validation Errors)
    #region ValidationErrors
    public const string InvalidContentTypeSelection = "NewsManagement2:460"; // Geçersiz içerik türü seçimi
    public const string ActiveCategoryLimitExceeded = "NewsManagement2:461"; // Birden fazla aktif kategori olamaz
    public const string StatusEnumValidationFailed = "NewsManagement2:462"; // Geçersiz durum tipi
    public const string VideoEnumValidationFailed = "NewsManagement2:463"; // Geçersiz video türü
    #endregion

   

}
