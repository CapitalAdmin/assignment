using Capital.CRM.Codes.Shared;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;

namespace Capital.CRM.Codes.WFlows.Account
{
	public sealed class AutoAccountNumber:CodeActivity
    {

		[RequiredArgument]
		[Input("AutoNumber")]
		[ReferenceTarget("cpa_autonumber")]
		public InArgument<EntityReference> autonumber { get; set; }
		protected override void Execute(CodeActivityContext activitycontext)
		{
			ITracingService tracingService = activitycontext.GetExtension<ITracingService>();

			//Create the context
			IWorkflowContext context = activitycontext.GetExtension<IWorkflowContext>();
			IOrganizationServiceFactory serviceFactory = activitycontext.GetExtension<IOrganizationServiceFactory>();
			IOrganizationService orgService = serviceFactory.CreateOrganizationService(context.UserId);

			tracingService.Trace("Retrieve autonumber ID");
			Guid autonumberId = this.autonumber.Get(activitycontext).Id;


			RetrieveRequest request = new RetrieveRequest();
			request.ColumnSet = new ColumnSet(new string[] { AutoAttributes.AcctNumberIncrementBy, AutoAttributes.AutoAccountNumber, AutoAttributes.AutoSortCode, AutoAttributes.SotCodeIncrementBy });
			request.Target = new EntityReference(EntityName.Autonumber, autonumberId);

			//Retrieve the entity to determine what the birthdate is set at
			Entity autoEntity = (Entity)((RetrieveResponse)orgService.Execute(request)).Entity;
			tracingService.Trace("Retrieve autonumber ID completed");

			if (autoEntity.Attributes.Contains(AutoAttributes.AutoAccountNumber))
			{
				tracingService.Trace("update account ");
				Entity updateEntity = new Entity(EntityName.Account);
				updateEntity.Id = context.PrimaryEntityId;
				updateEntity[AccountAributes.AccountNumber] = autoEntity.GetAttributeValue<int>(AutoAttributes.AutoAccountNumber);
				updateEntity[AccountAributes.SortCode] = autoEntity.GetAttributeValue<int>(AutoAttributes.AutoSortCode);
				orgService.Update(updateEntity);
				tracingService.Trace("update account completed");

				tracingService.Trace("update auto number ");
				updateEntity = new Entity(EntityName.Autonumber);
				updateEntity.Id = autoEntity.Id;
				updateEntity[AutoAttributes.AutoAccountNumber] = autoEntity.GetAttributeValue<int>(AutoAttributes.AutoAccountNumber) + autoEntity.GetAttributeValue<int>(AutoAttributes.AcctNumberIncrementBy);
				updateEntity[AccountAributes.SortCode] = autoEntity.GetAttributeValue<int>(AutoAttributes.AutoSortCode) + autoEntity.GetAttributeValue<int>(AutoAttributes.SotCodeIncrementBy);
				orgService.Update(updateEntity);
				tracingService.Trace("update auto number completed ");


			}

		}
	}
}
