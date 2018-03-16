using HtmlAgilityPack;

namespace adclean
{
    public interface IBlogFormattingFilter
    {
        string Process(string htmlContent);
    }
}