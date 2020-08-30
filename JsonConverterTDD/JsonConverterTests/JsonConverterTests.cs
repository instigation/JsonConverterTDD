using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonConverterTDD;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

        [TestMethod]
        public void TestJTokenAdapterConstructorFromJToken()
        {
            JToken jToken = JToken.Parse(@"{'age':10,'name':'Alice'}");
            JTokenAdapter adapter = new JTokenAdapter(jToken);
            Assert.AreEqual(2, adapter.Children.Count);
        }

        [TestMethod]
        public void TestJTokenAdapterForObject()
        {
            JToken jToken = JToken.Parse(@"{'info':{'age':10,'name':'Smith'}}");
            JTokenAdapter adapter = new JTokenAdapter(jToken);
            Assert.AreEqual(1, adapter.Children.Count);
            foreach(var infoChild in adapter.Children)
            {
                Assert.AreEqual("info", infoChild.Property);
                Assert.AreEqual(2, infoChild.Children.Count);
                var properties = new List<string>{ "age", "name"};
                var values = new List<JToken>{ new JValue(10), new JValue("Smith")};
                foreach(var child in infoChild.Children)
                {
                    Assert.IsTrue(properties.Contains(child.Property));
                    Assert.IsTrue(values.Contains(child.Value));
                }
            }
        }

        [TestMethod]
        public void TestJTokenAdapterForIntValue()
        {
            JTokenAdapter adapter = new JTokenAdapter("age", new JValue(10));
            Assert.AreEqual("10", adapter.ValueToString);
        }

        [TestMethod]
        public void TestJTokenAdapterForBoolValue()
        {
            JTokenAdapter adapter = new JTokenAdapter("married", new JValue(false));
            Assert.AreEqual(JTokenType.Boolean, adapter.Value.Type);
            Assert.AreEqual("False", adapter.ValueToString);
        }

        [TestMethod]
        public void TestJTokenAdapterForStringValue()
        {
            JTokenAdapter adapter = new JTokenAdapter("name", new JValue("Smith"));
            Assert.AreEqual("'Smith'", adapter.ValueToString);
        }
    }

    [TestClass]
    public class NewtonSoftJsonTests
    {
        [TestMethod]
        public void TestJObjectParse()
        {
            string jsonString = "{\"age\":\"10\"}";
            JObject o = JObject.Parse(jsonString);
            Assert.AreEqual("10", o["age"]);
        }

        [TestMethod]
        public void TestJObjectEnumerate()
        {
            string jsonString = "{\"age\":\"10\", 'name':'minsu'}";
            JObject o = JObject.Parse(jsonString);
            var attributes = new List<KeyValuePair<string, JToken>>(o);
            Assert.IsTrue(attributes.Contains(new KeyValuePair<string, JToken>("age", "10")));
            Assert.IsTrue(attributes.Contains(new KeyValuePair<string, JToken>("name", "minsu")));
        }

        [TestMethod]
        public void TestJTokenType()
        {
            string jsonString = "{'info':{\"age\":10, 'name':'minsu'}}";
            JObject jObject = JObject.Parse(jsonString);
            JToken infoToken = jObject["info"];
            Assert.AreEqual(infoToken.Type, JTokenType.Object);
            JToken ageToken = infoToken["age"];
            Assert.AreEqual(ageToken.Type, JTokenType.Integer);
        }
    }
}
