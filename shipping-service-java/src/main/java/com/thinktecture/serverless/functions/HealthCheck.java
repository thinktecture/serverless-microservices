package com.thinktecture.serverless.functions;

import java.util.*;
import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;

public class HealthCheck {
    @FunctionName("Ping")
    public HttpResponseMessage run(
        @HttpTrigger(name = "req", route = "ping", methods = { HttpMethod.GET }, authLevel = AuthorizationLevel.ANONYMOUS)
        HttpRequestMessage<Optional<String>> request,
        final ExecutionContext context) {
        context.getLogger().info("Ping HTTP trigger processed a request.");

        return request.createResponseBuilder(HttpStatus.OK).body("OK").build();
    }
}
