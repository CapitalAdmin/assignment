using System;
using System.Collections.Generic;
using System.Text;

namespace Capital.CRM.Codes.Shared
{
	public static class EntityName
	{
		public const string Account = "account";
		public const string Autonumber = "cpa_autonumber";
		public const string Contact = "contact";
	}

	public static class AccountAributes
	{
		public const string AccountNumber = "accountnumber";
		public const string SortCode = "cpa_sortcode";
		public const string AutoNumber = "cpa_autonumber";
		public const string Name = "name";
	}
	public static class AutoAttributes
	{
		public const string AutoAccountNumber = "cpa_autoaccountnumber";
		public const string AcctNumberIncrementBy = "cpa_accountincrementby";
		public const string AutoSortCode = "cpa_sortcodeautonumber";
		public const string SotCodeIncrementBy = "cpa_sorctcodeincrementby";

	}

	public static class PluginAttributes
	{
		public const string Target = "Target";
		public const string messageCreate = "create";
		public const string messageUpdate = "update";

	}
}
