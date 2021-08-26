namespace DataIntegrityCheckerTests.DataUtils
{
	public interface ITransformation
	{
		string Execute(string columnName, object row);
	}
}