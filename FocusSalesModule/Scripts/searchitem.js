var Focus8WAPI = {
    ENUMS: {
        MODULE_TYPE: {
            MASTER: 1,
            TRANSACTION: 2,
            UI: 3,
            GLOBAL: 4
        },

        REQUEST_TYPE: {
            GET: 1,
            SET: 2,
            CONTINUE: 3
        },

        REQUEST_TYPE_UI: {
            SET_POPUP_COORDINATE: 1,
            OPEN_POPUP: 2,
            CLOSE_POPUP: 3,
            GOTOHOMEPAGE: 4,
            OPEN_INVOICE_DESIGNER: 5,
            AWAKE_SESSION: 6,
            LOGOUT: 7
        }
    },

    getFieldValue: function (sCallbackFn, Field, iModuleType, isFieldId, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: 0,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.GET,
                objData: { fieldid: Field },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, false) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getFieldValue " + err.message);
        }
    },

    setFieldValue: function (sCallbackFn, Field, Value, iModuleType, isFieldId, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: 0,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.SET,
                objData: { fieldid: Field, value: Value },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, false) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.setFieldValue " + err.message);
        }
    },

    getBodyFieldValue: function (sCallbackFn, Field, iModuleType, isFieldId, iRowIndex, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: iRowIndex,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.GET,
                objData: { fieldid: Field },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, true) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getBodyFieldValue " + err.message);
        }
    },

    setBodyFieldValue: function (sCallbackFn, Field, Value, iModuleType, isFieldId, iRowIndex, iRequestId, bStruct) {
        var obj = null;

        try {
            obj = {
                moduleType: iModuleType,
                rowIndex: iRowIndex,
                isFieldId: isFieldId,
                requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.SET,
                objData: { fieldid: Field, value: Value },
                iRequestId: iRequestId,
                sCallbackFn: sCallbackFn,
                bStruct: bStruct
            };

            if (Focus8WAPI.PRIVATE.isValidInput(obj, true) == true) {
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
        }
        catch (err) {
            alert("Exception: Focus8WAPI.setBodyFieldValue " + err.message);
        }
    },

    continueModule: function (iModuleType, result) {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = iModuleType;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.CONTINUE;
            obj.result = result;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.continueModule " + err.message);
        }
    },

    openPopup: function (url, sCallback) {
        var obj = null;

        try {
            if (Focus8WAPI.PRIVATE.isNullOrEmpty(url, true) == true) {
                return (false);
            }

            obj = {};
            obj.URL = url;
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.OPEN_POPUP;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.openPopup " + err.message);
        }

        return (true);
    },

    closePopup: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.CLOSE_POPUP;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.closePopup " + err.message);
        }
    },

    gotoHomePage: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.GOTOHOMEPAGE;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.gotoHomePage " + err.message);
        }
    },

    logout: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.LOGOUT;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.logout " + err.message);
        }
    },

    awakeSession: function () {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.AWAKE_SESSION;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.awakeSession " + err.message);
        }
    },

    setPopupCoordinates: function (sLeft, sTop, sWidth, sHeight) {
        var obj = null;
        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.SET_POPUP_COORDINATE;
            obj.Left = sLeft;
            obj.Top = sTop;
            obj.Width = sWidth;
            obj.Height = sHeight;
            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.openPopup " + err.message);
        }

        return (true);
    },

    getGlobalValue: function (sCallbackFn, sVariable, iRequestId) {
        var obj = null;

        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.GLOBAL;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.GET;
            obj.Variable = sVariable;
            obj.iRequestId = iRequestId;
            obj.sCallbackFn = sCallbackFn;

            Focus8WAPI.PRIVATE.postMessage(obj);
        }
        catch (err) {
            alert("Exception: Focus8WAPI.getGlobalValue " + err.message);
        }
    },

    openInvoiceDesigner: function (sCallbackFn, LayoutId, iVouchertype, iHeaderId, eModuleType, HeaderGroup, iSubReportId, bSaveHTMLSource, iRequestId) {
        var obj = null;
        try {
            obj = {};
            obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
            obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.OPEN_INVOICE_DESIGNER;
            obj.LayoutId = LayoutId;
            obj.iVouchertype = iVouchertype;
            obj.iHeaderId = iHeaderId;
            obj.ModuleType = eModuleType;
            obj.HeaderGroup = HeaderGroup;
            obj.iSubReportId = iSubReportId;
            obj.bSaveHTMLSource = bSaveHTMLSource;
            obj.sCallbackFn = sCallbackFn;
            obj.iRequestId = iRequestId;
            Focus8WAPI.PRIVATE.postMessage(obj);
            return obj;
        }
        catch (err) {
            alert("Exception: Focus8WAPI.openPopup " + err.message);
        }
    },

    PRIVATE: {
        isValidInput: function (obj, bBodyField) {
            try {
                if (Focus8WAPI.PRIVATE.isValidObject(obj.moduleType) == false || obj.moduleType.toString() == "") {
                    alert("Validation Exception: Please pass Module Type parameter");

                    return (false);
                }

                if (Focus8WAPI.PRIVATE.isValidObject(obj.isFieldId) == false || obj.isFieldId.toString() == "") {
                    alert("Validation Exception: Please pass isFieldId parameter");

                    return (false);
                }

                if (Focus8WAPI.PRIVATE.isValidObject(obj.objData.fieldid) == false) {
                    alert("Validation Exception: Please pass Field parameter");

                    return (false);
                }
                else {
                    if (Array.isArray(obj.objData.fieldid) == true) {
                        if (obj.objData.fieldid.length == 0) {
                            alert("Validation Exception: Please pass Field parameter");

                            return (false);
                        }
                    }
                }


                if (bBodyField == true) {
                    if (Focus8WAPI.PRIVATE.isValidObject(obj.rowIndex) == false || isNaN(obj.rowIndex)) {
                        alert("Validation Exception: Row Index should be number type");

                        return (false);
                    }

                    if (obj.rowIndex == 0) {
                        alert("Validation Exception: Row Index should be greater than 0 for Body Fields");

                        return (false);
                    }
                }
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.isValidInput} " + err.message);
            }

            return (true);
        },

        postMessage: function (obj) {
            try {
                obj.FromClient = true;
                window.parent.postMessage(obj, "*");
            }
            catch (err) {
                alert("Exception: Focus8WAPI.PRIVATE.postMessage " + err.message);
            }
        },

        onReceiveMessage: function (evt) {
            var objReturnData = null;
            var obj = null;

            try {
                Focus8WAPI.PRIVATE.stopKeyProcess(evt);
                objReturnData = evt.data;

                // Client                
                if (Focus8WAPI.PRIVATE.isValidObject(objReturnData.FromClient) == true) {
                    return;
                }

                console.log('Focus8WAPI::Received Response: ', JSON.stringify(objReturnData));

                if (Focus8WAPI.PRIVATE.isNullOrEmpty(objReturnData.sCallbackFn, true) == false) {
                    obj = {};
                    obj.returnCode = objReturnData.response.lValue;
                    obj.message = objReturnData.response.sValue;
                    obj.data = objReturnData.response.data;
                    obj.fieldId = objReturnData.fieldId;
                    obj.requestType = objReturnData.requestType;
                    obj.moduleType = objReturnData.moduleType;
                    obj.iRequestId = objReturnData.iRequestId;

                    if (Focus8WAPI.PRIVATE.isValidObject(objReturnData.RowsInfo) == true) {
                        obj.RowsInfo = objReturnData.RowsInfo;
                    }

                    eval(objReturnData.sCallbackFn)(obj);
                }
            }
            catch (err) {
                alert("Exception: Focus8WAPI.PRIVATE.onReceiveMessage " + err.message);
            }
        },

        isValidObject: function (obj) {
            try {
                if (typeof obj == "undefined" || obj == null) {
                    return (false);
                }

                return (true);
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.isValidObject} " + err.message);
            }

            return (false);
        },

        isNullOrEmpty: function (sValue, bTrim) {
            var bResult = false;

            try {
                if (Focus8WAPI.PRIVATE.isValidObject(sValue) == false || (typeof sValue).toLowerCase() != "string" || sValue.length <= 0) {
                    return (true);
                }

                if (Focus8WAPI.PRIVATE.isValidObject(bTrim) == true && bTrim == true) {
                    if (sValue.trim().length == 0) {
                        return (true);
                    }
                }
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.isNullOrEmpty} " + err.message);
                bResult = true;
            }

            return (bResult);
        },

        stopKeyProcess: function (evt) {
            try {
                if (Focus8WAPI.PRIVATE.isValidObject(evt) == false) {
                    return;
                }

                if (evt.preventDefault) {
                    evt.preventDefault();
                }
                else {
                    evt.returnValue = false;
                }

                if (evt.bubbles == true) {
                    evt.stopPropagation();
                }
            }
            catch (err) {
                alert("Exception: {Focus8WAPI.PRIVATE.stopKeyProcess} " + err.message);
            }
        }
    }

}
window.addEventListener('message', Focus8WAPI.PRIVATE.onReceiveMessage);
var initialRequestId = 0;
var requestsProcessed = [];
var requestId = 0;
var companyid = 0;
var sessionid = "";
var vtype = 0;
var docno = "";
var totalRows = 0;
let shadowItemList = [];
let validRows = 0;
var lineRequestsProcessed = [];
var lineRequestId = 0;
function resetDefaults() {

    lineRequestsProcessed = [];
    shadowItemList = [];
    requestId = 0;
    companyid = 0;
    sessionid = "";
    vtype = 0;
    docno = "";
    totalRows = 0;
   shadowItemList = [];
    validRows = 0;
}
function isRequestProcessed(iRequestId) {

    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}

