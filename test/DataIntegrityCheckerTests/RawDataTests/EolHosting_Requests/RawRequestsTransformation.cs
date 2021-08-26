using System.Collections.Generic;
using DataIntegrityCheckerTests.DataUtils;

namespace DataIntegrityCheckerTests.RawDataTests.EolHosting_Requests
{
	public class RawRequestsTransformation : ITransformation
	{
		public string PrepareAwsValue(
			string columnName, Dictionary<string, string> row)
		{
			var value = row[columnName.ToLowerInvariant()];
			switch (columnName)
			{
				case "syscreated":
				case "sysmodified":
				case "Approved":
				case "Realized":
				case "Processed":
				case "StartDate":
				case "EndDate":
				case "StartDateActual":
				case "EndDateActual":
				case "StatusChanged":
				case "CIGCopyTime":
				case "FreeDateField_01":
				case "FreeDateField_02":
				case "FreeDateField_03":
				case "FreeDateField_04":
				case "FreeDateField_05":
					value = AwsRawCellPreparationMapper
						.AsDateTimeWithNoMilliseconds(value);
					break;
				//case "CIGProcessed":
				//case "FreeBoolField_01":
				//case "FreeBoolField_02":
				//case "FreeBoolField_03":
				//case "FreeBoolField_04":
				//case "FreeBoolField_05":
				//case "IsAssetItem":
				//case "IsFractionAllowedItem":
				//case "IsSalesItem":
				//case "IsPackageItem":
				//case "IsPurchaseItem":
				//case "IsSerialNumberItem":
				//case "IsSubAssemblyItem":
				//case "IsSubcontractedItem":
				//case "IsTaxable":
				//case "IsStockItem":
				//	// tinyint
				//	//case "RoundEach":
				//	//case "CopyRemarks":
				//	//case "RoundPlannedQuantityFactor":
				//	//case "UseExplosion":
				//	//case "IsTime":
				//	//case "IsWebshopItem":
				//	//case "IsRegistrationCodeItem":
				//	//case "IsMakeItem":
				//	//case "IsNewContract":
				//	//case "IsOnDemandItem":
				//	//case "HasBillOfMaterial":
				//	//case "IsBatchNumberItem":
				//	value = AwsRawCellPreparationMapper.AsBool(value);
				//	break;
				//case "AverageCost":
				//case "BatchQuantity":
				//case "CalculatorUnitFactor":
				//case "CostPriceNew":
				//case "CostPriceStandard":
				//case "Depth":
				//case "FreeNumberField_01":
				//case "FreeNumberField_02":
				//case "FreeNumberField_03":
				//case "FreeNumberField_04":
				//case "FreeNumberField_05":
				//case "FreeNumberField_06":
				//case "FreeNumberField_07":
				//case "FreeNumberField_08":
				//case "GrossWeight":
				//case "Length":
				//case "Margin":
				//case "NetWeight":
				//case "Width":
				//case "StatisticalNetWeight":
				//case "StatisticalQuantity":
				//case "StatisticalUnits":
				//case "StatisticalValue":
				//case "WeightFactor":
				//	value = AwsRawCellPreparationMapper.AsFloat(value);
				//	break;
			}
			return $"{value}\n";
		}

		public string Execute(string columnName, object row)
		{
			var value = (row as IDictionary<string, object>)[columnName];
			switch (columnName)
			{
				case "ID":
				case "Account":
				case "Contact":
				case "FreeGuidField_01":
				case "FreeGuidField_02":
				case "FreeGuidField_03":
				case "FreeGuidField_04":
				case "FreeGuidField_05":
				case "FreeGuidField_06":
				case "FreeGuidField_07":
				case "FreeGuidField_08":
				case "FreeGuidField_09":
				case "FreeGuidField_10":
				case "Type":
				case "UserID":
				case "Processor":
				case "Realizer":
				case "Item_1":
				case "ItemNumber_1":
				case "Item_2":
				case "ItemNumber_2":
				case "Employee":
				case "Document":
				case "HelpFile":
				case "HelpItem":
				case "FinancialEntry":
				case "Project":
				case "Approver":
				case "XBRLDocument":
				case "Campaign":
				//case "ItemGroup":
				//case "DepricationMethod":
				//case "TopicParent":
				case "Opportunity":
				case "Lead":
				case "ProjectWBS":
				case "PayrollComponent":
				case "MultipleEntryID":
				case "Message":
				case "syscreator":
				case "sysmodifier":
					value = RawCellMapper.AsGuid(value);
					break;
				case "syscreated":
				case "sysmodified":
				case "Approved":
				case "Realized":
				case "Processed":
				case "StartDate":
				case "EndDate":
				case "StartDateActual":
				case "EndDateActual":
				case "StatusChanged":
				case "CIGCopyTime":
				case "FreeDateField_01":
				case "FreeDateField_02":
				case "FreeDateField_03":
				case "FreeDateField_04":
				case "FreeDateField_05":
					value = RawCellMapper
						.AsDateTimeWithNoMilliseconds(value);
					break;
					//case "AverageCost":
					//case "BatchQuantity":
					//case "CalculatorUnitFactor":
					//case "CostPriceNew":
					//case "CostPriceStandard":
					//case "Depth":
					//case "FreeNumberField_01":
					//case "FreeNumberField_02":
					//case "FreeNumberField_03":
					//case "FreeNumberField_04":
					//case "FreeNumberField_05":
					//case "FreeNumberField_06":
					//case "FreeNumberField_07":
					//case "FreeNumberField_08":
					//case "GrossWeight":
					//case "Length":
					//case "Margin":
					//case "NetWeight":
					//case "Width":
					//case "StatisticalNetWeight":
					//case "StatisticalQuantity":
					//case "StatisticalUnits":
					//case "StatisticalValue":
					//case "WeightFactor":
					//	value = RawCellMapper.AsFloat(value);
					//	break;
			}
			return $"{value}\n";
		}
	}
}