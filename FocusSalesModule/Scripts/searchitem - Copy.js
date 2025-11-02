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

var requestsProcessed = [];
var requestId = 0;
var companyid = 0;
var sessionid = "";
var vtype = 0;
var docno = "";
var totalRows = 0;
var shadowItemList = [];
function isRequestProcessed(iRequestId) {

    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function initRmaSearch(response) {
    ++requestId;

    Focus8WAPI.getFieldValue("getRma", ["", "DocNo", "RmaSearch","Outlet"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);
}
function getRma(response) {
    if (isRequestProcessed(response.iRequestId)) {
        return;
    }
 
    requestsProcessed.push(response.iRequestId);
    console.log(response.data);
    companyid = response.data[0].CompanyId;
    sessionid = response.data[0].SessionId;
    vtype = response.data[0].iVoucherType;
    docno = response.data[1].FieldValue;
    let rmano = response.data[2].FieldValue;
    let outletid = response.data[3].FieldValue;

    if (outletid == 0) {

        alert("Select outlet to continue !!! ");
        return;
    }


    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "/focussalesmodule/content/sweetalert2.min.css";
    document.head.appendChild(link);


    const script = document.createElement("script");
    script.src = "/focussalesmodule/scripts/sweetalert2.all.min.js";
    script.onload = () => {
        Focus8WAPI.closePopup();
        openPopup(rmano);
    };
    document.body.appendChild(script);

}
async function openPopup(rmano)
{
    try
    {
        displaySearch(rmano)
        let url = `/focussalesmodule/api/sales/rmaitems/?compid=${companyid}&rmano=${rmano}`;
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
        setTableRows(dataObj.datalist);

        }
        catch (err) {

            alert("Unable when loading Rma: " + err);

        }
        finally {
            
        }

}
async function getRmaData() {

    try {
        let inputsearch = document.getElementById("searchitem");
        if (inputsearch == null) {
            return;
        }
        let rmano = document.getElementById("searchitem").value;
        console.log(rmano);

        let url = `/focussalesmodule/api/sales/rmaitems/?compid=${companyid}&rmano=${rmano}`;
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
        setTableRows(dataObj.datalist);


    }
    catch (err) {

        console.log(err);
        alert("Unable when loading Rma: " + err);

    }
    finally {

    }

}
function displaySearch(rmano) {

       
    let htmltext = `${getStyling()}
        <div class="sa-grid sa-gutter-sm">
        <div class="sa-col">
            <label style="margin-left : 10px;"><b>Rma No:</b> <label>
                <input value="${rmano}" id="searchitem" type="text" style="width: 200px; height: 40px;font-size: 14px;" />
                <button style="margin-left: 5px;padding: 10px;" id="btnSearchRma">Search</button>
        </div>
        </div>
        <div class="sa-grid sa-gutter-sm">
        <div class="sa-col">
                ${getTable()}
        </div>
        </div>
    `;

    

    Swal.fire(
        {
            width: '800px',
            html: htmltext,
            showDenyButton: false,
            showCancelButton: true,
            allowOutsideClick: false,
            confirmButtonColor: "#333333",
            cancelButtonText: `<div><svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle" viewBox="0 0 16 16">
  <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14m0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16"/>
  <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708"/>
</svg><br/>Cancel</div>`,

            confirmButtonText: `<div>
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-floppy2" viewBox="0 0 16 16">
  <path d="M1.5 0h11.586a1.5 1.5 0 0 1 1.06.44l1.415 1.414A1.5 1.5 0 0 1 16 2.914V14.5a1.5 1.5 0 0 1-1.5 1.5h-13A1.5 1.5 0 0 1 0 14.5v-13A1.5 1.5 0 0 1 1.5 0M1 1.5v13a.5.5 0 0 0 .5.5H2v-4.5A1.5 1.5 0 0 1 3.5 9h9a1.5 1.5 0 0 1 1.5 1.5V15h.5a.5.5 0 0 0 .5-.5V2.914a.5.5 0 0 0-.146-.353l-1.415-1.415A.5.5 0 0 0 13.086 1H13v3.5A1.5 1.5 0 0 1 11.5 6h-7A1.5 1.5 0 0 1 3 4.5V1H1.5a.5.5 0 0 0-.5.5m9.5-.5a.5.5 0 0 0-.5.5v3a.5.5 0 0 0 .5.5h1a.5.5 0 0 0 .5-.5v-3a.5.5 0 0 0-.5-.5z"/>
</svg>
            <br/>Save</div>`,
            preConfirm: () => {
                
              
            }
        }
    ).then(result => {
        if (result.isConfirmed) {
            console.log("Selected rows:", result.value);

            Swal.close();
        }
    });
    var searchBar = document.getElementById("searchitem");
    if (searchBar != null) {

        searchBar.onchange = getRmaData(companyid);
    }
    var searchBtn = document.getElementById("btnSearchRma");
    if (searchBtn != null) {
        console.log("Clicked search button!!");
        searchBtn.onclick = getRmaData;
    }
    
}

function setTableRows(datalist) {
    let tbody = document.getElementById("dataList");
    tbody.innerHTML = '';
    let rowStrList = "";
    for (let i = 0; i < datalist.length; i++) {

        rowStrList += ` <tr><td style="display: none;">${datalist[i].Id}</td>
                      <td style="padding: 12px 15px;
border-bottom: 1px solid #ddd;">${datalist[i].Name}</td>
 <td style="padding: 12px 15px;
border-bottom: 1px solid #ddd;">${datalist[i].RmaNo}</td>
<td  style="padding: 12px 15px;
border-bottom: 1px solid #ddd;">${datalist[i].Price}</td><td  style="padding: 12px 15px;
border-bottom: 1px solid #ddd;">${datalist[i].Stock}</td><td style="padding: 12px 15px;
border-bottom: 1px solid #ddd;">
 <button style="margin-left: 5px;padding: 10px;" id="btnAddItem">Add</button>
</td>
                  </tr>`

        

    }

    tbody.innerHTML = rowStrList;
    var btnAdd = document.getElementById("btnAddItem");
    if (btnAdd != null) {

        btnAdd.onclick = addItemToScr;
    }
   

}
function getHeaderDetails(response) {
    ++requestId;
    shadowItemList = [];
    Focus8WAPI.getFieldValue("getExistingItems", ["", "DocNo", "RmaSearch", "Outlet"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);
}
let validRows = 0;
function getExistingItems(response) {

    if (isRequestProcessed(response.iRequestId)) {
        return;
    }

    requestsProcessed.push(response.iRequestId);

    for (i = 0; i < response.data[0].RowsInfo.iValidRows; i++) {
        ++requestId;
        Focus8WAPI.getBodyFieldValue("getDocBodyData", ["", "Item", "Unit", "RMA", "Quantity", "Rate", "Gross"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, i + 1, i);
    }
}
function getDocBodyData(response) {

    if (isRequestProcessed(response.iRequestId)) {
        return;
    }

    requestsProcessed.push(response.iRequestId);

    if (response.iRequestId == response.data[0].RowsInfo.iValidRows) {

        lastrowid = response.data[0].RowsInfo.iValidRows;
    }

}
function addItemToScr() {

    let tbody = document.getElementById("dataList");
    let rows = tbody.querySelectorAll("tr");
    let selected = [];

    shadowItemList.push({
        ItemId: 0, ItemName: "", RmaNo: "", RmaNoList: [],Price: 0, Qty: 0
    });

    rows.forEach(row => {

            let itemId = row.cells[0].textContent;
            let rmano = row.cells[2].textContent;
            let price = row.cells[3].textContent;
        let qty = 1;

        selected.push({ ItemId: itemId, RmaNo: rmano, Price: price, Qty: qty });
        
    });


    for (i = 0; i < selected.length; i++) {
        ++requestId

        let gross = parseInt(selected[i].Qty) * parseFloat(selected[i].Price);
        Focus8WAPI.setBodyFieldValue("AddLinesToScr", ["Item", "Quantity", "Rate", "Gross", "RMA"], [selected[i].ItemId, selected[i].Qty, selected[i].Price, gross, selected[i].RmaNo], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, totalRows + 1, requestId);

    }
}



function getTable() {
    return `<table style="width: 100%;border-collapse: collapse; 
  margin: 20px 0;font-size: 14px;text-align: left;border-radius: 8px;overflow: hidden;box-shadow: 0 2px 8px rgba(0,0,0,0.1);">
                    <thead>
                    <tr>
                         <th style="display: none;">##</th>   <th style="background-color: #0073AA;color: white;
font-weight: bold;padding: 12px 15px;">Product</th>
<th style="background-color: #0073AA;color: white;
font-weight: bold;padding: 12px 15px;">Rma No</th>
<th style="background-color: #0073AA;color: white;
font-weight: bold;padding: 12px 15px;">Price</th><th style="background-color: #0073AA;color: white;
font-weight: bold;padding: 12px 15px;">Stock</th><th style="background-color: #0073AA;color: white;
font-weight: bold;padding: 12px 15px;">##</th>
                    </tr>
                    </thead>
                    <tbody id="dataList">
                    <tr><td rowspan='5' style='padding: 12px 15px;border - bottom: 1px solid #ddd;'>No data found !!</td></></tr>
                    </tbody>
            </table>`
}
function getStyling() {
    return `<style>
/* ---------- Simple Sa-Grid (self-contained for Swal) ---------- */
.sa-grid{
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); /* responsive columns */
  gap: 12px;            /* default gutter */
  align-items: start;
  width: 100%;
  box-sizing: border-box;
  margin: 0;
  padding: 0;
  font-family: system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial;
}
.sa-col{
  background: #fbfdff;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid rgba(20,30,50,0.06);
  box-shadow: 0 1px 0 rgba(0,0,0,0.02) inset;
  font-size: 14px;
  line-height: 1.3;
  box-sizing: border-box;
}

/* Utilities */
.sa-gutter-sm{ gap: 8px; }
.sa-gutter-lg{ gap: 18px; }
.sa-center{ text-align: center; }
.sa-strong{ font-weight: 600; }

/* Column span helpers (use sparingly) */
.sa-span-2{ grid-column: span 2; }
.sa-span-3{ grid-column: span 3; }

/* Stack to single column on narrow modals */
@media (max-width: 420px){
  .sa-grid{ grid-template-columns: 1fr; }
  .sa-span-2, .sa-span-3 { grid-column: auto; }
}

/* Small helper for label/value rows inside a column */
.sa-row{ display:flex; justify-content:space-between; gap:8px; align-items:center; }
.sa-label{ color: #586069; font-size:13px; }
.sa-value{ font-weight:600; color:#111827; font-size:13px; }
</style>`;
}