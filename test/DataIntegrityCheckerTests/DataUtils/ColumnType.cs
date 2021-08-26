namespace DataIntegrityCheckerTests.DataUtils
{
	public class ColumnType
	{
		public string ColumnName { get; }
		public string TypeName { get; }
		public string IsNullable { get; }

		public ColumnType(
			string columnName, string typeName, string isNullable = "")
		{
			ColumnName = columnName;
			TypeName = typeName;
			IsNullable = isNullable;
		}

		public override string ToString()
		{
			// TODO: Nullability is always Unknown in AWS
			return $"{ColumnName} : {TypeName}";
		}
	}
}