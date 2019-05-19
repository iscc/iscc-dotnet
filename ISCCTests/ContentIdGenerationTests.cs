using System;
using Moq;
using Xunit;
using ISCC.Interfaces;
using ISCC.Builders.ContentId;
namespace ISCCTests
{
    public class ContentIdGenerationTests
    {
        private IContentIdBuilder contentBuilder;
        private IContentIdTextBuilder contentTextBuilder;

        public ContentIdGenerationTests()
        {
            //contentTextBuilder = new Mock<IContentIdTextBuilder>( new ContentIdTextBuilder()).Object;
        }

        [Theory, InlineData("")]
        [JsonFileData("testfiles/test_data.json")]
        public void content_id_text(string text)
        {
            //var cid_i = contentTextBuilder.GetTextContentId(text);

            //Assert.True(cid_i == 0x1);
        }
    }
}
