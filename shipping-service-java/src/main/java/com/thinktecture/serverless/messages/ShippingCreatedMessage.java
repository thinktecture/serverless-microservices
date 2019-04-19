package com.thinktecture.serverless.messages;

import java.util.Date;
import java.util.UUID;

public class ShippingCreatedMessage {
   public UUID Id;
   public Date Created;
   public UUID OrderId;
   public String UserId;
}