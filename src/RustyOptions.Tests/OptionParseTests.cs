#if NET7_0_OR_GREATER

using static RustyOptions.Option;

namespace RustyOptions.Tests
{
    public class OptionParseTests
    {
        [Fact]
        public void CanParseStrings()
        {
            var integer = Option.Parse<int>("12345");
            var date = Option.Parse<DateTime>("2023-06-17");
            var timespan = Option.Parse<TimeSpan>("05:11:04");
            var fraction = Option.Parse<double>("3.14");
            var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965");
            var nothing = Option.Parse<int>("foo");

            Assert.True(integer.IsSome(out var i) && i == 12345);
            Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
            Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
            Assert.True(fraction.IsSome(out var x) && x == 3.14);
            Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
            Assert.True(nothing.IsNone);
        }

        [Fact]
        public void CanParseSpans()
        {
            var integer = Option.Parse<int>("12345".AsSpan());
            var date = Option.Parse<DateTime>("2023-06-17".AsSpan());
            var timespan = Option.Parse<TimeSpan>("05:11:04".AsSpan());
            var fraction = Option.Parse<double>("3.14".AsSpan());
            var guid = Option.Parse<Guid>("ac439fd6-9b64-42f3-bc74-38017c97b965".AsSpan());
            var nothing = Option.Parse<int>("foo".AsSpan());

            Assert.True(integer.IsSome(out var i) && i == 12345);
            Assert.True(date.IsSome(out var d) && d == new DateTime(2023, 06, 17));
            Assert.True(timespan.IsSome(out var t) && t == new TimeSpan(5, 11, 4));
            Assert.True(fraction.IsSome(out var x) && x == 3.14);
            Assert.True(guid.IsSome(out var g) && g == new Guid("ac439fd6-9b64-42f3-bc74-38017c97b965"));
            Assert.True(nothing.IsNone);
        }
    }
}

#endif
