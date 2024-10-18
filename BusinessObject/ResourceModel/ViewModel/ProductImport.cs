using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.ResourceModel.ViewModel
{
    public class ProductImport : Product
    {
        public ProductImport()
        {
            ImportInvalidErrors = new List<string>();
        }
        public List<string> ImportInvalidErrors { get; set; }
        public bool IsImported { get; set; }
    }
}
