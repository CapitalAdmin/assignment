using Capital.CRM.Codes.Shared;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Globalization;
using System.ServiceModel;

namespace Capital.CRM.Codes.Plugins.Account
{
	public class ValidateAccount:IPlugin
	{
		public void Execute(IServiceProvider serviceProvider)
		{
			//tracing service for debugging sandbox plguins
			ITracingService tracingService =(ITracingService)serviceProvider.GetService(typeof(ITracingService));

			// Obtain the execution context from the service provider.
			IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
			
			if (context.MessageName.ToLower()==PluginAttributes.messageCreate && context.InputParameters.Contains(PluginAttributes.Target) && context.InputParameters[PluginAttributes.Target] is Entity)
            {
				Entity entity = (Entity)context.InputParameters[PluginAttributes.Target];

				if (entity.LogicalName == EntityName.Account)
				{
					tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "ValidateAccount Plugin : Executing for Account."));

					try
					{
						var query = new QueryExpression(EntityName.Account)
						{
							ColumnSet = new ColumnSet(AccountAributes.Name),							
						};

						#region [ name and address condition]
						FilterExpression nameandAddress = new FilterExpression(LogicalOperator.And);
						nameandAddress.AddCondition("name", ConditionOperator.Equal, entity.GetAttributeValue<string>("name"));
						tracingService.Trace("ValidateAccount Plugin : Executing for Account."+ entity.GetAttributeValue<string>("name"));
						if (entity.Attributes.Contains("address1_city"))
						{
							nameandAddress.AddCondition("address1_city", ConditionOperator.Equal, entity.GetAttributeValue<string>("address1_city"));
							tracingService.Trace("ValidateAccount Plugin : Executing for Account." + entity.GetAttributeValue<string>("address1_city"));
						}
						if (entity.Attributes.Contains("address1_line1"))
						{
							nameandAddress.AddCondition("address1_line1", ConditionOperator.Equal, entity.GetAttributeValue<string>("address1_line1"));
							tracingService.Trace("ValidateAccount Plugin : Executing for Account." + entity.GetAttributeValue<string>("address1_line1"));
						}
						//if (entity.Attributes.Contains("address1_state"))
						//	query.Criteria.AddCondition("address1_state", ConditionOperator.Equal, entity.GetAttributeValue<string>("address1_state"));
						if (entity.Attributes.Contains("address1_postalcode"))
						{
							nameandAddress.AddCondition("address1_postalcode", ConditionOperator.Equal, entity.GetAttributeValue<string>("address1_postalcode"));
							tracingService.Trace("ValidateAccount Plugin : Executing for Account." + entity.GetAttributeValue<string>("address1_postalcode"));
						}
						if (entity.Attributes.Contains("address1_country"))
						{
							nameandAddress.AddCondition("address1_country", ConditionOperator.Equal, entity.GetAttributeValue<string>("address1_country"));
							tracingService.Trace("ValidateAccount Plugin : Executing for Account." + entity.GetAttributeValue<string>("address1_country"));
						}
						#endregion

						#region [OR phone condition ]
						FilterExpression phoneCondition = new FilterExpression(LogicalOperator.And);
						if (entity.Attributes.Contains("telephone1"))
						{
							phoneCondition.AddCondition("telephone1", ConditionOperator.Equal, entity.GetAttributeValue<string>("telephone1"));
							tracingService.Trace("ValidateAccount Plugin : Executing for Account." + entity.GetAttributeValue<string>("telephone1"));

						}
						#endregion

						#region [email condition ]
						FilterExpression emailCondition = new FilterExpression(LogicalOperator.And);
						if (entity.Attributes.Contains("emailaddress1"))
						{
							emailCondition.AddCondition("emailaddress1", ConditionOperator.Equal, entity.GetAttributeValue<string>("emailaddress1"));
							tracingService.Trace("ValidateAccount Plugin : Executing for Account." + entity.GetAttributeValue<string>("emailaddress1"));

						}
						#endregion


						FilterExpression accountFilters = new FilterExpression(LogicalOperator.Or);
						accountFilters.AddFilter(nameandAddress);
						accountFilters.AddFilter(phoneCondition);
						accountFilters.AddFilter(emailCondition);
						query.Criteria = accountFilters;

						IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
						IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
						tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "ValidateAccount Plugin : Org service is created."));

						EntityCollection results = service.RetrieveMultiple(query);
						tracingService.Trace(string.Format(CultureInfo.InvariantCulture,"ValidateAccount Plugin : Retrieved existing accounts."));
						if (results.Entities!=null && results.Entities.Count > 0)
							throw new InvalidPluginExecutionException("An Account with same information (Name,address or phone or email) is available in the system. ");

					}
					catch (FaultException<OrganizationServiceFault> ex)
					{
						throw new InvalidPluginExecutionException("Account validation Error: "+ex.Message);
					}

					catch (Exception ex)
					{
						tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "ValidateAccount Plugin Error: {0}", ex.Message));
						throw new InvalidPluginExecutionException("Account creation Error: " + ex.Message); ;
					}
				}
			}
		}
	}

}
