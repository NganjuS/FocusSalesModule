var posBaseUrl = `/focussalesmodule/`
var requestId = 0;
var requestsProcessed = [];
function isRequestProcessed(iRequestId) {

    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}
function focusSessionUpdater(callbackFunc) {
    ++requestId;
    Focus8WAPI.getGlobalValue(callbackFunc, "", requestId);
}
function setMemberDetails(response) {

    if (isRequestProcessed(response.iRequestId)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    let sessionid = response.data.SessionId;

    const alpineComponent = document.getElementById('full-container')._x_dataStack[0];
    alpineComponent.postMember(sessionid);

}
function posSystem() {
        return {
            compid: 0,
            sessionid: "",
            isDisabled: false,
            docno: "",
            vtype : 0,
            outletid : 0,
            paymentModes: [],
            totalInvoiceAmt: 0,
            totalPaid : 0,
            discountVoucherList: [],
            onlinePaymentList : [],
            paymentTypes: {

                Cash: 1, Bank: 2,  Integration :3,  DiscountVoucher : 4 ,  CreditNote : 5
            },
            allowedBankTableView: [],
            allowedBankTableView: [],
            outstandingAmt: 0,
            init() {

                this.compid = this.$refs.compid.value;
                this.outletid = this.$refs.outletid.value;
                this.docno = this.$refs.docno.value;
                this.vtype = this.$refs.vtype.value;
                this.totalInvoiceAmt = this.getAmount(this.$refs.netamt.value);
                this.outstandingAmt = this.getAmount(this.$refs.netamt.value);
                Focus8WAPI.awakeSession();
                this.sessionid = this.$refs.sessionid.value;
                this.loadPaymentModes();
            },
            setPendingAmount(paymentMode) {

                let pendingAmt = this.totalInvoiceAmt - this.getOustandingAmt();
                paymentMode.Amount = pendingAmt <= 0 ? 0 : pendingAmt;
                this.updateTotals(paymentMode);

            },
            saveTxn() {

                let totalPayment = this.getOustandingAmt();
                if (totalPayment != this.totalInvoiceAmt) {
                    this.showAlertMessage("Paid amount should be equal to sale amount !!", "warning");
                    return;
                }
                this.isDisabled = true;
                let mainUrl = `${posBaseUrl}api/sales/postpayment/?compid=${this.compid}&vtype=${this.vtype}&sessionid=${this.sessionid}&docno=${this.docno}`;

                fetch(mainUrl, {
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(this.paymentModes)
                }).then(async response => {
                    this.isDisabled = false;
                    if (!response.ok) {
                        const errorText = await response.text();
                        console.log(errorText);
                        throw new Error(errorText);
                    }
                    return response.json();

                }).then(dataObj => {

                    if (dataObj.result == 1) {

                        this.showAlertMessage(dataObj.message, "success");
                        setTimeout(() => {
                            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                            Focus8WAPI.closePopup();
                           
                        }, 2000);
                    }
                    else {
                        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                        this.showAlertMessage(dataObj.message, "warning");
                        //alert(dataObj.message);
                    }

                }).catch(error => {
                    console.log(error);
                });


            },
            async updatePaymentDetails(paymentMode, selectObj) {

                try {
                    let outstandingAmt = this.totalInvoiceAmt - this.getOustandingAmt();
                    if (paymentMode.TypeSelect == this.paymentTypes.Integration) {

                        console.log("Clicked payment mode", paymentMode);
                        this.showProgress(`Searching ...`)
                        let url = `${posBaseUrl}api/salespayments/validatesale?compid=${this.compid}&outletid=${this.outletid}&manualvalidate=${paymentMode.ManualValidate}&reference=${paymentMode.Reference}&outstandingamt=${outstandingAmt}`;
                       
                        let response = await fetch(url);
                        let dataObj = await response.json();
                        if (dataObj.result == -1) {

                            this.showAlertMessage(dataObj.message, "warning");
                        }

                        if (dataObj.data != null) {                       
                            paymentMode.Amount = dataObj.data.Amount;
                            paymentMode.Reference = dataObj.data.TransactionReference;
                            paymentMode.TransactionTime = dataObj.data.TransactionTime;
                            
                            //this.onlinePaymentList.push(dtObj);
                            this.updateOtherPayments(paymentMode, {});
                        }

                        this.hideProgress();
                    }
                    
                }
                catch (error) {

                    console.log(error);
                }

                            
            },
            addDiscountAmount(paymentMode, discount) {

                if (discount.IsSelected) {

                    paymentMode.Amount = discount.Amount;
                    paymentMode.Reference = discount.Reference;
                    this.updateOtherPayments(paymentMode, discount);
                }
                else {

                    paymentMode.PayList = paymentMode.PayList.filter(item => item.Reference !== discount.Reference);
                    this.outstandingAmt = this.totalInvoiceAmt - this.getOustandingAmt();
                    this.totalPaid = this.getOustandingAmt();
                }


            },
            addOnlinePaymentAmt(paymentMode, onlinePay) {

                if (onlinePay.IsSelected) {

                    paymentMode.Amount = onlinePay.Amount;
                    paymentMode.Reference = onlinePay.Reference;
                    this.updateOtherPayments(paymentMode, onlinePay);
                }
                else {

                    paymentMode.PayList = paymentMode.PayList.filter(item => item.Reference !== onlinePay.Reference);
                    this.outstandingAmt = this.totalInvoiceAmt - this.getOustandingAmt();
                }


            }
            ,
            updateTotals(paymentMode) {
                

                if (paymentMode.TypeSelect == this.paymentTypes.Cash) {
                    let totalReceived = 0;
                    for (let i = 0; i < this.paymentModes.length; i++) {

                        totalReceived += this.paymentModes[i].PayList.reduce((sum, item) => sum + item.Amount, 0);

                    }
                    

                    if (totalReceived + paymentMode.Amount > this.totalInvoiceAmt) {
                        this.showAlertMessage("Amount will exceed document total !!", "warning");
                        paymentMode.Amount = 0;
                        this.outstandingAmt = this.totalInvoiceAmt - totalReceived;
                        return;
                        
                    }
                    else {

                        this.outstandingAmt = this.totalInvoiceAmt - (totalReceived + paymentMode.Amount);
                        this.totalPaid = this.getOustandingAmt() ;
                    }

                    
                }
                
                
            },
            allowManualValidation(paymentMode) {
                if (paymentMode.ManualValidate) {

                    paymentMode.ManualValidate = false;
                    return;
                }
                Swal.fire({
                    title: "Admin Verification",
                    input: "password",
                    showCancelButton: true
                }).then(result => {
                    if (result.isConfirmed) {
                        if (result.value == 'admin12') {

                            paymentMode.ManualValidate = true;
                        }
                        else {
                            paymentMode.ManualValidate = false;
                            this.showAlertMessage("Invalid Password !!", "warning");
                        }
                        
                    }
                    else if (result.isDismissed) {
                        paymentMode.ManualValidate = false;
                    }
                });
            },
            updateOtherPayments(paymentMode, tableLineData) {


                if (paymentMode.AllowedRows == paymentMode.PayList.length) {
                    this.showAlertMessage("Maximum allowed entries reached !!", "warning");
                    tableLineData.IsSelected = false;
                    return;
                }
                if (paymentMode.ShowReference && !paymentMode.Reference)
                {
                    this.showAlertMessage("Reference is required !!", "warning");
                    tableLineData.IsSelected = false;
                    return;
                }
                let amount = this.getAmount(paymentMode.Amount)
                if (amount <= 0)
                {
                    this.showAlertMessage("Enter a valid amount !!", "warning");
                    tableLineData.IsSelected = false;
                    return;
                }
                if (paymentMode.ShowReference) {
                    let referenceExists = paymentMode.PayList.some(obj => obj.Reference == paymentMode.Reference);

                    if (referenceExists) {
                        this.showAlertMessage("Reference already exists", "warning");
                        tableLineData.IsSelected = false;
                        return;
                    }
                }

                let currentAmt = this.getOustandingAmt();
                
                if (currentAmt + amount > this.totalInvoiceAmt) {

                    this.showAlertMessage("Adding the current amount will exceed the current sale total", "warning");
                    tableLineData.IsSelected = false;
                    return;
                }
                let actualObj = Object.assign({}, paymentMode);
                let payObj = {
                    Amount: amount, Reference: actualObj.Reference, TransactionTime: actualObj.TransactionTime
                }
                paymentMode.Amount = 0;
                paymentMode.Reference = "";

                paymentMode.PayList.push(payObj);

                this.outstandingAmt = this.totalInvoiceAmt - (currentAmt + amount);
                this.totalPaid = this.getOustandingAmt() ;

            },
            getOustandingAmt() {

                let totalReceived = 0;
                for (let i = 0; i < this.paymentModes.length; i++) {

                    if (this.paymentModes[i].TypeSelect == this.paymentTypes.Cash) {

                        totalReceived += this.paymentModes[i].Amount;

                    }
                    else {

                        totalReceived += this.paymentModes[i].PayList.reduce((sum, item) => sum + item.Amount, 0);
                    }
                   
                }
           
                
                return totalReceived;
            },
            removeRowItem(paymentMode, index) {

                paymentMode.PayList.splice(index,1); 
                this.outstandingAmt = this.totalInvoiceAmt - this.getOustandingAmt();
                this.totalPaid = this.getOustandingAmt();
            },
            loadPaymentModes()
            {
                let url = `${posBaseUrl}api/sales/outletpaymenttypes/?compid=${this.compid}&vtype=${this.vtype}&docno=${this.docno}`;

                fetch(url).then(async response => {
                    if (!response.ok) {
                        const errorText = await response.text();
                        throw new Error(errorText);
                    }
                    return response.json();
                }).then(dataObj => {

                    console.log(dataObj);
                    if (dataObj.result == 1) {

                        if (dataObj.data.PaymentTypes.length == 0) {
                            this.showAlertMessage("No payment modes found for this outlet !!");
                        }
                        this.paymentModes = dataObj.data.PaymentTypes;
                        for (let i = 0; i < this.paymentModes.length; i++) {
                            this.paymentModes[i].Amount = 0;
                            this.paymentModes[i].Reference = "";
                            this.paymentModes[i].PayList = [];
                            this.paymentModes[i].ManualValidate = false;
                        }
                        for (let i = 0; i <
                            dataObj.data.DiscountVouchers.length; i++) {

                            let loadedDiscObj = dataObj.data.DiscountVouchers[i];

                            if (loadedDiscObj.DiscountValue > 0) {

                                var discObj = {
                                    IsSelected: false,
                                    Reference: loadedDiscObj.Code,
                                    Amount: loadedDiscObj.DiscountValue
                                }
                                this.discountVoucherList.push(discObj);
                                console.log(this.discountVoucherList);
                            }

                            
                        }

                        if (this.paymentModes.length > 0 && this.paymentModes[0].TypeSelect == this.paymentTypes.Integration) {

                            this.updatePaymentDetails(this.paymentModes[0])
                        }
                    }
                    else {
                        this.showAlertMessage(dataObj.message,"warning");
                    }

                }).catch(error => {
                    console.log(error);
                    //
                });
              },
            showAlertMessage(mssg, status) {

                 Swal.fire(
                     {
                         title: 'Message', text: mssg, icon: status, 
              
                 });
            },
            closePopup() {

                Focus8WAPI.closePopup();
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            },
            showProgress(messg = 'Please wait ') {

                $('#full-container').waitMe({

                    effect: 'facebook',
                    text: messg,
                    bg: 'rgba(255, 255, 255, 0.7)',
                    color: '#000',
                    maxSize: '',
                    waitTime: -1,
                    textPos: 'vertical',
                    fontSize: '',
                    source: '',
                    onClose: function () { }
                });

            },
            hideProgress() {

                $('#full-container').waitMe("hide");
            },
            getAmount(num) {
                return isNaN(parseFloat(num)) ? 0.00 : parseFloat(num);
            },
            formatNum(val) {
                    return (val).toLocaleString("en", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            },
            getAccountName(paymentMode) {

                switch (paymentMode.TypeSelect) {
                    case 1:
                        return paymentMode.DefaultCashAccountName;
                    case 2:
                        return paymentMode.DefaultBankAccountName;
                    case 3:
                        return paymentMode.DefaultOnlineAccountName;
                    case 4:
                        return paymentMode.DefaultDiscountAccountName;
                    case 5:
                        return paymentMode.DefaultCreditNoteAccountName;
                    default:
                        return paymentMode.DefaultAccountName;
                }
            }


   }
}
