IF OBJECT_ID(N'fsm_TemporaryAdvancePayments', N'U') IS NULL create table fsm_TemporaryAdvancePayments (  Id int IDENTITY PRIMARY KEY, DocumentTagId varchar(200),DocNo varchar(50), ReferenceNo varchar(300),Vtype int,CustomerId int)

IF OBJECT_ID(N'fsm_TemporaryPayments', N'U') IS NULL 
create table fsm_TemporaryPayments ( Id int IDENTITY PRIMARY KEY, DocumentTagId varchar(200), ProbableDocNo varchar(50), Vtype int,CustomerId int, MemberId int,OutletId int, CostCenterId int,DocDate int, LoginId int ,IsValidated bit,Amount decimal(20,4), Reference  varchar(150), PaymentMethodId int , PaymentType int , ShowReference  bit,ShowBank bit,SelectedAccount int, TxnDate Datetime, CreatedOn Datetime)

IF OBJECT_ID(N'fsm_TemporaryPaymentsList', N'U') IS NULL 
create table fsm_TemporaryPaymentsList (  Id int IDENTITY PRIMARY KEY, TempPaymentId int,  Code varchar(50), Reference varchar(50), Amount decimal(20,4), TxnDate Datetime);
