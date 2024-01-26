using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVMetadataCWAs.Models
{
     // Data structure to hold column information
     public class ColumnData
     {
          public string ColumnName { get; set; }
          public string DataType { get; set; }
          public string ReferencedTable { get; set; }
          
          public string DisplayName { get; set; }
     }
}
