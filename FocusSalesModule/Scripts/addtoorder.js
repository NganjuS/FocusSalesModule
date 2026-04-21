var requestId = 0;
var requestFooterId = 0;
var requestsProcessed = [];
var scrItemList = [];
var loadedItemsArr = [];
function isRequestProcessed(iRequestId) {

    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function getCurrentRows() {

    ++requestId;
    Focus8WAPI.getFieldValue("addItemsToScreen", ["", "DocNo"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);
    setFooterDetailsScreen();
}
var validRows = 0;
function addItemsToScreen(response) {
    if (isRequestProcessed(response.iRequestId)) {
        return;
    }

    requestsProcessed.push(response.iRequestId);

    validRows = response.data[0].RowsInfo.iValidRows;
    let companyid = response.data[0].CompanyId;
    let sessionid = response.data[0].SessionId;
    let vtype = response.data[0].iVoucherType;
    let docno = response.data[1].FieldValue;

    for (let i = 0; i < scrItemList.length; i++) {
        ++requestId;
        Focus8WAPI.setBodyFieldValue("afterLineAdded", ["Item", "Quantity", "Rate", "Discount"], [scrItemList[i].ItemId, scrItemList[i].Qty, scrItemList[i].Rate, scrItemList[i].Discount ], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, validRows+i+1, requestId);
    }
}
function setFooterDetailsScreen() {

    ++requestFooterId;
    Focus8WAPI.setFieldValue("afterFooterAdded", ["Minimum Advance", "Total Qty", "sNarration"], [scrItemList[0].MinimumAdvance, scrItemList[0].TotalQty, scrItemList[0].SetItem], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false,  requestFooterId);
}

function afterLineAdded(response) {
    //let arrLength = scrItemList.length;
    //if ((arrLength + validRows) == response.iRequestId) {
    //    scrItemList = [];
    //}

}
function afterFooterAdded(response) {
    console.log(response);

}
