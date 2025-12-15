using FocusSalesModule.Models;
using FocusSalesModule.Models.MoniePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FocusSalesModule.Queries
{
    public class MoneyPointQuery
    {
        public static string CreateTransactionsTable()
        {
            return "IF OBJECT_ID(N'MoniePointData', N'U') IS NULL create table MoniePointData\r\n(\r\n  Id int IDENTITY PRIMARY KEY, WebhookId varchar(200),WebhookTimestamp Datetime , Amount DECIMAL(18,4), BusinessId int,     ResponseCode varchar(100), TerminalSerial varchar(100), BusinessOwnerId int , ResponseMessage varchar(100),TransactionTime Datetime,TransactionType varchar(100), MerchantReference varchar(100),  TransactionStatus varchar(100),\r\nTransactionReference varchar(100),RetrievalReferenceNumber  varchar(100),InvoiceID varchar(100), EventId varchar(100),Domain varchar(100),ReqResource varchar(100), ResourceId varchar(100),CreatedAt Datetime,EventType varchar(100), IsAllocatedToSale bit )";
        }
        public static string InsertTransactionsTable(MoniePointRequestDTO requestDTO, string webhookid, string timestamp)
        {
            DateTime txnTime = DateTime.Parse(requestDTO.data.transactionTime);
            string txnTimeSqlFormat = txnTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            DateTime createdAt = DateTime.Parse(requestDTO.createdAt);
            string createdAtSqlFormat = txnTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            return $"insert into MoniePointData (WebhookId ,WebhookTimestamp  , Amount , BusinessId,     ResponseCode , TerminalSerial , BusinessOwnerId , ResponseMessage ,TransactionTime ,TransactionType , MerchantReference ,  TransactionStatus ,\r\nTransactionReference ,RetrievalReferenceNumber  ,InvoiceID , EventId,Domain ,ReqResource , ResourceId ,CreatedAt ,EventType ) values ('{webhookid}','{timestamp}',{requestDTO.data.amount}, {requestDTO.data.businessId},'{requestDTO.data.responseCode}','{requestDTO.data.terminalSerial}',{requestDTO.data.businessOwnerId},'{requestDTO.data.responseMessage}', '{txnTimeSqlFormat}','{requestDTO.data.transactionType}','{requestDTO.data.merchantReference}','{requestDTO.data.transactionStatus}','{requestDTO.data.transactionReference}','{requestDTO.data.retrievalReferenceNumber}','{requestDTO.data.customFields.InvoiceID}','{requestDTO.eventId}','{requestDTO.subject.domain}','{requestDTO.subject.resource}','{requestDTO.subject.resourceId}','{createdAtSqlFormat}','{requestDTO.eventType}')";
        }
    }
}