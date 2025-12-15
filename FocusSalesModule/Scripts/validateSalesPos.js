var posBaseUrl = `/focussalesmodule/`
var postRequestId = 0;
var postRequestsProcessed = [];
var postValidRows = 0;
var postShadowItemList = [];
var linepostRequestsProcessed = [];
var isProcessing = false; 


var paymentHeaderObj = {

    LoginId: 0, Vtype: 0, CompId: 0, SessionId: "", DocNo: "", DocDate: 0, OutletId: 0, CostCenterId: 0, CustomerId: 0, MemberId: 0
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

    console.log(isProcessing);
    if (isProcessing) {
        return;
    }

    isProcessing = true
    resetUI();
   
    console.log(response)
    console.log("Called open pop up")
    console.log("postRequestId", postRequestId);
    console.log("Request Obj", postRequestsProcessed);
    ++postRequestId;
    Focus8WAPI.getFieldValue("getDocumentAmount", ["", "DocNo", "Date", "Outlet", "Cost Center", "CustomerAC", "Member"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, postRequestId);
    
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
    paymentHeaderObj.OutletId = response.data[3].FieldValue;
    paymentHeaderObj.CostCenterId = response.data[4].FieldValue;
    paymentHeaderObj.CustomerId = response.data[5].FieldValue;
    paymentHeaderObj.MemberId = response.data[6].FieldValue;

    if (paymentHeaderObj.CustomerId == 0) {
        alert("Select customer to continue !!!!");

        isProcessing = false;
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
        return;
    }
    if (paymentHeaderObj.OutletId == 0) {

        alert("Select outlet to continue !!!!");
        isProcessing = false;
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
       
        return;
    }
    if (paymentHeaderObj.CostCenterId == 0) {
        alert("Select cost center to continue !!!!");
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
  
    if (postValidRows == 0) {
        
        alert("Enter products to continue !!!!");
        isProcessing = false;
        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
        return;
    }

    for (i = 0; i < postValidRows; i++) {

        Focus8WAPI.getBodyFieldValue("getDocPostData", ["", "Item", "Unit", "RMA", "Quantity", "Rate", "Gross", "Discount"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i + 1, i + 1);
        //After last row
        //problem area

    }

}
function getDocPostData(response) {


    if (isPostLineRequestProcessed(response.iRequestId)) {

        return;
    }

    linepostRequestsProcessed.push(response.iRequestId);

    

    //"Item", "Unit", "RMA", "Quantity", "Rate", "Gross"
    let payload = {
        "Item": response.data[1].FieldValue, "Unit": response.data[2].FieldValue, "RMA": response.data[3].FieldValue, "Qty": validateInt(response.data[4].FieldValue), "Rate": validateDecimal(response.data[5].FieldValue), "Gross": validateDecimal(response.data[6].FieldValue), "Discount": validateDecimal(response.data[7].FieldValue)
    };
    postShadowItemList.push(payload);

    if (response.iRequestId == postValidRows) {
        console.log("Finished loading items, continuing to post");
        let amount = validateDecimal(postShadowItemList.reduce((sum, item) => sum + item.Gross, 0));

        if (amount <= 0) {
            alert("Sale amount not valid !!");
            isProcessing = false;
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
            return;
        }
        for (let i = 0; i < postShadowItemList.length; i++) {

            if (validateDecimal(postShadowItemList[i].Rate) <= 0) {
                alert(`Rate required for item on line ${i + 1} !! `);
                isProcessing = false;
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                return;
            }
        }
        let mainUrl = `${posBaseUrl}/posscreen/openposbeforesave/?compid=${paymentHeaderObj.CompId}&vtype=${paymentHeaderObj.Vtype}&outletid=${paymentHeaderObj.OutletId}&memberid=${paymentHeaderObj.MemberId}&sessionid=${paymentHeaderObj.SessionId}&docno=${paymentHeaderObj.CompId}&amount=${amount}`

        console.log(mainUrl);
        Focus8WAPI.openPopup(mainUrl);


    }
    //else {

    //    alert("Could not proceed unable to get items");
    //    ++postRequestId;
    //    isProcessing = false;
    //    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
    //}

}
function setDocumentIdentifier(strval) {
    ++postRequestId;

    Focus8WAPI.setFieldValue("afterIdentifierAdded", ["DocumentTagId"], [strval], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, postRequestId);
   /* Focus8WAPI.getFieldValue("getDocumentAmount", ["", "DocNo", "Date", "Outlet", "Cost Center", "CustomerAC", "Member"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, postRequestId);*/

}
function afterIdentifierAdded(response) {

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

function validateDecimal(num) {

    return isNaN(parseFloat(num)) ? 0.00 : parseFloat(num);
}
function validateInt(num) {

    return isNaN(parseInt(num)) ? 0.00 : parseInt(num);
}