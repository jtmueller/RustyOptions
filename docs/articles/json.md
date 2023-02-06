# JSON Serialization

RustyOptions types support serialization and deserialization using `System.Text.Json`.

**Options:**
 - A `Some` option will be serialized as the raw value in json.
 - A `None` option will be serialized as explicitly null in json.
 - When parsing, a missing property will be deserialized as `None`.
 - An explicitly null value will be deserialized as `None`.
 - Any other value will be deserialized as `Some`.

 ```csharp
using RustyOptions;
using System.Text.Json;

record Person(string First, Option<string> Middle, string Last, Option<int> Age);

string json1 = "{ \"first\": \"James\", \"middle\": \"Tiberius\", \"last\": \"Kirk\" }";
string json2 = "{ \"first\": \"Martin\", \"last\": \"Redshirt\", \"age\": 23 }";

var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

var person1 = JsonSerializer.Deserialize<Person>(json1, options);
// [Person { First = James, Middle = Some(Tiberius), Last = Kirk, Age = None }]

var person1 = JsonSerializer.Deserialize<Person>(json2, options);
// [Person { First = Martin, Middle = None, Last = Redshirt, Age = Some(23) }]
 ```

 **Results:**
   - A `Result<T, TErr>` will be serialized or parsed as an object that contains either an `ok` or an `err` property:
     - `{ "ok": 42 }`
     - `{ "err": "oops!" }`
