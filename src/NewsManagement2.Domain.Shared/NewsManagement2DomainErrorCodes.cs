namespace NewsManagement2;

public static class NewsManagement2DomainErrorCodes
{
    //100 Serisi: Bilgilendirme(Informational Responses)
    //200 Serisi: Başarılı Yanıtlar (Success Responses)
    //300 Serisi: Yönlendirme (Redirection Responses)
    //400 Serisi: İstemci Hataları (Client Errors)
    //500 Serisi: Sunucu Hataları (Server Errors)

    // Genel Hatalar
    public const string InvalidFilterCriteria = "NewsManagement:400"; //hatalı filtreleme
    public const string ResourceNotFound = "NewsManagement:404"; // Kaynak bulunamadı
    public const string ResourceAlreadyExists = "NewsManagement:409"; // Çakışma (Conflict)

    // Şehir Kodları Hataları
    public const string DuplicateCityCode = "NewsManagement:470"; // Şehir kodu zaten var

}
