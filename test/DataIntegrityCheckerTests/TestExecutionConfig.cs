using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace DataIntegrityCheckerTests
{
	public class TestExecutionConfig
	{
		public SourceConfig source { get; set; }
		public SourceConfig target { get; set; }
		public List<string> scope { get; set; }

		// Display name for Console
		public override string ToString()
		{
			return $"({source.type}) {source.name} - ({target.type}) {target.name}";
		}
	}

	public class SourceConfig : IXunitSerializable
	{
		public string name { get; set; }
		public string type { get; set; }
		public SourceOptions options { get; set; }

		// Display name for Console
		public override string ToString()
		{
			return $"({type}) {name}";
		}

		// Fix for tests count
		public void Deserialize(IXunitSerializationInfo info)
		{
			var serialized = info.GetValue<string>("SourceConfig");
			var instance = JsonConvert.DeserializeObject<SourceConfig>(serialized);
			this.name = instance.name;
			this.type = instance.type;
			this.options = instance.options;
		}

		public void Serialize(IXunitSerializationInfo info)
		{
			info.AddValue("SourceConfig", JsonConvert.SerializeObject(this), typeof(string));
		}
	}

	public class SourceOptions
	{
		public string order_by { get; set; }
		public int? limit { get; set; }
		public List<string> skip_columns { get; set; }
		public Dictionary<string, string> override_column_type { get; set; }
		public Dictionary<string, string> override_column_name { get; set; }
	}
}