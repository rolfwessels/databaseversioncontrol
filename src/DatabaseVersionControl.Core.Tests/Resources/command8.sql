CREATE TABLE [OrderItem2] (
      [GmvTransactionID] int NOT NULL,
      [SellableProductID] bigint NOT NULL,
      [Title] varchar(100) NOT NULL,
      [ArtistName] varchar(100) NOT NULL,
      [Value] Decimal(8,2) NOT NULL,
      [VoucherCode] varchar(20) NOT NULL,
      [CreateDate] datetime NOT NULL,
      [UpdateDate] datetime NOT NULL
      CONSTRAINT [PK__OrderItem] PRIMARY KEY CLUSTERED ([GmvTransactionID],[SellableProductID])
      )
      ON [PRIMARY]