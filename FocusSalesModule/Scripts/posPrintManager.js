var MAX_RETRIES = 3;
var WAIT_SECONDS = 2;
qz.websocket.setClosedCallbacks([]);
qz.websocket.setErrorCallbacks([]);

async function initSilentPrint(CompId, Vtype, SessionId, DocNo) {
    try {

        let printUrl = `/focussalesmodule/printdocument/silentprint/?compid=${CompId}&vtype=${Vtype}&sessionid=${SessionId}&docno=${DocNo}`;

        let response = await fetch(printUrl);
        let dataObj = await response.json();
        ///console.log(dataObj);
        if (dataObj.result > 0 && dataObj.data.length > 0) {

            let base64Pdf = dataObj.data[0].Attachment;
 
            //let isQzRunning = await isQZTrayRunning();
           
            if (!qz.websocket.isActive()) {

                setCertificate();


                await qz.websocket.connect({
                    host: 'localhost',
                    usingSecure: true,
                    port: { insecure: [8182] },
                    retries: MAX_RETRIES,
                    delay: WAIT_SECONDS
                }).then(async () => {

                    startPrintProcess(base64Pdf);

                }).catch(err => {
                  ;
                    resetPrintCount(CompId, Vtype, DocNo);
                    console.warn(`QZ connection failed `, err);
                    alert("QZ Tray is NOT running, Install it to print : " + err);



                    //if (retryCount < maxRetries) {

                    //} else {
                    //    showInstallPrompt();
                    //    throw err;
                    //}
                });
            }
            else {
                startPrintProcess(base64Pdf);
            }

        }
        else {

            alert("Error when printing: " + dataObj.message);
        }
    }
    catch (err) {
        console.warn("QZ Tray not available:", err);

    } finally {
        //if (qz.websocket.isActive()) {
        //    await qz.websocket.disconnect();
        //}
    }
}
function setCertificate() {

    qz.security.setCertificatePromise(function (resolve, reject) {
        fetch("/focussalesmodule/printdocument/getpubliccert", { cache: 'no-store', headers: { 'Content-Type': 'text/plain' } })
            .then(function (r) {
                console.log(r);
                r.ok ? resolve(r.text()) : reject(r.text());
            });
    });

    qz.security.setSignatureAlgorithm("SHA512");

    qz.security.setSignaturePromise(function (toSign) {
        return function (resolve, reject) {
            fetch("/focussalesmodule/printdocument/sign?request=" + encodeURIComponent(toSign), { cache: 'no-store', headers: { 'Content-Type': 'text/plain' } })
                .then(function (data) { data.ok ? resolve(data.text()) : reject(data.text()); });
        };
    });

}
async function resetPrintCount(CompId, Vtype,  DocNo) {
    let printUrl = `/focussalesmodule/printdocument/ResetPrintCount/?compid=${CompId}&vtype=${Vtype}&docno=${DocNo}`;
    let response = await fetch(printUrl);
    let dataObj = await response.json();

    if (dataObj.result != 1) {
        alert("Error when reversing print " + dataObj.message);
    }
}
async function startPrintProcess(base64Pdf) {

    const printer = await qz.printers.getDefault();
    const config = qz.configs.create(printer);
    const data = [{ type: 'pixel', format: 'pdf', flavor: 'base64', data: base64Pdf }];
    return qz.print(config, data);
}
async function isQZTrayRunning() {
    try {

        qz.websocket.setClosedCallbacks(function () { }); // override with empty
        qz.websocket.setErrorCallbacks(function () { });  
        await qz.websocket.connect({ retries: 5, delay: 1 });
        console.log("QZ Tray is running");
        return true;
    } catch (err) {
        alert("QZ Tray is NOT running: " + err);
        return false;
    }
}
//function connectQZ() {
//    return qz.websocket.connect()
//        .then(() => {
//            console.log("Connected to QZ Tray");
//        })
//        .catch(err => {
//            retryCount++;

//            console.warn(`QZ connection failed (attempt ${retryCount})`, err);

//            if (retryCount < maxRetries) {
//                return new Promise(resolve => {
//                    setTimeout(() => resolve(connectQZ()), 1000); // retry after 1s
//                });
//            } else {
//                showInstallPrompt();
//                throw err;
//            }
//        });
//}