using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace DVMetadataCWAs
{
          public class GetFormsForTable : IPlugin
          {

               [Input("TableLogicalName")]
               [RequiredArgument]
               public InArgument<string> TableLogicalName { get; set; }

               [Output("TableForms")]
               public OutArgument<string> TableForms { get; set; }



               public void Execute(IServiceProvider serviceProvider)
               {
                    IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                    {
                         EntityReference targetEntity = (EntityReference)context.InputParameters["Target"];

                         // Use FetchXML to retrieve forms for the specified entity
                         string fetchXml = $@"
                    <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='systemform'>
                        <attribute name='name' />
                        <attribute name='formxml' />
                        <filter type='and'>
                          <condition attribute='objecttypecode' operator='eq' value='{targetEntity.LogicalName}' />
                        </filter>
                      </entity>
                    </fetch>";

                         EntityCollection results = service.RetrieveMultiple(new FetchExpression(fetchXml));

                         foreach (Entity formEntity in results.Entities)
                         {
                              string formName = formEntity.GetAttributeValue<string>("name");
                              string formXml = formEntity.GetAttributeValue<string>("formxml");

                              // Do something with the form details (formName and formXml) here
                         }
                    }
               }
          }
     }
     





          }
}

}