function isLineRequestProcessed(iRequestId) {

    for (let i = 0; i < lineRequestsProcessed.length; i++) {
        if (lineRequestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function initRmaSearch(response) {

    console.log("Initial log ...",response.iRequestId)
    //if (initialRequestId != 0) {
    //    resetDefaults()
    //}
    //initialRequestId = response.iRequestId;
    //resetDefaults();
    ++requestId;

    Focus8WAPI.getFieldValue("getRma", ["", "DocNo", "RmaSearch","Outlet"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);
}
function clearSearchField(clearField)
{
    ++requestId;

    Focus8WAPI.setFieldValue("afterLineAdded", [clearField], [""], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);
    if (clearField == "RmaSearch") {

        setRmaSearchFocus();
    }
   
}
function setRmaSearchFocus() {
    // Find the label with text "Rma Search"
    const label = Array.from(document.querySelectorAll("label"))
        .find(l => l.textContent.trim() === "Rma Search");

    if (label) {
        const inputId = label.getAttribute("for");
        const input = document.getElementById(inputId);
        if (input) {
            input.focus();
        }
    }

}
function getRma(response) {
    if (isRequestProcessed(response.iRequestId)) {
        return;
    }
 
    requestsProcessed.push(response.iRequestId);

    validRows = response.data[0].RowsInfo.iValidRows;
    
    shadowItemList = [];
    lineRequestsProcessed = [];
    
    companyid = response.data[0].CompanyId;
    sessionid = response.data[0].SessionId;
    vtype = response.data[0].iVoucherType;
    docno = response.data[1].FieldValue;
    let rmano = response.data[2].FieldValue;
    let outletid = response.data[3].FieldValue;
    if (rmano.trim().length == 0) {

        console.log("Initiated rma trigger but rma field is empty !!");
        return;
    }

    if (outletid == 0) {

        alert("Select outlet to continue !!! ");
        return;
    }
    searchRma(rmano, outletid)

}
async function searchRma(rmano, outletid)
{
    try
    {
       
        let url = `/focussalesmodule/api/sales/rmaitems/?compid=${companyid}&outletid=${outletid}&rmano=${rmano}`;
        let response = await fetch(url);

        if (!response.ok) {
            throw new Error(`Server error: ${response.status}`);
        }

        let dataObj = await response.json();

        console.log(dataObj);

        if (dataObj.result == -1) {
            alert(dataObj.message);

            return;
        }
        else {
            clearSearchField("RmaSearch");
            console.log("Valid rows", validRows);
            if (validRows == 0) {
                console.log("Setting row no",)
                
                setLineItemsToDoc(1, dataObj.data)
                
            }
            else {
                console.log("Retrieving existing",)
                getExistingItems(dataObj.data)
            }
            
        }


        }
        catch (err) {

            alert("Error when searching Rma: " + err);
    
        }
        finally {
            
        }

}
var loadedItem = null;

function getExistingItems(item) {
    loadedItem = item;
    lineRequestsProcessed = [];
    console.log(`Processing for Rma No:`, item.RmaNo);
    console.log("Getting data from focus, valid Rows ", validRows);
    for (i = 0; i < validRows; i++) {
        console.log(`Getting row ${i}`, validRows);
        Focus8WAPI.getBodyFieldValue("getDocBodyData", ["", "Item", "Unit", "RMA", "Quantity", "Rate", "Gross"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i + 1, i + 1);
        //After last row
        //problem area
        
    }
}
function getDocBodyData(response) {

    console.log("Logging row no", response.iRequestId);
    if (isLineRequestProcessed(response.iRequestId)) {
        console.log("Row no already exists, returning", response.iRequestId);
        return;
    }

    lineRequestsProcessed.push(response.iRequestId);
    console.log("Adding data for row no:", response.iRequestId);
    var docNo = response.data[1].FieldValue;
    var companyid = response.data[0].CompanyId;
    var sessid = response.data[0].SessionId;
    var vtype = response.data[0].iVoucherType;
    //"Item", "Unit", "RMA", "Quantity", "Rate", "Gross"
    let payload = {

        "compid": companyid, "vtype": vtype, "sessid": sessid, "docno": docNo, "Item": response.data[1].FieldValue, "Unit": response.data[2].FieldValue, "RMA": response.data[3].FieldValue, "Qty": response.data[4].FieldValue, "Rate": response.data[5].FieldValue, "Gross": response.data[6].FieldValue
    };
    shadowItemList.push(payload);
    console.log("Valid rows", response.data[0].RowsInfo.iValidRows);
    console.log("Request Id", response.iRequestId, "Valid rows", validRows)
    if (response.iRequestId == validRows) {

        console.log("Valid rows: ", validRows, "Reading lines completed");
        
            console.log(shadowItemList);
            //check if rma exists
            // let rmaExists = false;

            /*"Item": response.data[1].FieldValue, "Unit": response.data[2].FieldValue, "RMA": response.data[3].FieldValue, "Qty": response.data[4].FieldValue, "Rate": response.data[5].FieldValue, "Gross": response.data[6].FieldValue*/
            //Find Item in shadow list
            let itemExists = shadowItemList.find(x => x.Item == loadedItem.Id);
            if (itemExists) {
                let rmaExists = itemExists.RMA.find(x => x == loadedItem.RmaNo);
                console.log(rmaExists);
                let rowNo = shadowItemList.indexOf(itemExists) + 1;
                if (rmaExists) {
                    
                    alert("RMA already exists !!")
                    console.log("Rma Item", rmaExists);
                    console.log("Current Item", loadedItem);
                    let newQty = itemExists.RMA.length;
                    Focus8WAPI.setBodyFieldValue("afterLineAdded", ["RMA", "Quantity"], [itemExists.RMA, newQty], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, rowNo, requestId);
                    //clearSearchField();
                    return;
                }
                else {
                    //clearSearchField();
                    console.log("updating existing row !!");
                    ++requestId;
                    
                    itemExists.RMA.push(loadedItem.RmaNo);
                    let newQty = itemExists.RMA.length;
                    Focus8WAPI.setBodyFieldValue("afterLineAdded", ["RMA", "Quantity"], [itemExists.RMA, newQty], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, rowNo, requestId);


                }

            }
            else {
                console.log("Adding new line for rma item");
                setLineItemsToDoc(validRows + 1, loadedItem)
            }

       
    }

}
function setLineItemsToDoc(rowNo, item) {

    ++requestId;
    let gross = parseInt(item.Qty) * parseFloat(item.Price);
    Focus8WAPI.setBodyFieldValue("afterLineAdded", ["Item", "Unit", "Quantity", "Rate", "Gross", "RMA"], [item.Id, item.UnitId, item.Qty, item.Price, gross, item.RmaNo], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, rowNo, requestId);
}
function afterLineAdded(response) {
}
//Remove item RMA
let validRemovalRows = 0;
let removalRmaNo = "";
let removalRequestsProcessed = [];
let removalRequestId = 0;

function initRemoveRma(response) {

    console.log("Initial remove RMA ...", response.iRequestId)
    ++removalRequestId;

    Focus8WAPI.getFieldValue("getRemoveRmaList", ["", "DocNo", "RemoveRma", "Outlet"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, removalRequestId);
}

function isRemoveRequestProcessed(iRequestId) {

    for (let i = 0; i < removalRequestsProcessed.length; i++) {
        if (removalRequestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function getRemoveRmaList(response) {

    if (isRemoveRequestProcessed(response.iRequestId)) {
        return;
    }
    validRemovalRows = response.data[0].RowsInfo.iValidRows;
    removalRequestsProcessed = [];
    companyid = response.data[0].CompanyId;
    sessionid = response.data[0].SessionId;
    vtype = response.data[0].iVoucherType;
    docno = response.data[1].FieldValue;
    let rmano = response.data[2].FieldValue;
    let outletid = response.data[3].FieldValue;
    if (rmano.trim().length == 0 || validRemovalRows == 0) {

        console.log("Initiated rma trigger but rma field is empty or no valid rows!!");
        return;
    }
    getRemovalItems(rmano)
}
function getRemovalItems(rmano) {
    removalRmaNo = rmano;
    for (i = 0; i < validRows; i++) {
        console.log(`Getting row ${i}`, validRows);
        Focus8WAPI.getBodyFieldValue("removeRmaData", ["", "Item", "Unit", "RMA", "Quantity", "Rate", "Gross"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i + 1, i + 1);
        //After last row
        //problem area

    }
}


function removeRmaData(response) {

    console.log("Logging row no", response.iRequestId);
    if (isRemoveRequestProcessed(response.iRequestId)) {
        console.log("Row no already exists, returning", response.iRequestId);
        return;
    }

    removalRequestsProcessed.push(response.iRequestId);
    let rmaList = response.data[3].FieldValue; 
    rmaExists = rmaList.find(x => x == removalRmaNo);
    if (rmaExists) {
        console.log("Rma to remove found")
        rmaList = rmaList.filter(x => x != removalRmaNo);
        ++requestId;
        let newQty = rmaList.length;
        if (rmaList.length == 0) {
            console.log("Removing Item")
            Focus8WAPI.setBodyFieldValue("afterLineAdded", [ "Item", "Unit", "RMA", "Quantity", "Rate", "Gross"], ["0","0",[],0,0,0], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, response.iRequestId, requestId);
        }
        else {

            Focus8WAPI.setBodyFieldValue("afterLineAdded", ["RMA", "Quantity"], [rmaList, newQty], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, response.iRequestId, requestId);
        }
       
    }
    clearSearchField("RemoveRma");
}