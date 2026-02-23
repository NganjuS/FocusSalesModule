var posBaseUrl = `/focussalesmodule/`
var postRequestId = 0;
var postRequestsProcessed = [];
var postValidRows = 0;
var postShadowItemList = [];
var discountVoucherList = [];
var linepostRequestsProcessed = [];
var isProcessing = false; 


var paymentHeaderObj = {

    LoginId: 0, Vtype: 0, CompId: 0, SessionId: "", DocNo: "", DocDate: 0, CostCenterId: 0, MemberId: 0, OutletId: 0
};
function resetUI() {

    postRequestId = 0;
    postRequestsProcessed = [];
    postValidRows = 0;
    postShadowItemList = [];
    linepostRequestsProcessed = [];
}

function isPostRequestProcessed(iRequestId) {

    for (let i = 0; i < postRequestsProcessed.length; i++) {
        if (postRequestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function isPostLineRequestProcessed(iRequestId) {

    for (let i = 0; i < linepostRequestsProcessed.length; i++) {
        if (linepostRequestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}

function openPOS(response) {
   
    console.log(response)
    console.log("Called open pop up")
    console.log("postRequestId", postRequestId);
    console.log("Request Obj", postRequestsProcessed);
    ++postRequestId;
    Focus8WAPI.getFieldValue("getDocumentAmount", ["", "DocNo", "Date",  "Cost Center",  "Member", "Outlet"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, postRequestId);

    
    
}

function getDocumentAmount(response) {

    if (isPostRequestProcessed(response.iRequestId)) {
        return;
    }
    postRequestsProcessed.push(response.iRequestId);

    postValidRows = response.data[0].RowsInfo.iValidRows;
    
    paymentHeaderObj.LoginId = response.data[0].LoginId;
    paymentHeaderObj.Vtype = response.data[0].iVoucherType;
    paymentHeaderObj.CompId = response.data[0].CompanyId;
    paymentHeaderObj.SessionId = response.data[0].SessionId;
    paymentHeaderObj.DocNo = response.data[1].FieldValue;
    paymentHeaderObj.DocDate = response.data[2].FieldValue;
    paymentHeaderObj.CostCenterId = response.data[3].FieldValue;
    paymentHeaderObj.MemberId = response.data[4].FieldValue;
    paymentHeaderObj.OutletId = response.data[5].FieldValue;


    if (paymentHeaderObj.OutletId == 0) {

        alert("Select outlet to continue !!!!");
        isProcessing = false;
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);

        return;
    }

    if (paymentHeaderObj.MemberId == 0) {

        alert("Select member to continue !!!!");
        isProcessing = false;
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);

        return;
    }
    let mainUrl = `${posBaseUrl}/posscreen/advancereceiptbeforesave/?compid=${paymentHeaderObj.CompId}&vtype=${paymentHeaderObj.Vtype}&memberid=${paymentHeaderObj.MemberId}&sessionid=${paymentHeaderObj.SessionId}&docno=${paymentHeaderObj.DocNo}&outletid=${paymentHeaderObj.OutletId}`

    

    discountVoucherList = [];
    Focus8WAPI.openPopup(mainUrl);

}

function setDocumentIdentifier(strval) {
    ++postRequestId;

    Focus8WAPI.setFieldValue("afterIdentifierAdded", ["DocumentTagId"], [strval], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, postRequestId);
    if (discountVoucherList.length > 0) {

        for (let i = 0; i < postShadowItemList.length; i++) {

            let itemObjList = discountVoucherList.filter(obj => obj.ItemId == postShadowItemList[i].Item)

            if (itemObjList.length > 0) {

                let discAmt = validateDecimal(itemObjList.reduce((sum, item) => sum + item.Amount, 0))
                let reference = itemObjList.map(x => x.Reference).join(",");
                ++postRequestId;
  
                Focus8WAPI.setBodyFieldValue("afterDiscountAdded", ["VoucherCode", "Voucher Discount"], [reference,discAmt], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i + 1, postRequestId);
            }
        }
    }
   
   

}
function afterIdentifierAdded(response) {

}
function afterDiscountAdded(response) {

}
function onPosClosePopupStop() {
    console.log("Popup closed !!!");
    isProcessing = false;
    ++postRequestId;
    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    Focus8WAPI.closePopup();
    
}
function onPosClosePopupContinue() {
    console.log("Popup closed !!!");
    isProcessing = false;
    ++postRequestId;
    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
    Focus8WAPI.closePopup();
    
    

}
paymentTypes = {

    Cash: 1, Bank: 2, Integration: 3, DiscountVoucher: 4, CreditNote: 5, AdvanceReceipt: 6
}
function clearData() {
    for (let i = 0; i < 20; i++) {

        ++postRequestId;
        Focus8WAPI.setBodyFieldValue("afterAdvanceLineAdded", ["Payment Type", "Account", "Amount", "ReferenceNo"], [0, 0, 0, ""], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i+1, postRequestId);
    }
    
}
function setDocumentDetails(paymentList) {
    paymentList = Object.values(paymentList);
    console.log(paymentList);
    clearData();
    let rowNo = 0;
    for (let i = 0; i < paymentList.length; i++) {

        console.log("Checking type",paymentList[i].TypeSelect)
        if (paymentList[i].TypeSelect == paymentTypes.Cash) {
            console.log("Cash found !!", paymentList[i].TypeSelect)
            if (validateDecimal(paymentList[i].Amount) > 0) {

                ++postRequestId
                ++rowNo;
                Focus8WAPI.setBodyFieldValue("afterAdvanceLineAdded", ["Payment Type", "Account", "Amount", "ReferenceNo"], [paymentList[i].iMasterId, getAccountId(paymentList[i]), paymentList[i].Amount, paymentList[i].Reference], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, rowNo, postRequestId);
            }
        }
        else {

            console.log("Other payment types !!", paymentList[i].TypeSelect)
            let curPayObjList = paymentList[i].PayList;
            for (let j = 0; j < curPayObjList.length; j++) {

                if (curPayObjList[j].IsSelected) {

                    ++postRequestId
                    ++rowNo;
                    console.log("Setting row value !!!")
                    console.log(curPayObjList[j]);
                    Focus8WAPI.setBodyFieldValue("afterAdvanceLineAdded", ["Payment Type", "Account", "Amount", "ReferenceNo"], [paymentList[i].iMasterId, getAccountId(paymentList[i]), curPayObjList[j].Amount, curPayObjList[j].Reference], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, rowNo, postRequestId);
                }
            }
        }
    }
    Focus8WAPI.closePopup();
    
   // Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
}
function afterAdvanceLineAdded(response) {

}
function validateDecimal(num) {

    return isNaN(parseFloat(num)) ? 0.00 : parseFloat(num);
}
function validateInt(num) {

    return isNaN(parseInt(num)) ? 0.00 : parseInt(num);
}
function getAccountId(paymentMode) {

    switch (paymentMode.TypeSelect) {
        case 1:
            return paymentMode.DefaultCashAccount;
        case 2:
            return paymentMode.DefaultBankAccount;
        case 3:
            return getOnlineAccountsId(paymentMode);
        case 4:
            return paymentMode.DefaultDiscountAccount;
        case 5:
            return paymentMode.DefaultCreditNoteAccount;
        case 6:
            return paymentMode.AdvanceReceiptAccount;
        default:
            return paymentMode.DefaultAccount;
    }
}
function getOnlineAccountsId(paymentMode) {

    switch (paymentMode.IntegrationType) {
        case 1:
            return paymentMode.DefaultMoniepointAccount;
        case 2:
            return paymentMode.DefaultEasyBuyAccount;
        case 3:
            return paymentMode.DefaultSentinalAccount;
        default:
            return paymentMode.DefaultOnlineAccount;

    }
}