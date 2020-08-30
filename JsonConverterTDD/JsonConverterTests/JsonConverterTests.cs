using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonConverterTDD;

namespace JsonConverterTests
{
    [TestClass]
    public class JsonConverterTests
    {
        [TestMethod]
        public void TestHtmlDecoder()
        {
            HtmlText doubleQuoteIncludedText = new HtmlText("&quot;Hello&quot;");
            string expectedDecodeResult = "\"Hello\"";
            Assert.AreEqual(expectedDecodeResult, doubleQuoteIncludedText.GetDecodedText());
        }
    }
}
