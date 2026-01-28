var posBaseUrl = `/focussalesmodule/`;
function posSystem() {
        return {
            compid: 0,
            sessionid: "",
            isDisabled: false,
            docno: "",
            vtype : 0,
            outletid : 0,
            paymentModes: [],
            totalReceivedAmt : 0,
            totalInvoiceAmt: 0,
            totalPaid : 0,
            discountVoucherList: [],
            onlinePaymentList : [],
            paymentTypes: {

                Cash: 1, Bank: 2,  Integration :3,  DiscountVoucher : 4 ,  CreditNote : 5, AdvanceReceipt : 6
            },
            integrationTypes: {

                NONE: 0,MONIEPOINT : 1, EASYBUY : 2, SENTINAL : 3
            },
            allowedBankTableView: [],
            outstandingAmt: 0,
            searchVal: "",
            currentPage: 1,
            pagingData: {
                totalItems: 1,
                pageSize: 10,
                startingPage: 0,
                visiblePages: 5,
                curVisiblePages: 1,
                totalPages: 1,
                pagingDesc: "",

            },
           
            init() {

                this.compid = this.$refs.compid.value;
                this.outletid = this.$refs.outletid.value;
                this.memberid = this.$refs.memberid.value;
                this.docno = this.$refs.docno.value;
                this.vtype = this.$refs.vtype.value;
                this.totalInvoiceAmt = this.getAmount(this.$refs.netamt.value);
                this.outstandingAmt = this.getAmount(this.$refs.netamt.value);
                Focus8WAPI.awakeSession();
                this.sessionid = this.$refs.sessionid.value;
                this.loadPaymentModes();
            },
            setAdvancePaymentAmt(paymentMode, lineObj) {


                let totalReceived = 0;
                for (let i = 0; i < this.paymentModes.length; i++) {

                    if (this.paymentModes[i].TypeSelect == this.paymentTypes.Cash) {

                        totalReceived += this.getAmount(this.paymentModes[i].Amount);
                    }
                    else {

                        let sumamt = this.paymentModes[i].PayList.filter(obj => obj.IsSelected == true).reduce((sum, item) => sum + item.Amount, 0);
                        console.log(sumamt);

                        totalReceived += sumamt;
                    }
                        

                }
                console.log("Total received: ",totalReceived),
                this.totalReceivedAmt = totalReceived;
 
            },
            loadPaymentModes() {
                try {


                    let url = `${posBaseUrl}api/sales/advancepaymenttypes/?compid=${this.compid}&vtype=${this.vtype}&outletid=${this.outletid}`;
                    this.showProgress(messg = 'Loading payment modes ')
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
                                this.paymentModes[i].Amount = '';
                                this.paymentModes[i].Reference = "";
                                this.paymentModes[i].PayList = [];
                                this.paymentModes[i].ManualValidate = false;
                            }

                            let crEnabled = this.paymentModes.some(obj => obj.TypeSelect == this.paymentTypes.CreditNote);
                            let discEnabled = this.paymentModes.some(obj => obj.TypeSelect == this.paymentTypes.DiscountVoucher);

                            if (this.paymentModes[0].TypeSelect == this.paymentTypes.Integration) {
                                this.loadOnlinePayments(this.paymentModes[0], {}, false)
                            }

                            if (crEnabled) {

                                this.loadCreditNotes();
                            }

                            if (discEnabled) {
                                this.loadDiscounts();
                            }


                        }
                        else {
                            this.showAlertMessage(dataObj.message, "warning");
                        }

                    }).catch(error => {
                        console.log(error);
                        //
                    });
                }
                catch (err) {

                    console.log(err);
                }
                finally {
                    this.hideProgress();
                }
            },
            updateBankPayments(paymentMode, lineObj) {
                var amount = this.getAmount(paymentMode.Amount);

                if (amount <= 0) {

                    this.showAlertMessage("Enter a valid amount !!", "warning");
                    return;
                }

                let referenceExists = paymentMode.PayList.some(obj => obj.Reference == paymentMode.Reference);

                if (referenceExists) {
                    this.showAlertMessage("Reference already exists", "warning");
                    tableLineData.IsSelected = false;
                    return;
                }

                let actualObj = Object.assign({}, paymentMode);
                let payObj = {

                    IsSelected: true,
                    Amount: amount, Reference: actualObj.Reference, TransactionTime: actualObj.TransactionTime
                }
                paymentMode.Amount = "";
                paymentMode.Reference = "";

                paymentMode.PayList.push(payObj);

                this.setAdvancePaymentAmt(paymentMode, lineObj);


            },
            addDiscCreditNoteAmt(paymentMode, lineObj) {
                console.log("Setting bank txn active");
                console.log(lineObj);
                
                this.setAdvancePaymentAmt(paymentMode, lineObj);

            },
           
           
            refreshData(paymentMode) {

                this.loadOnlinePayments(paymentMode, {}, true);
            },
            addOnlinePaymentAmt(paymentMode, onlinePay, payList) {

                paymentMode.PayList = payList;
                this.setAdvancePaymentAmt();

            },
            saveTxn() {

                this.setAdvancePaymentAmt();
               
                if (this.totalReceivedAmt == 0) {

                    this.showAlertMessage("No data has been entered !!", "warning");
                }
                else {

                    let finalData = Object.assign({}, this.paymentModes);
                    window.parent.setDocumentDetails(finalData);
                    
                }

               

            },
            setManualOnlineSearch(paymentMode) {

                //this.allowManualValidation(paymentMode);

                if (paymentMode.ManualValidate) {

                    paymentMode.ManualValidate = true;
                    paymentMode.PayList = [];
                               
                }
                else {
                    paymentMode.PayList = [];
                    this.loadOnlinePayments(paymentMode, {},false);
                }

            },
            async loadManualSearchPayments(paymentMode) {
                try {

                    if (paymentMode.Reference.trim().length == 0) {

                        this.showAlertMessage("Enter transaction reference to continue !!!", "warning");
                        return;
                    }
                    this.showProgress(`Searching ...`)
                    let url = `${posBaseUrl}api/salespayments/manualonlinepayment?compid=${this.compid}&&integrationtype=${paymentMode.IntegrationType}&outletid=${this.outletid}&reference=${paymentMode.Reference.trim()}`;

                    console.log(url);
                    let response = await fetch(url);
                    let dataObj = await response.json();
                    console.log(dataObj);
                    this.hideProgress();
                    paymentMode.Reference = "";

                    if (dataObj.result == 1 && dataObj.datalist.length > 0) {

                        let currentAmt = this.getOustandingAmt();
                        let amount = this.getAmount(dataObj.datalist[0].Amount);
                        let selectrec = currentAmt + amount < this.totalInvoiceAmt;

                        let nwPayList = dataObj.datalist.map(x => ({

                            Amount: amount,
                            IsSelected: selectrec,
                            Reference: x.TransactionReference,
                            TransactionTime: x.TransactionTime

                        }));

                        

                        this.addOnlinePaymentAmt(paymentMode, {}, [...paymentMode.PayList, ...nwPayList])

                    }
                    else {

                       
                        this.showAlertMessage("No payment with this reference was found !!!", "warning");
                    }

                    
                
                }
                catch (error) {

                    console.log(error);
                }
                finally {
                    this.hideProgress();
                }

            },
            async loadOnlinePayments(paymentMode, selectObj, refreshMode) {

                try {
                    
                    //let outstandingAmt = this.totalInvoiceAmt - this.getOustandingAmt();
                    if (paymentMode.TypeSelect == this.paymentTypes.Integration ) {

                        if (!refreshMode && paymentMode.PayList.length > 0) {
                            return;
                        }
                        //console.log(paymentMode);
                        this.showProgress(`Searching ...`)
                        let url = `${posBaseUrl}api/salespayments/onlinepaymentlist?compid=${this.compid}&maxmin=${paymentMode.MaxMinutes}&integrationtype=${paymentMode.IntegrationType}&outletid=${this.outletid}&pageno=${this.currentPage}&pagesize=${this.pagingData.pageSize}&searchval=${this.searchVal}`;

                        console.log(url);
                        let response = await fetch(url);
                        let dataObj = await response.json();
                        console.log(dataObj);

                        if (dataObj.result == 1) {                            
                            let nwPayList = [];
                            let dataList = dataObj.data.data;
                            for (let i = 0; i < dataList.length; i++) {

                                let amt = this.getAmount(dataList[i].Amount)
                                nwPayList.push({

                                    Amount: amt,
                                    OrigAmount: amt,
                                    IsSelected: false,
                                    Reference: dataList[i].TransactionReference,
                                    TransactionTime: dataList[i].TransactionTime

                                });
                            }

                            
                            this.pagingData.totalItems = dataObj.data.recordsTotal;
                            this.pagingData.totalPages = dataObj.data.totalPages;

                            this.addOnlinePaymentAmt(paymentMode, {}, nwPayList)
                            this.setCurVisiblePages();
                        }
         
                        this.hideProgress();
                    }

                }
                catch (error) {

                    console.log(error);
                }
                finally {
                    this.hideProgress();
                }

                            
            },          
            removeRowItem(paymentMode, index) {

                paymentMode.PayList.splice(index,1); 
                this.setAdvancePaymentAmt(paymentMode, {});
            },            
            
            async loadDiscounts() {

                console.log(window.parent.postShadowItemList);
                let itemList = window.parent.postShadowItemList.map(x => x.Item);
                console.log(itemList);
                let postObj = {
                    TxnDate: window.parent.paymentHeaderObj.DocDate,
                    Item: itemList
                };
                let url = `${posBaseUrl}api/salespayments/availablediscount/?compid=${this.compid}&vtype=${this.vtype}&outletid=${this.outletid}`;
                console.log(url);
                let response = await fetch(url, {
                        method: "POST",
                        headers: {
                            'Content-Type': 'application/json'
                            },
                    body: JSON.stringify(postObj)
                    });

                if (!response.ok) {
                    throw new Error(`Server error: ${response.status}`);
                }

                let dataObj = await response.json();
                console.log(dataObj);
                if (dataObj.result == 1) {
 
                    if (dataObj.datalist.length == 0) {
                        return;
                    }
                    var discountObj = this.paymentModes.filter(obj => obj.TypeSelect == this.paymentTypes.DiscountVoucher);

                    if (discountObj.length > 0) {
              
                        for (let i = 0; i < dataObj.datalist.length; i++) { 
                            
                            let nwOBj = {

                                ItemId: dataObj.datalist[i].ItemId,
                                IsSelected: false,
                                Reference: dataObj.datalist[i].Name,
                                Amount: this.getAmount(dataObj.datalist[i].DiscountValue),
                                OrigAmount: this.getAmount(dataObj.datalist[i].DiscountValue),
                                PostedAmt: this.getAmount(dataObj.datalist[i].PostedAmt),
                                FullAmt: this.getAmount(dataObj.datalist[i].FullAmt),
                                AccountId: dataObj.datalist[i].SelectedAccount
                            }

                            discountObj[0].PayList.push(nwOBj);
                        }

                    }
        

                }
            },
            async loadCreditNotes() {

               // console.log(window.parent.postShadowItemList);
                let itemList = window.parent.postShadowItemList.flatMap(x => x.RMA.map(String));
                
                let postObj = {
                    TxnDate: window.parent.paymentHeaderObj.DocDate,
                    RmaNoList: itemList
                };
                console.log(postObj);
                let url = `${posBaseUrl}api/salespayments/availablecreditnotes/?compid=${this.compid}&vtype=${this.vtype}&outletid=${this.outletid}&memberid=${this.memberid}`;
                console.log(url);
                let response = await fetch(url, {
                    method: "POST",
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(postObj)
                });
             
                if (!response.ok) {
                    throw new Error(`Server error: ${response.status}`);
                }

                let dataObj = await response.json();
                console.log(dataObj);
                if (dataObj.result == 1) {
                    
                    if (dataObj.datalist.length == 0) {
                        debugger;
                        return;
                    }
                    var creditNoteObj = this.paymentModes.filter(obj => obj.TypeSelect == this.paymentTypes.CreditNote);
                    debugger;
                    if (creditNoteObj.length > 0) {
                        debugger;
                        for (let i = 0; i < dataObj.datalist.length; i++) {

                            //console.log(dataObj.datalist);
                            let nwOBj = {

                                IsSelected: false,
                                Reference: dataObj.datalist[i].Reference,
                                Amount: this.getAmount(dataObj.datalist[i].Amount),
                                OrigAmount: this.getAmount(dataObj.datalist[i].Amount),
                                PostedAmt: this.getAmount(dataObj.datalist[i].PostedAmt),
                                FullAmt: this.getAmount(dataObj.datalist[i].FullAmt)
                            }

                            creditNoteObj[0].PayList.push(nwOBj);
                        }

                    }
                    console.log(creditNoteObj[0]);

                }
            },
            showAlertMessage(mssg, status) {

                 Swal.fire(
                     {
                         title: 'Message', text: mssg, icon: status, 
              
                 });
            },
            timeout: null,
            debouncedInput(paymentMode) {
                clearTimeout(this.timeout);
                this.timeout = setTimeout(() => {
                    this.onFinishedTyping(paymentMode);
                }, 500); // 500ms debounce
            },
            onFinishedTyping(paymentMode) {

                this.loadOnlinePayments(paymentMode, {}, true);
            },
            resetFilters(paymentMode) {
                this.currentPage = 1;
                this.searchVal = "";
                this.loadOnlinePayments(paymentMode, {}, true);
            },
            totalPages() {
                return Math.ceil(this.pagingData.totalItems / this.pagingData.pageSize);
            },
            setPagingDesc() {
                this.pagingData.pagingDesc = `Showing page ${this.currentPage} of ${this.pagingData.totalPages}`;
            },
            currentGroup(paymentMode,page) {

                this.currentPage = page;
                this.loadOnlinePayments(paymentMode, {}, true);
            },
            setCurVisiblePages() {
                let remainingPages = this.pagingData.totalPages - this.getCurrentGroupStart();
                if (remainingPages < this.pagingData.visiblePages) {
                    this.pagingData.curVisiblePages = remainingPages;
                }
                else {
                    this.pagingData.curVisiblePages = this.pagingData.visiblePages;
                }
                this.setPagingDesc();
            },
            getCurrentGroupStart() {

                return (Math.floor((this.currentPage - 1) / this.pagingData.visiblePages)) * this.pagingData.visiblePages;
            },

            visiblePages() {

                const start = this.getCurrentGroupStart() * 10 + 1;
                const end = Math.min(start + 9, this.pagingData.totalPages);
                return Array.from({ length: end - start + 1 }, (_, i) => start + i);
            },

            nextGroup(paymentMode) {

                if (this.currentPage < this.pagingData.totalPages) {
                    this.currentPage += 1;
                    this.pagingData.startingPage = this.getCurrentGroupStart();

                    this.setCurVisiblePages()

                }
                this.loadOnlinePayments(paymentMode, {}, true);

            },

            prevGroup(paymentMode) {

                if (this.currentPage > 1) {
                    this.currentPage -= 1;
                    this.pagingData.startingPage = this.getCurrentGroupStart();

                    this.setCurVisiblePages();
                }
                this.loadOnlinePayments(paymentMode, {}, true);

            },
            closePopup() {
                
                window.parent.onPosClosePopupStop();
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
                        return this.getOnlineAccountsName(paymentMode);
                    case 4:
                        return paymentMode.DefaultDiscountAccountName;
                    case 5:
                        return paymentMode.DefaultCreditNoteAccountName;
                    case 6:
                        return paymentMode.AdvanceReceiptAccountName;
                    default:
                        return paymentMode.DefaultAccountName;
                }
            },
            getOnlineAccountsName(paymentMode) {

                switch (paymentMode.IntegrationType) {
                    case 1:
                        return paymentMode.DefaultMoniepointAccountName;
                    case 2:
                        return paymentMode.DefaultEasyBuyAccountName;
                    case 3:
                        return paymentMode.DefaultSentinalAccountName;
                    default:
                        return paymentMode.DefaultOnlineAccountName;
                    
                }
            }

   }
}
