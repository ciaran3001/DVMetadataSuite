using DVMetadataCWAs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DVMetadataCWAs.Helpers
{
     class JSONBuilder
     {


          public string ConvertListToJSONArray<T>(List<T> ListData)
          {
               List<ColumnData> columnsInfo = null; //RetrieveColumns(service, _logicalName);

               // Build JSON manually
               StringBuilder jsonBuilder = new StringBuilder();
               jsonBuilder.Append("[");
               bool isFirstItem = true;

               Type objType = ListData.First().GetType();
               
               // Get all properties of the object using reflection
               PropertyInfo[] properties = objType.GetProperties();
               foreach(var Item in ListData) {
                    // Print the names and values of properties
                    foreach (var property in properties)
                    {
                         object propertyValue = property.GetValue(Item);
                         Console.WriteLine($"Property Name: {property.Name}, Property Value: {propertyValue}");
                    }
               }



               foreach (var column in columnsInfo)
               {    
                    
                    if (!isFirstItem)
                    {
                         jsonBuilder.Append(",");
                    }
                    jsonBuilder.Append("");
                    jsonBuilder.Append($"\"ColumnName\":\"{EscapeJsonString( column.ColumnName)}\","+
                                       $"\"DataType\":\"{EscapeJsonString(column.DataType)}\"," +
                                        $"\"DisplayName\":\"{EscapeJsonString(column.DisplayName)}\"," +
                                       $"\"ReferencedTable\":\"{EscapeJsonString(column.ReferencedTable)}\"}}");
                    isFirstItem = false;
               }
               jsonBuilder.Append("]");

               return jsonBuilder.ToString();
          }
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
