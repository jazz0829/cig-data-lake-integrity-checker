namespace DataIntegrityCheckerTests.DataAccess
{
	public interface ICiDbConfiguration
	{
		string ConnectionString { get; set; }
	}

	public class CiDbConfiguration: ICiDbConfiguration
	{
		public string ConnectionString { get; set; }
	}
}
