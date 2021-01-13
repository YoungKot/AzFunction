using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace FirstFunction
{
    public class TableStorage : TableEntity
    {
        public string Log { get; set; }
    }
}
