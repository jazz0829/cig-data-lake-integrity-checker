using System.IO;
using System.Text;
using ApprovalTests;

namespace DataIntegrityCheckerTests.Infrastructure
{
	public class TwoTextsApprovalFileWriter : ApprovalTextWriter
	{
		private readonly string approvedFilePath;
		private string receivedFilePath;

		public TwoTextsApprovalFileWriter(string data) : base(data)
		{
		}

		public TwoTextsApprovalFileWriter(string data, string extensionWithoutDot)
			: base(data, extensionWithoutDot)
		{
		}

		public TwoTextsApprovalFileWriter(
			string approvedText, string actualText, string nameOfExpected = "", string nameOfActual = "")
			: base(actualText, "txt")
		{
			var tmpFolder = Path.GetTempPath();
			if (!string.IsNullOrWhiteSpace(nameOfActual))
			{
				receivedFilePath = Path.ChangeExtension(
					Path.Combine(tmpFolder, makeSafeFilename(nameOfActual)),
					base.ExtensionWithDot);
			}
			var tmpFile = "tmpExpected.txt";
			if (!string.IsNullOrWhiteSpace(nameOfExpected))
			{
				tmpFile = makeSafeFilename(nameOfExpected);
			}
			approvedFilePath = Path.Combine(tmpFolder, tmpFile);
			File.WriteAllText(approvedFilePath, approvedText, Encoding.UTF8);
		}

		private string makeSafeFilename(string filename)
		{
			return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}

		public override string GetApprovalFilename(string basename)
		{
			return approvedFilePath;
		}

		public override string GetReceivedFilename(string basename)
		{
			if (string.IsNullOrEmpty(receivedFilePath))
			{
				receivedFilePath = Path.ChangeExtension(Path.Combine(Path.GetTempPath(), Path.GetTempFileName()), ExtensionWithDot);
			}
			return receivedFilePath;
		}
	}
}