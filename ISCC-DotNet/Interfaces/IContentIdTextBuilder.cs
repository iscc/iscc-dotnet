namespace ISCC.Interfaces
{
    public interface IContentIdTextBuilder
    {
        string GetTextContentId(string text, int version = 0);
    }
}