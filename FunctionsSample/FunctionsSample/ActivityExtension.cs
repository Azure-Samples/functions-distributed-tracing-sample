using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace FunctionsSample
{
    public static class ActivityExtension
    {
        public static RequestTelemetry CreateAndStartRequestTelemetry(this Activity activity)
        {
            var requestTelemetry = new RequestTelemetry {Name = activity.OperationName};
            requestTelemetry.Id = activity.Id;
            requestTelemetry.Context.Operation.Id = activity.RootId;
            requestTelemetry.Context.Operation.ParentId = activity.ParentId;
            requestTelemetry.Start();
            return requestTelemetry;
        }
    }
}
