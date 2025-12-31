function transactionManager() {
    return {

        init() {
            this.compid = this.$refs.compid.value;
            this.loadPayments();
        },
        compid: 0,
        searchVal: "",
        paymentList: [],
        filterList: [],
        currentPage: 1,
        pagingData: {
            totalItems: 1,
            pageSize: 15,
            startingPage: 0,
            visiblePages: 10,
            curVisiblePages: 1,
            totalPages: 1,
            pagingDesc: "",

        },
        getDateFormat(date) {
            const d = new Date(date);
            const day = String(d.getDate()).padStart(2, '0');
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const year = d.getFullYear();
            return `${day}/${month}/${year}`;
        },
        formatNum(num) {
            return num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        },
        getIsTxnValid(row) {
            return row.IsValidTxn && (row.TransactionStatus == "SUCCESSFUL" || row.TransactionStatus == "APPROVED")  
        },
        toggleStatus(row, status) {
            let checkedStatus = this.filterList.filter(obj => obj == row.TxnId);
            if (checkedStatus.length > 0 && !status) {
                this.filterList = this.filterList.filter(obj => obj != row.TxnId);

            }

            if (checkedStatus.length == 0 && status) {
                this.filterList.push(row.TxnId);
            }
        },
        selectAll(status) {

            for (let i = 0; i < this.paymentList.length; i++) {
                let txnId = this.paymentList[i].TxnId;
                this.paymentList[i].Status = status;

                if (status && !this.filterList.includes(txnId)) {
                    this.filterList.push(txnId);
                }

                if (!status && this.filterList.includes(txnId)) {

                    this.filterList = this.filterList.filter(obj => obj != txnId)
                }


            }
        },
        //initExportInvoices() {
        //    for (let i = 0; i < this.filterList.length; i++) {

        //        this.exportInvoices(this.filterList[i]);
        //    }
        //},
        //exportInvoices(invId) {
        //    let paymentListUrl = `/subatiintegrations/InvoicePrint/ExportById?InvoiceId=${invId}&compid=${this.compid}`;
        //    window.open(paymentListUrl);

        //},

        async loadPayments() {
            let txnListUrl = `/focussalesmodule/TransactionManager/GetAllPaged/?compid=${this.compid}&pageno=${this.currentPage}&pagesize=${this.pagingData.pageSize}&searchval=${this.searchVal}`;

            fetch(txnListUrl).then(async response => {
                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
                }
                return response.json();
            }).then(dataObj => {

                console.log(dataObj);
                if (dataObj.result == 1) {
                    this.paymentList = dataObj.data.data;
                    this.filterList = [];
                    this.pagingData.totalItems = dataObj.data.recordsTotal;
                    this.pagingData.totalPages = dataObj.data.totalPages;
                    this.setCurVisiblePages();
                    console.log(this.paymentList);
                }
                else {
                    alert(dataObj.message);
                }


            }).catch(error => {
                console.log(error);
            });

        },
        screenModal: null,
        openEditModal(txnObj) {

            if (txnObj.IsAllocatedToSale || !this.getIsTxnValid(txnObj)) {

                Swal.fire('The transaction has already been allocated or its invalid !!!');
                return;
            }
            //let options = { keyboard: false };
            //this.screenModal = new bootstrap.Modal(document.getElementById('lipaMpesaModal'), options)
            //this.screenModal.show();
            Swal.fire({
                title: 'Change Txn Status?',
                text: 'Do you want to change transaction status to allocated?',
                icon: 'info',
                showCancelButton: true,
                confirmButtonText: 'Yes',
                cancelButtonText: 'No'
            }).then((result) => {
                if (result.isConfirmed) {
                    this.updateTxnStatus(txnObj);
                } else if (result.isDismissed) {
                    
                }
            });
        },
        async updateTxnStatus(txnObj) {

            let updateUrl = `/focussalesmodule/TransactionManager/updatetxnstatus/?compid=${this.compid}&txnid=${txnObj.Id}&status=1`;
            fetch(updateUrl).then(async response => {

                if (!response.ok) {

                    const errorText = await response.text();
                    throw new Error(errorText);
                }
                return response.json();
            }).then(dataObj => {

                Swal.fire(dataObj.message);

                if (dataObj.result == 1) {

                    this.loadPayments();
                }
                

            }).catch(error => {
                console.log(error);
            });
        },
        closeFormModal() {
            this.screenModal.toggle();
        },
        timeout: null,
        debouncedInput() {
            clearTimeout(this.timeout);
            this.timeout = setTimeout(() => {
                this.onFinishedTyping();
            }, 500); // 500ms debounce
        },
        onFinishedTyping() {
            this.loadPayments();
        },
        resetFilters() {
            this.searchVal = "";
            this.itemStatus = 0;
            this.filterList = [];
            this.currentPage = 1;
            this.loadPayments();
        },
        totalPages() {
            return Math.ceil(this.pagingData.totalItems / this.pagingData.pageSize);
        },
        setPagingDesc() {
            this.pagingData.pagingDesc = `Showing page ${this.currentPage} of ${this.pagingData.totalPages}`;
        },
        currentGroup(page) {

            this.currentPage = page;
            this.loadPayments();
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

        nextGroup() {

            if (this.currentPage < this.pagingData.totalPages) {
                this.currentPage += 1;
                this.pagingData.startingPage = this.getCurrentGroupStart();

                this.setCurVisiblePages()

            }
            this.loadPayments();

        },

        prevGroup() {

            if (this.currentPage > 1) {
                this.currentPage -= 1;
                this.pagingData.startingPage = this.getCurrentGroupStart();

                this.setCurVisiblePages();
            }
            this.loadPayments();

        },
        getAmount(num) {
            return isNaN(parseFloat(num)) ? 0.00 : parseFloat(num);
        },
        formatNum(val) {
            return (val).toLocaleString("en", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        },
        closeWin() {
            Focus8WAPI.gotoHomePage();
        }
    }
}