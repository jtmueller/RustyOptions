using static RustyOptions.Option;
using static RustyOptions.Result;

namespace RustyOptions.Tests;

public class JsonSerializationTests
{


    private class ClassWithOptions
    {
        public int Foo { get; set; }
        public Option<int> Bar { get; set; }
        public Option<string> Name { get; set; }
        public Option<DateTimeOffset> LastUpdated { get; set; }
    }

    private class ClassWithResult
    {
        public int Foo { get; set; }
        public Result<int, string> CurrentCount { get; set; }
    }

    private class ClassWithOptionAndResult
    {
        public int Foo { get; set; }
        public Option<int> Bar { get; set; }
        public Result<int, string> Output { get; set; }
    }
}

