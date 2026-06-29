IF OBJECT_ID(N'fsm_PaymentsReservations', N'U') IS NULL CREATE TABLE fsm_PaymentsReservations (
    Id           int IDENTITY PRIMARY KEY,
    Reference    varchar(150) NOT NULL,
    DocNo        varchar(150) NOT NULL,
    Amount       decimal(18,2) NOT NULL,
    ReservedAt   datetime2 NOT NULL,
    IsConfirmed  bit NOT NULL DEFAULT 0,
    INDEX ix_ref (Reference)
);