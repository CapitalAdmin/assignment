using Capital.CRM.Codes.Shared;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capital.CRM.Codes.Plugins.Account
{
	public sealed class DummyPlugin 
	{
		public void Execute(IServiceProvider serviceProvider)
		{

			
			//tracing service for debugging sandbox plguins
			ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

			// Obtain the execution context from the service provider.
			IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
			tracingService.Trace("ValidateAccount Plugin : " + context.MessageName.ToLower());

			if (context.MessageName.ToLower() == PluginAttributes.messageCreate && context.InputParameters.Contains(PluginAttributes.Target) && context.InputParameters[PluginAttributes.Target] is Entity)
			{
				Entity entity = (Entity)context.InputParameters[PluginAttributes.Target];

				if (entity.LogicalName == EntityName.Account)
				{
					tracingService.Trace("ValidateAccount Plugin : Account");
					//throw new InvalidPluginExecutionException("hello!!!"+ context.MessageName.ToLower());
				}
			}
		}
	}
}
