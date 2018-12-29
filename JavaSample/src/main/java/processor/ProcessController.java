package processor;

import java.io.UnsupportedEncodingException;

import org.springframework.http.HttpEntity;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class ProcessController {
    private static final String template = "Parent Request-Id: %s";
    @RequestMapping("/process")
    public Message process(HttpEntity<byte[]> requestEntity) throws UnsupportedEncodingException {
        //TODO Support W3C Trace Context
        String parentRequestId = requestEntity.getHeaders().getFirst("Request-Id");
        System.out.println(String.format(template, parentRequestId));
        return new Message(String.format(template, parentRequestId));

        //TODO Dependency Telemetry Traking
        //TODO Send ServiceBus Queue
    }
}