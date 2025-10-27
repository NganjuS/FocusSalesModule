function posSystem() {
    return {
        items: [],
        rmaSearch: '',
        showAlert: false,
        alertMessage: '',
        taxRate: 0.16,
        showRMAModal: false,
        selectedItemCode: '',
        selectedItemRMAs: [],

        // Settlement Modal
        showSettlementModal: false,
        activePaymentTab: 'cash',

        // Cash payment
        cashAmount: 0,

        // Bank payments
        bankPayments: [],
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
        mobileMoneyTransactionId: '',
        mobileMoneyAmount: 0,
        mobileMoneyApplied: false,

        settlementError: '',

        // Mock data for RMA lookup (replace with actual API call)
        rmaDatabase: {
            'RMA2024001': {
                itemCode: 'ITM001',
                itemName: 'Product Sample Name Here',
                unit: 'PCS',
                rate: 250.00,
                discountPct: 10,
                discountAmt: 125.00
            },
            'RMA2024002': {
                itemCode: 'ITM002',
                itemName: 'Another Product Item',
                unit: 'KG',
                rate: 150.00,
                discountPct: 5,
                discountAmt: 75.00
            },
            'RMA2024003': {
                itemCode: 'ITM003',
                itemName: 'Sample Item Description',
                unit: 'BOX',
                rate: 500.00,
                discountPct: 15,
                discountAmt: 150.00
            },
            'RMA2024004': {
                itemCode: 'ITM004',
                itemName: 'Electronics Component',
                unit: 'PCS',
                rate: 75.00,
                discountPct: 0,
                discountAmt: 0.00
            },
            'RMA2024005': {
                itemCode: 'ITM001',
                itemName: 'Product Sample Name Here',
                unit: 'PCS',
                rate: 250.00,
                discountPct: 10,
                discountAmt: 125.00
            }
        },

        get subtotal() {
            return this.items.reduce((sum, item) => sum + item.gross, 0);
        },

        get totalDiscount() {
            return this.items.reduce((sum, item) => sum + item.discountAmt, 0);
        },

        get tax() {
            return this.subtotal * this.taxRate;
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

        get canCompleteSettlement() {
            // Can complete if total payments >= grand total OR if cash is provided and covers the outstanding
            const totalPaid = this.totalPayments + (this.cashAmount || 0);
            return totalPaid >= this.grandTotal;
        },

        calculateGross(qty, rate, discountPct, fixedDiscountAmt) {
            // Formula: (Qty*Rate) - ((Qty*Rate)* (Disc %)) - Disc Amt
            const base = qty * rate;
            const percentageDiscount = base * (discountPct / 100);
            const totalDiscount = percentageDiscount + fixedDiscountAmt;
            const gross = base - percentageDiscount - fixedDiscountAmt;

            return {
                gross: gross,
                totalDiscount: totalDiscount
            };
        },

        scanRMA() {
            if (!this.rmaSearch.trim()) {
                this.showAlertMessage('Please enter an RMA number');
                return;
            }

            // Look up RMA in database (replace with actual API call)
            const rmaData = this.rmaDatabase[this.rmaSearch.trim()];

            if (rmaData) {
                // Check if item (by itemCode) already exists
                const existingItem = this.items.find(item => item.itemCode === rmaData.itemCode);

                if (existingItem) {
                    // Check if this specific RMA is already added
                    if (existingItem.rmas.includes(this.rmaSearch.trim())) {
                        this.showAlertMessage('This RMA is already added to the transaction');
                    } else {
                        // Add RMA to the existing item and increment quantity by 1
                        existingItem.rmas.push(this.rmaSearch.trim());
                        existingItem.qty += 1;

                        // Recalculate gross based on new qty
                        // Formula: (Qty*Rate) - ((Qty*Rate)* (Disc %)) - Disc Amt
                        const result = this.calculateGross(
                            existingItem.qty,
                            existingItem.rate,
                            existingItem.discountPct,
                            existingItem.fixedDiscountAmt
                        );

                        existingItem.discountAmt = result.totalDiscount;
                        existingItem.gross = result.gross;

                        this.showAlertMessage('RMA added to existing item, quantity updated');
                        this.rmaSearch = '';
                    }
                } else {
                    // New item - default qty is 1
                    const qty = 1;

                    // Calculate gross: (Qty*Rate) - ((Qty*Rate)* (Disc %)) - Disc Amt
                    const result = this.calculateGross(
                        qty,
                        rmaData.rate,
                        rmaData.discountPct,
                        rmaData.discountAmt
                    );

                    this.items.push({
                        itemCode: rmaData.itemCode,
                        rmas: [this.rmaSearch.trim()],
                        itemName: rmaData.itemName,
                        unit: rmaData.unit,
                        qty: qty,
                        rate: rmaData.rate,
                        discountAmt: result.totalDiscount,
                        discountPct: rmaData.discountPct,
                        fixedDiscountAmt: rmaData.discountAmt,  // Store original fixed discount
                        gross: result.gross
                    });

                    this.showAlertMessage('Item added successfully');
                    this.rmaSearch = '';
                }
            } else {
                this.showAlertMessage('RMA not found in system');
            }
        },

        removeItem(index) {
            this.items.splice(index, 1);
            this.showAlertMessage('Item removed from transaction');
        },

        showRMAs(itemCode) {
            const item = this.items.find(i => i.itemCode === itemCode);
            if (item) {
                this.selectedItemCode = itemCode;
                this.selectedItemRMAs = item.rmas;
                this.showRMAModal = true;
            }
        },

        clearSearch() {
            this.rmaSearch = '';
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

        removeBankPayment(index) {
            this.bankPayments.splice(index, 1);
        },

        async applyVoucher() {
            if (!this.voucherCode) {
                this.settlementError = 'Please enter a voucher code';
                return;
            }

            // Get all unique item codes from transaction
            const itemCodes = [...new Set(this.items.map(item => item.itemCode))];

            if (itemCodes.length === 0) {
                this.settlementError = 'No items in transaction';
                return;
            }

            // Validate voucher against each item code
            let validVoucher = null;
            let validItemCode = null;

            for (const itemCode of itemCodes) {
                try {
                    const response = await fetch(`/DiscountVoucher/ValidateVoucher?code=${encodeURIComponent(this.voucherCode)}&itemCode=${encodeURIComponent(itemCode)}`);
                    const result = await response.json();

                    if (result.success) {
                        validVoucher = result.data;
                        validItemCode = itemCode;
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

        async completeSettlement() {
            if (!this.canCompleteSettlement) {
                this.settlementError = 'Payment amount is insufficient';
                return;
            }

            // If voucher was used, record its usage
            if (this.voucherApplied && this.voucherCode) {
                try {
                    const response = await fetch('/DiscountVoucher/UseVoucher', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            Code: this.voucherCode,
                            ItemCode: this.voucherItemCode
                        })
                    });

                    const result = await response.json();
                    if (!result.success) {
                        this.settlementError = 'Failed to record voucher usage: ' + result.message;
                        return;
                    }
                } catch (error) {
                    console.error('Error recording voucher usage:', error);
                    this.settlementError = 'Error recording voucher usage';
                    return;
                }
            }

            // Prepare transaction data with payment details
            const transactionData = {
                items: this.items,
                subtotal: this.subtotal,
                tax: this.tax,
                totalDiscount: this.totalDiscount,
                grandTotal: this.grandTotal,
                payments: {
                    cash: this.cashAmount || 0,
                    bankPayments: this.bankPayments,
                    voucher: this.voucherApplied ? {
                        code: this.voucherCode,
                        amount: this.voucherAmount,
                        itemCode: this.voucherItemCode
                    } : null,
                    creditNote: this.creditNoteApplied ? { number: this.creditNoteNumber, amount: this.creditNoteAmount } : null,
                    mobileMoney: this.mobileMoneyApplied ? {
                        provider: this.mobileMoneyProvider,
                        phone: this.mobileMoneyPhone,
                        transactionId: this.mobileMoneyTransactionId,
                        amount: this.mobileMoneyAmount
                    } : null
                },
                totalPaid: this.totalPayments + (this.cashAmount || 0),
                change: this.changeAmount,
                timestamp: new Date().toISOString()
            };

            console.log('Saving transaction with settlement:', transactionData);

            // Close modal and show success
            this.showSettlementModal = false;
            this.showAlertMessage('Transaction saved successfully!');

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
                }
            }
        },

        formatCurrency(amount) {
            return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
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