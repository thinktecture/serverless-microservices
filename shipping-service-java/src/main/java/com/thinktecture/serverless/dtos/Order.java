package com.thinktecture.serverless.dtos;

import java.util.Date;
import java.util.List;
import java.util.UUID;

public class Order {
   public UUID Id;
   public String Description;
   public Date Created;
   public List<OrderItem> Items;
}