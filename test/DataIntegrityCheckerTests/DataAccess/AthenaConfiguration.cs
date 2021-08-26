namespace DataIntegrityCheckerTests.DataAccess
{
	public interface IAthenaConfiguration
	{
		string AccessKey { get; set; }
		string AccessSecret { get; set; }
		string QueryContextRawDataBase { get; set; }
		string QueryContextDomainDataBase { get; set; }
		string QueryOutputLocation { get; set; }
		string QueryContextDataBase { get; }
	}

	public class RawAthenaConfiguration : IAthenaConfiguration
	{
		public string AccessKey { get; set; }
		public string AccessSecret { get; set; }
		public string QueryContextRawDataBase { get; set; }
		public string QueryContextDomainDataBase { get; set; }
		public string QueryOutputLocation { get; set; }
		public string QueryContextDataBase => QueryContextRawDataBase;
	}

	public class DomainAthenaConfiguration : IAthenaConfiguration
	{
		public string AccessKey { get; set; }
		public string AccessSecret { get; set; }
		public string QueryContextRawDataBase { get; set; }
		public string QueryContextDomainDataBase { get; set; }
		public string QueryOutputLocation { get; set; }
		public string QueryContextDataBase => QueryContextDomainDataBase;
	}
}