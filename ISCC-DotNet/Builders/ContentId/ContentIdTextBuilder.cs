using System;
using ISCC.Interfaces;
namespace ISCC.Builders.ContentId
{
    public class ContentIdTextBuilder : IContentIdTextBuilder
    {
        public ContentIdTextBuilder()
        {
        }

        public string GetTextContentId(string text, int version = 0)
        {
            return String.Empty;
        }
    }
}
