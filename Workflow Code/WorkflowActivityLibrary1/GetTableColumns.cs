using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;
using DVMetadataCWAs.Models;
//using Microsoft.Xrm.Sdk.Workflow;



namespace DVMetadataCWAs
{
     public class GetTableColumns : CodeActivity
     {

          [Input("TableLogicalName")]
          [RequiredArgument]
          public InArgument<string> TableLogicalName { get; set; }

          [Output("TableColumns")]
          public OutArgument<string> OutputProperty { get; set; }


          protected override void Execute(CodeActivityContext wfContext)
          {
               IWorkflowContext context = wfContext.GetExtension<IWorkflowContext>();
               IOrganizationServiceFactory serviceFactory = wfContext.GetExtension<IOrganizationServiceFactory>();
               IOrganizationService service = serviceFactory.CreateOrganizationService(context.InitiatingUserId);

               string _logicalName = TableLogicalName.Get(wfContext);
               List<ColumnData> columnsInfo = RetrieveColumns(service, _logicalName);

               // Build JSON manually
               StringBuilder jsonBuilder = new StringBuilder();
               jsonBuilder.Append("[");
               bool isFirstItem = true;
               foreach (ColumnData column in columnsInfo)
               {
                    if (!isFirstItem)
                    {
                         jsonBuilder.Append(",");
                    }
                    jsonBuilder.Append($"{{\"ColumnName\":\"{EscapeJsonString(column.ColumnName)}\"," +
                                       $"\"DataType\":\"{EscapeJsonString(column.DataType)}\"," +
                                       $"\"DisplayName\":\"{EscapeJsonString(column.DisplayName)}\"," +
                                       $"\"ReferencedTable\":\"{EscapeJsonString(column.ReferencedTable)}\"}}");
                    isFirstItem = false;
               }
               jsonBuilder.Append("]");

               // Set the JSON string to the output property
               wfContext.SetValue(this.OutputProperty, jsonBuilder.ToString());
          }

          public List<ColumnData> RetrieveColumns(IOrganizationService service, string tableName)
          {
               List<ColumnData> columnsInfo = new List<ColumnData>();
               // Create the request to retrieve entity metadata
               RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest
               {
                    LogicalName = tableName,
                    EntityFilters = EntityFilters.All
               };

               // Execute the request
               RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);

               // Retrieve entity metadata
               EntityMetadata entityMetadata = retrieveEntityResponse.EntityMetadata;

               foreach (AttributeMetadata attribute in entityMetadata.Attributes)
               {
                    string columnName = attribute.LogicalName;
                    string columnType = GetAttributeDataType(attribute);
                    string referencedTable = null;
                    string displayName = attribute.DisplayName.LocalizedLabels?.FirstOrDefault()?.Label ?? " ";


                   

                    // Handle Lookup attribute type
                    if (attribute.AttributeType == AttributeTypeCode.Lookup)
                    {
                         LookupAttributeMetadata lookupAttribute = (LookupAttributeMetadata)attribute;
                         foreach (string targetEntityName in lookupAttribute.Targets)
                         {
                              referencedTable = targetEntityName;
                         }
                    }

                    // Add column data to the list
                    columnsInfo.Add(new ColumnData
                    {
                         ColumnName = columnName,
                         DataType = columnType,
                         ReferencedTable = referencedTable,
                         DisplayName = displayName
                    });
               }

               return columnsInfo;
          }

          private string GetAttributeDataType(AttributeMetadata attr)
          {
               var attributeTypeCode = attr.AttributeType.Value;
               var ReferencedTable = "";
               switch (attributeTypeCode)
               {
                    case AttributeTypeCode.Boolean:
                         return "Two Options (Boolean)";
                    case AttributeTypeCode.DateTime:
                         return "Date and Time";
                    case AttributeTypeCode.Decimal:
                         return "Decimal Number";
                    case AttributeTypeCode.Double:
                         return "Floating Point Number";
                    case AttributeTypeCode.Integer:
                         return "Whole Number";
                    case AttributeTypeCode.Memo:
                         return "Multiple Lines of Text";
                    case AttributeTypeCode.Money:
                         return "Currency";
                    //   case AttributeTypeCode.OptionSet:
                    //        return "Option Set";
                    case AttributeTypeCode.Picklist:
                         return "Picklist";
                    case AttributeTypeCode.State:
                         return "State";
                    case AttributeTypeCode.Status:
                         return "Status";
                    case AttributeTypeCode.String:
                         return "Single Line of Text";
                    case AttributeTypeCode.Uniqueidentifier:
                         return "Unique Identifier";
                    case AttributeTypeCode.BigInt:
                         return "Big Integer";
                    case AttributeTypeCode.ManagedProperty:
                         return "Managed Property";
                    case AttributeTypeCode.EntityName:
                         return "Entity Name";
                    case AttributeTypeCode.Virtual:
                         return "Virtual";
                    case AttributeTypeCode.CalendarRules:
                         return "Calendar Rules";
                    case AttributeTypeCode.Lookup:
                         LookupAttributeMetadata lookupAttribute = (LookupAttributeMetadata)attr;
                         foreach (string targetEntityName in lookupAttribute.Targets)
                         {
                              ReferencedTable += "[" + targetEntityName + "]";
                         }
                         return "Lookup";
                              //+ ReferencedTable;

                    case AttributeTypeCode.Customer:
                         return "Customer";
                    case AttributeTypeCode.Owner:
                         return "Owner";
                    // Add more cases for other data types as needed
                    default:
                         return "Unknown";
               }
          }



          // Helper method to escape special characters in JSON strings
          private string EscapeJsonString(string input)
          {
               if (input == null)
               {
                    return null;
               }
               return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
          }


     }
}
     



