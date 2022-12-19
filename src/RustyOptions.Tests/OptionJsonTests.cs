using System.Text.Json;
using System.Text.Json.Nodes;
using static RustyOptions.Option;

namespace RustyOptions.Tests
{
    public class OptionJsonTests
    {
        private const string s_testJson = """
            {
                "number": 3,
                "string": "test",
                "array": [1,2,3],
                "date": "2022-12-07"
            }
            """;

        [Fact]
        public void CanGetJsonNodeValue()
        {
            var obj = (JsonObject)JsonNode.Parse(s_testJson)!;

            var numVal = obj.GetPropValue<int>("number");
            var stringVal = obj.GetPropValue<string>("string");
            var arrVal = obj.GetPropValue<int[]>("array");
            var dateVal = obj.GetPropValue<DateTime>("date");
            var noVal = obj.GetPropValue<decimal>("bogus");
            var wrongVal = obj.GetPropValue<int>("string");

            Assert.Equal(Some(3), numVal);
            Assert.Equal(Some("test"), stringVal);
            Assert.True(arrVal.IsNone); // GetPropValue does not support arrays
            Assert.Equal(Some(new DateTime(2022, 12, 7)), dateVal);
            Assert.True(noVal.IsNone);
            Assert.True(wrongVal.IsNone);

            var properArrVal = obj.GetPropOption("array");
            Assert.True(properArrVal.IsSome(out var arrNode));
            Assert.True(arrNode is JsonArray jArr && jArr.Count == 3);
        }

        [Fact]
        public void CanGetJsonElementProps()
        {
            using var doc = JsonDocument.Parse(s_testJson);

            var numVal = doc.RootElement
                .GetPropOption("number")
                .AndThen(x => Option.Bind<int>(x.TryGetInt32));

            var stringVal = doc.RootElement
                .GetPropOption("string".AsSpan())
                .AndThen(x => x.GetString().Some());

            var dateVal = doc.RootElement
                .GetPropOption("date"u8)
                .AndThen(x => Option.Bind<DateTime>(x.TryGetDateTime));

            var noVal = doc.RootElement
                .GetPropOption("bogus"u8)
                .AndThen(x => Option.Bind<decimal>(x.TryGetDecimal));

            var wrongVal = doc.RootElement
                .GetPropOption("string"u8)
                .AndThen(x => Option.Bind<int>(x.TryGetInt32));

            Assert.Equal(Some(3), numVal);
            Assert.Equal(Some("test"), stringVal);
            Assert.Equal(Some(new DateTime(2022, 12, 7)), dateVal);
            Assert.True(noVal.IsNone);
            Assert.True(wrongVal.IsNone);
        }
    }
}

