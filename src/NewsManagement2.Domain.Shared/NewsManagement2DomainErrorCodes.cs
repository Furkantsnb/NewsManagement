namespace NewsManagement2;

public static class NewsManagement2DomainErrorCodes
{
    //100 Serisi: Bilgilendirme(Informational Responses)
    //200 Serisi: Başarılı Yanıtlar (Success Responses)
    //300 Serisi: Yönlendirme (Redirection Responses)
    //400 Serisi: İstemci Hataları (Client Errors)
    //500 Serisi: Sunucu Hataları (Server Errors)

    // Genel Hatalar
    public const string InvalidFilterCriteria = "NewsManagement2:400"; //hatalı filtreleme
    public const string ResourceNotFound = "NewsManagement2:404"; // Kaynak bulunamadı
    public const string ResourceAlreadyExists = "NewsManagement2:409"; // Çakışma (Conflict)


    // Kategori Hataları
    public const string MustHaveTheSameContentType = "NewsManagement2:422"; // Alt kategori ile ana kategori aynı içerik türüne sahip olmalıdır.
    public const string SubcategoryCannotHaveSameNameParentCategory = "NewsManagement2:423"; // Alt kategori, ana kategoriyle aynı isme sahip olamaz.
    public const string OnlyOneSubCategory = "NewsManagement2:424"; // Bir kategori yalnızca bir alt kategoriye sahip olabilir.
    public const string MainCategoryWithSubCannotBeChanged = "NewsManagement2:425"; // Alt kategorilere sahip olan bir ana kategori değiştirilemez.

    // Şehir Kodları Hataları
    public const string DuplicateCityCode = "NewsManagement2:470"; // Şehir kodu zaten var

}
