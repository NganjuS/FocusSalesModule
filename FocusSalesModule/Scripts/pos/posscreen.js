var requestsProcessed = [];
var requestId = 0;
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
function setSession(response) {

    if (isRequestProcessed(response.iRequestId)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    let sessionid = response.data.SessionId;

    const alpineComponent = document.getElementById('main-container')._x_dataStack[0];
    alpineComponent.validateDocument(sessionid);

}
function setUserDetails(response) {

    if (isRequestProcessed(response.iRequestId)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    response.data

    const alpineComponent = document.getElementById('main-container')._x_dataStack[0];
    alpineComponent.setUserOutlet(response.data);

}
function posSystem() {
    return {
        init() {

            this.compid = this.$refs.compid.value;
            this.initSetUserOutlet();
            this.transactionDate = new Date().toISOString().split('T')[0];
            this.docNo = 'AUTO';
        },
        compid : 0,
        items: [],
        rmaSearch: '',
        cashierName: '',
        cashierPhone: '',
        branchName: '',
        activeOutlet: '',
        registerName: 'Register 1',
        showAlert: false,
        alertMessage: '',
        showRMAModal: false,
        selectedItemId: '',
        selectedItemRMAs: [],
        outletList : [],
        costCenters : [],
        // Header fields
        docNo: '',
        transactionDate: '',
        selectedOutlet: '',
        isCreditCustomer: false,
        selectedMember: '',
        customerAccount: '',
        members: [],

        // Add member modal
        showAddMemberModal: false,
        newMemberName: '',
        newMemberPhone: '',
        memberError: '',

        // Advance Receipt modal
        showAdvanceReceiptModal: false,
        advanceReceiptDate: '',
        advanceReceiptCashBankAccount: '',
        advanceReceiptOutlet: '',
        advanceReceiptCostCenter: '',
        advanceReceiptMember: '',
        advanceReceiptChequeNo: '',
        advanceReceiptCustomerAcc: '',
        advanceReceiptAmount: 0,
        isCreditCustomerReadOnly: false,
        emptyObject() {
            return {

                Id: 0, Code: "", Name: "", RmaNo : "",Unit : "", Stock : 0,Price : 0, RmaNoList : []
            }
        },
        // Settlement Modal
        showSettlementModal: false,
        activePaymentTab: 'cash',

        // Cash payment
        cashAmount: 0,

        // Bank payments
        bankPayments: [],
        moniePayments: [],
        newMoniePayment: { reference: '', amount: 0 },
        newBankPayment: { method: '', reference: '', amount: 0 },

        // Voucher
        voucherCode: '',
        voucherAmount: 0,
        voucherApplied: false,
        voucherItemCode: '', // Track which item the voucher is for

        // Credit Note
        creditNoteNumber: '',
        creditNoteAmount: 0,
        creditNoteApplied: false,

        // Mobile Money
        mobileMoneyProvider: '',
        mobileMoneyPhone: '',
        monieMoneyTransactionId: '',
        monieAmount: 0,
        monieApplied: false,

        settlementError: '',

   
        get subtotal() {
            return this.items.reduce((sum, item) => sum + item.Gross, 0);
        },

        get totalDiscount() {
            return this.items.reduce((sum, item) => sum + item.DiscountAmt, 0);
        },

        get tax() {

            return this.items.reduce((sum, item) => sum + item.TaxAmt, 0);
        },

        get grandTotal() {
            return this.subtotal + this.tax;
        },

        get totalPayments() {
            let total = 0;
            // Add all payment types except cash (cash can overpay for change)
            total += this.bankPayments.reduce((sum, p) => sum + (p.amount || 0), 0);
            if (this.voucherApplied) total += this.voucherAmount || 0;
            if (this.creditNoteApplied) total += this.creditNoteAmount || 0;
            if (this.mobileMoneyApplied) total += this.mobileMoneyAmount || 0;
            return total;
        },

        get outstandingAmount() {
            const remaining = this.grandTotal - this.totalPayments - (this.cashAmount || 0);
            return remaining > 0 ? remaining : 0;
        },

        get changeAmount() {
            const overpayment = (this.cashAmount || 0) + this.totalPayments - this.grandTotal;
            return overpayment > 0 ? overpayment : 0;
        },
        initSetUserOutlet() {

            Focus8WAPI.awakeSession();
            focusSessionUpdater("setUserDetails");
        },
        setUserOutlet(respData) {

            // Set user information
            this.cashierName = respData.UserName || respData.LoginName || '';
            this.cashierPhone = respData.PhoneNumber || respData.Phone || '';
            this.branchName = respData.BranchName || respData.OutletName || '';

            let url = `/focussalesmodule/api/sales/outlets/?compId=${respData.CompanyId}&loginId=${respData.LoginId}`;

            fetch(url).then(async response => {
                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
                }
                return response.json();
            }).then(dataObj => {

                console.log(dataObj);
                this.docNo = dataObj.data.DocNo;
                if (dataObj.result == 1) {

                    this.outletList = dataObj.data.Outlets;
                    this.costCenters = dataObj.data.CostCenters;

                    // Update branch name from outlet list if available and not already set
                    if (dataObj.data.BranchName) {
                        this.branchName = dataObj.data.BranchName;
                    }

                    //const select2Data = dataObj.data.Outlets.map(item => ({
                    //    id: item.Id,
                    //    text: item.Name
                    //}));
                    //console.log(select2Data)
                    //$("#outletSelect").select2({ data: select2Data });
                }
                else {
                    this.showAlertMessage(dataObj.message);
                }


            }).catch(error => {
                console.log(error);
                //
            });
        },
        get canCompleteSettlement() {
            // Can complete if total payments >= grand total OR if cash is provided and covers the outstanding
            const totalPaid = this.totalPayments + (this.cashAmount || 0);
            return totalPaid >= this.grandTotal;
        },
        calculateGross(qty, Price, DiscountPct, fixedDiscountAmt) {
            // Formula: (Qty*SellingRate) - ((Qty*SellingRate)* (Disc %)) - Disc Amt
            const base = qty * Price;
            const percentageDiscount = base * (DiscountPct / 100);
            const totalDiscount = percentageDiscount + fixedDiscountAmt;
            const gross = base - percentageDiscount - fixedDiscountAmt;

            return {
                Gross: gross,
                TotalDiscount: totalDiscount
            };
        },

        async scanRMA() {
            Focus8WAPI.awakeSession();
            if (!this.rmaSearch.trim()) {
                this.showAlertMessage('Please enter an RMA number');
                return;
            }

            if (!this.selectedOutlet.toString().trim()) {

                this.showAlertMessage('Select outlet to continue !!!');
                return;

            }
            let outletid = 42;
            let url = `/focussalesmodule/api/sales/rmaitems/?compid=${this.compid}&outletid=${outletid}&rmano=${this.rmaSearch.trim()}`;
            fetch(url).then(async response => {
                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
                }
                return response.json();
            }).then(dataObj => {
                if (dataObj.result == 1) {
                    console.log(dataObj);
                    this.setItemInPOS(dataObj.data);
                }
                else {
                    this.showAlertMessage(dataObj.message);
                }


            }).catch(error => {
                console.log(error);
                //
            });
                      
        },
        setItemInPOS(rmaData) {
            
            if (rmaData) {
                // Check if item (by Id) already exists
                const existingItem = this.items.find(item => item.Id === rmaData.Id);

                if (existingItem) {
                    // Check if this specific RMA is already added
                    if (existingItem.RmaNoList.includes(this.rmaSearch.trim())) {

                        this.showAlertMessage('This RMA is already added to the transaction');

                    } else {
                        // Add RMA to the existing item and increment quantity by 1
                        existingItem.RmaNoList.push(this.rmaSearch.trim());
                        existingItem.Qty += 1;

                        // Recalculate gross based on new qty
                        // Formula: (Qty*SellingRate) - ((Qty*SellingRate)* (Disc %)) - Disc Amt
                        const result = this.calculateGross(
                            existingItem.Qty,
                            existingItem.Price,
                            existingItem.DiscountPct,
                            existingItem.FixedDiscountAmt
                        );

                        existingItem.DiscountAmt = result.TotalDiscount;
                        existingItem.Gross = result.Gross;

                        

                        if (existingItem.TaxRate > 0) {
                            let taxrate = existingItem.TaxRate / 100;
                            existingItem.TaxAmt = existingItem.IsPriceExcl ? existingItem.Gross * taxrate : (existingItem.Gross / (taxrate + 1)) * taxrate;
                        }
                        else {
                            existingItem.TaxAmt = 0;
                        }
                       

                        this.showAlertMessage('RMA added to existing item, quantity updated');
                        this.rmaSearch = '';
                    }
                }
                else {
                    // New item - default qty is 1
                    const qty = 1;

                    // Calculate gross: (Qty*SellingRate) - ((Qty*SellingRate)* (Disc %)) - Disc Amt
                    const result = this.calculateGross(
                        qty,
                        rmaData.Price,
                        rmaData.FixedDiscountAmt,
                        rmaData.DiscountPct
                    );
                    rmaData.DiscountAmt = result.TotalDiscount;
                    rmaData.Gross = result.Gross;
                    rmaData.RmaNoList = [];
                    rmaData.RmaNoList.push(rmaData.RmaNo);

                    if (rmaData.TaxRate > 0) {
                        let taxrate = rmaData.TaxRate / 100;
                        rmaData.TaxAmt = rmaData.IsPriceExcl ? rmaData.Gross * taxrate : (rmaData.Gross / (taxrate + 1)) * taxrate;
                    }
                    else {
                        rmaData.TaxAmt = 0;
                    }


                    this.items.push(rmaData);




                    this.showAlertMessage('Item added successfully');
                    this.rmaSearch = '';
                }
            }
            else {

                this.showAlertMessage('RMA not found in system');
            }
        },
        removeItem(index) {
            this.items.splice(index, 1);
            this.showAlertMessage('Item removed from transaction');
        },

        showRMAs(Id){
            const item = this.items.find(i => i.Id === Id);
            if (item) {
                this.selectedItemId = Id;
                this.selectedItemRMAs = item.RmaNoList;
                this.showRMAModal = true;
            }
        },

        clearSearch() {
            this.rmaSearch = '';
        },

        handleCustomerTypeChange() {
            if (!this.isCreditCustomer) {
                this.selectedMember = '';
                this.customerAccount = '';
            }
        },

        updateCustomerAccount() {
            if (this.selectedMember) {
                const member = this.members.find(m => m.id === this.selectedMember);
                if (member) {
                    this.customerAccount = member.accountNo;
                }
            } else {
                this.customerAccount = '';
            }
        },

        addMember() {
            if (!this.newMemberName.trim()) {
                this.memberError = 'Please enter member name';
                return;
            }
            if (!this.newMemberPhone.trim()) {
                this.memberError = 'Please enter phone number';
                return;
            }

            // Generate a simple ID (in production, this would come from backend)
            const newMember = {
                id: Date.now().toString(),
                name: this.newMemberName.trim(),
                phone: this.newMemberPhone.trim(),
                accountNo: 'ACC-' + Date.now()
            };

            this.members.push(newMember);
            this.selectedMember = newMember.id;
            this.customerAccount = newMember.accountNo;

            this.showAlertMessage('Member added successfully');
            this.cancelAddMember();
        },

        cancelAddMember() {
            this.showAddMemberModal = false;
            this.newMemberName = '';
            this.newMemberPhone = '';
            this.memberError = '';
        },

        openAdvanceReceiptModal() {
            this.showAdvanceReceiptModal = true;
            this.isCreditCustomerReadOnly = true;
            this.advanceReceiptDate = new Date().toISOString().split('T')[0];
        },

        closeAdvanceReceiptModal() {
            this.showAdvanceReceiptModal = false;
            this.advanceReceiptDate = '';
            this.advanceReceiptCashBankAccount = '';
            this.advanceReceiptOutlet = '';
            this.advanceReceiptCostCenter = '';
            this.advanceReceiptMember = '';
            this.advanceReceiptChequeNo = '';
            this.advanceReceiptCustomerAcc = '';
            this.advanceReceiptAmount = 0;
        },

        saveAdvanceReceipt() {
            // Validate required fields
            if (!this.advanceReceiptDate) {
                this.showAlertMessage('Please select a date');
                return;
            }
            if (!this.advanceReceiptAmount || this.advanceReceiptAmount <= 0) {
                this.showAlertMessage('Please enter a valid amount');
                return;
            }

            // Here you would typically send this data to the backend
            console.log('Advance Receipt Data:', {
                date: this.advanceReceiptDate,
                cashBankAccount: this.advanceReceiptCashBankAccount,
                outlet: this.advanceReceiptOutlet,
                costCenter: this.advanceReceiptCostCenter,
                member: this.advanceReceiptMember,
                chequeNo: this.advanceReceiptChequeNo,
                customerAcc: this.advanceReceiptCustomerAcc,
                amount: this.advanceReceiptAmount
            });

            this.showAlertMessage('Advance Receipt created successfully');
            this.closeAdvanceReceiptModal();
        },

        saveTransaction() {
            if (this.items.length === 0) {
                this.showAlertMessage('No items to save');
                return;
            }

            // Open settlement modal
            this.resetSettlement();
            this.showSettlementModal = true;
        },
     
        validateDocument(sessionid) {

            this.postSale(sessionid);
        },
        postSale(sessionid) {


            let url = `/focussalesmodule/api/sales/addsale?compid=${this.compid}&sessionid=${sessionid}`;
            fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(this.transactionData)
            }).then(async response => {
                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
                }
                return response.json();
            }).then(dataObj => {

                this.showAlertMessage(dataObj.message);

                this.items = [];
                this.transactionData = {};

            }).catch(error => {
                console.log(error);
                //
            });
        },
        resetSettlement() {
            this.cashAmount = 0;
            this.bankPayments = [];
            this.newBankPayment = { method: '', reference: '', amount: 0 };
            this.voucherCode = '';
            this.voucherAmount = 0;
            this.voucherApplied = false;
            this.voucherItemCode = '';
            this.creditNoteNumber = '';
            this.creditNoteAmount = 0;
            this.creditNoteApplied = false;
            this.mobileMoneyProvider = '';
            this.mobileMoneyPhone = '';
            this.mobileMoneyTransactionId = '';
            this.mobileMoneyAmount = 0;
            this.mobileMoneyApplied = false;
            this.settlementError = '';
            this.activePaymentTab = 'cash';
        },

        calculatePayments() {
            // Recalculate to update computed properties
            this.settlementError = '';
        },

        addBankPayment() {
            if (!this.newBankPayment.method) {
                this.settlementError = 'Please select a payment method';
                return;
            }
            if (!this.newBankPayment.reference) {
                this.settlementError = 'Please enter a reference number';
                return;
            }
            if (!this.newBankPayment.amount || this.newBankPayment.amount <= 0) {
                this.settlementError = 'Please enter a valid amount';
                return;
            }

            // Check if total exceeds grand total
            const newTotal = this.totalPayments + this.newBankPayment.amount;
            if (newTotal > this.grandTotal) {
                this.settlementError = 'Total bank payments cannot exceed the bill amount';
                return;
            }

            this.bankPayments.push({
                method: this.newBankPayment.method,
                reference: this.newBankPayment.reference,
                amount: this.newBankPayment.amount
            });

            this.newBankPayment = { method: '', reference: '', amount: 0 };
            this.settlementError = '';
        },
        addMonie() {

            if (!this.newMoniePayment.reference) {
                this.settlementError = 'Please enter a reference number';
                return;
            }
            if (!this.newMoniePayment.amount || this.newMoniePayment.amount <= 0) {
                this.settlementError = 'Please enter a valid amount';
                return;
            }

            // Check if total exceeds grand total
            const newTotal = this.totalPayments + this.newMoniePayment.amount;
            if (newTotal > this.grandTotal) {
                this.settlementError = 'Total bank payments cannot exceed the bill amount';
                return;
            }

            this.moniePayments.push({
                reference: this.newMoniePayment.reference,
                amount: this.newMoniePayment.amount
            });

            this.newMoniePayment = { reference: '', amount: 0 };
            this.settlementError = '';
        },

        removeBankPayment(index) {
            this.bankPayments.splice(index, 1);
        },
        removeMoniePayment(index) {
            this.moniePayments.splice(index, 1);
        },

        async applyVoucher() {
            if (!this.voucherCode) {
                this.settlementError = 'Please enter a voucher code';
                return;
            }

            // Get all unique item codes from transaction
            const ItemCodes = [...new Set(this.items.map(item => item.ItemCode))];

            if (ItemCodes.length === 0) {
                this.settlementError = 'No items in transaction';
                return;
            }

            // Validate voucher against each item code
            let validVoucher = null;
            let validItemCode = null;

            for (const ItemCode of ItemCodes) {
                try {
                    const response = await fetch(`/DiscountVoucher/ValidateVoucher?code=${encodeURIComponent(this.voucherCode)}&ItemCode=${encodeURIComponent(ItemCode)}`);
                    const result = await response.json();

                    if (result.success) {
                        validVoucher = result.data;
                        validItemCode = ItemCode;
                        break;
                    }
                } catch (error) {
                    console.error('Error validating voucher:', error);
                }
            }

            if (!validVoucher) {
                this.settlementError = 'Invalid voucher code or voucher not applicable to items in cart';
                return;
            }

            // Check if this item already has a voucher applied
            if (this.voucherApplied && this.voucherItemCode === validItemCode) {
                this.settlementError = 'This item already has a voucher applied';
                return;
            }

            // Set voucher amount from validated voucher
            this.voucherAmount = validVoucher.VoucherValue;
            this.voucherItemCode = validItemCode;

            // Check if total exceeds grand total
            const newTotal = this.totalPayments + this.voucherAmount;
            if (newTotal > this.grandTotal) {
                this.settlementError = 'Total payments cannot exceed the bill amount';
                return;
            }

            this.voucherApplied = true;
            this.settlementError = '';
            this.showAlertMessage(`Voucher applied successfully! Discount: ${this.formatCurrency(this.voucherAmount)}`);
        },

        applyCreditNote() {
            if (!this.creditNoteNumber) {
                this.settlementError = 'Please enter a credit note number';
                return;
            }
            if (!this.creditNoteAmount || this.creditNoteAmount <= 0) {
                this.settlementError = 'Please enter a valid credit note amount';
                return;
            }

            // Check if total exceeds grand total
            const newTotal = this.totalPayments + this.creditNoteAmount;
            if (newTotal > this.grandTotal) {
                this.settlementError = 'Total payments cannot exceed the bill amount';
                return;
            }

            this.creditNoteApplied = true;
            this.settlementError = '';
        },

        applyMobileMoney() {
            if (!this.mobileMoneyProvider) {
                this.settlementError = 'Please select a mobile money provider';
                return;
            }
            if (!this.mobileMoneyPhone) {
                this.settlementError = 'Please enter a phone number';
                return;
            }
            if (!this.mobileMoneyTransactionId) {
                this.settlementError = 'Please enter a transaction ID';
                return;
            }
            if (!this.mobileMoneyAmount || this.mobileMoneyAmount <= 0) {
                this.settlementError = 'Please enter a valid amount';
                return;
            }

            // Check if total exceeds grand total
            const newTotal = this.totalPayments + this.mobileMoneyAmount;
            if (newTotal > this.grandTotal) {
                this.settlementError = 'Total payments cannot exceed the bill amount';
                return;
            }

            this.mobileMoneyApplied = true;
            this.settlementError = '';
        },
        transactionData: {},
        async completeSettlement() {
            if (!this.canCompleteSettlement) {
                this.settlementError = 'Payment amount is insufficient';
                return;
            }

            

            // Prepare transaction data with payment details
            this.transactionData = {
                Items: this.items,
                SubTotal: this.subtotal,
                Tax: this.tax,
                TotalDiscount: this.totalDiscount,
                GrandTotal: this.grandTotal,
                Payments: {
                    Cash: this.cashAmount || 0,
                    BankPayments: this.bankPayments,
                    DiscountVoucher: this.voucherApplied ? {
                        Code: this.voucherCode,
                        Amount: this.voucherAmount,
                        
                    } : null,
                    CreditNote: this.creditNoteApplied ? { Number: this.creditNoteNumber, Amount: this.creditNoteAmount } : null,
                    Monie: this.monieApplied ? {
                        MonieTransactionId: this.monieTransactionId,
                        Amount: this.monieMoneyAmount
                    }  : null
                },
                TotalPaid: this.totalPayments + (this.cashAmount || 0),
                Change: this.changeAmount,
                Timestamp: new Date().toISOString()
            };

            console.log('Saving transaction with settlement:', this.transactionData);
            focusSessionUpdater("setSession");
            // Close modal and show success
            this.showSettlementModal = false;
           // this.showAlertMessage('Transaction saved successfully!');

            // Optionally clear items after successful save
            // this.items = [];
        },

        cancelSettlement() {
            this.showSettlementModal = false;
            this.resetSettlement();
        },

        reprintTransaction() {
            if (this.items.length === 0) {
                this.showAlertMessage('No transaction to reprint');
                return;
            }

            console.log('Reprinting transaction');
            this.showAlertMessage('Sending to printer...');
        },

        newTransaction() {
            if (this.items.length > 0) {
                if (confirm('Start a new transaction? Current items will be cleared.')) {
                    this.items = [];
                    this.rmaSearch = '';
                    this.showAlertMessage('New transaction started');
                }
            } else {
                this.showAlertMessage('Ready for new transaction');
            }
        },

        holdTransaction() {
            if (this.items.length === 0) {
                this.showAlertMessage('No transaction to hold');
                return;
            }

            console.log('Holding transaction');
            this.showAlertMessage('Transaction held');
        },

        cancelTransaction() {
            if (this.items.length > 0) {
                if (confirm('Cancel this transaction? All items will be cleared.')) {
                    this.items = [];
                    this.rmaSearch = '';
                    this.showAlertMessage('Transaction cancelled');
                    Focus8WAPI.gotoHomePage();
                }
            }
            
        },

        formatCurrency(amount) {
            let num = Number(amount);
            let formatted = num.toFixed(2);
            return formatted;
            //return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        },

        showAlertMessage(message) {
            this.alertMessage = message;
            this.showAlert = true;
            setTimeout(() => {
                this.showAlert = false;
            }, 3000);
        }
    }
}