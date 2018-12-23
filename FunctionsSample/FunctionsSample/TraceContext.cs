using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionsSample
{
    

    public class TraceContext
    {
        // $AzureWebJobsTraceparent might be a name of a member. In the near future, it might be automatically tracked. 
        // https://github.com/lmolkova/azure-webjobs-sdk/commit/7a6aabe2c82fa9ba2f8aff1fd90ff00091917358
        public string ParentId { get; set; }
        // TODO W3C TraceContext
        //public string Traceparent { get; set; }
        //public string Tracestate { get; set; }
    }
}
