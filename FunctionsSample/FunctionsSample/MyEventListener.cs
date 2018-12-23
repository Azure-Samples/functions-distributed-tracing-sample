using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;

namespace FunctionsSample
{
    public class MyEventListener : EventListener
    {
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            var memberNameIndex = eventData.PayloadNames.IndexOf("memberName");

            var message = new StringBuilder();
            for (var i = 0; i < eventData.Payload.Count; i++)
            {
                if (i == memberNameIndex) continue;
                if (i > 0)
                {
                    message.Append(", ");
                }

                message.Append(eventData.PayloadNames[i] + "=" + eventData.Payload[i]);
            }

            var last = eventData.Payload.Last().ToString();

            if (string.IsNullOrWhiteSpace(last)) return;
            var content = message.ToString();

            Debug.WriteLine(message);
        }
    }
}
