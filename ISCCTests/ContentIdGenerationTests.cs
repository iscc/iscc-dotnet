using System;
using Moq;
using Xunit;
using ISCC.Interfaces;
using ISCC.Builders.ContentId;
using System.Runtime.Serialization.Formatters.Binary;

namespace ISCCTests
{
    public class ContentIdGenerationTests
    {
        private IContentIdBuilder contentBuilder;
        private IContentIdTextBuilder contentTextBuilder;

        public ContentIdGenerationTests()
        {
            contentTextBuilder = new ContentIdTextBuilder();
        }

        [Theory]
        [JsonFileData("testfiles/test_data.json")]
        public void content_id_text(string testName, object inputs, object expected)
        {
            var inputString = ((string[])inputs)[0];
            var expectedString = (string)expected;
            var cid_i = contentTextBuilder.GetTextContentId(inputString);

            Assert.True(cid_i == expected);
        }

        // Convert an object to a byte array
        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
