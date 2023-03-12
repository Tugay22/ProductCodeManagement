namespace ProductCodeManagement.Services.Abstract
{
    public interface IProductCodeService
    {
        List<string> GenerateCode(int count);
        string CheckCode(string code);
    }
}
