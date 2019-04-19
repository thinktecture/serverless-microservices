package com.thinktecture.serverless.functions;

import java.time.*;
import com.microsoft.azure.functions.annotation.*;
import com.microsoft.azure.functions.*;

/**
 * Azure Functions with Timer trigger.
 */
public class KeepItWarm {
    /**
     * This function will be invoked periodically according to the specified schedule.
     */
    @FunctionName("KeepItWarm")
    public void run(
        @TimerTrigger(name = "timerInfo", schedule = "0 */9 * * * *")
        String timerInfo,
        final ExecutionContext context
    ) {
        context.getLogger().info("Java Timer trigger function executed at: " + LocalDateTime.now());
    }
}
