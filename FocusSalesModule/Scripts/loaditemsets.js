function itemSetScr() {
    return {
        compid: 0,
        itemList: [],
        currentPage: 1,
        searchVal: "",
        pagingData: {
            totalItems: 1,
            pageSize: 10,
            startingPage: 0,
            visiblePages: 10,
            curVisiblePages: 1,
            totalPages: 1,
            pagingDesc: "",

        },
        init() {

            this.compid = this.$refs.compid.value;
            //loadedItemsArr = [];
            console.log(loadedItemsArr);
            this.loadItems();
        },
        updateStatus(row) {

            let val = row.Status;
            for (let i = 0; i < this.itemList.length; i++) {
                this.itemList[i].Status = false;

            }
            if (val) {
                row.Status = val;
            }
        },
        async loadItems() {
            let itemListUrl = `/focussalesmodule/api/preordersales/loadsets?compid=${this.compid}&pageno=${this.currentPage}&pagesize=${this.pagingData.pageSize}&searchval=${this.searchVal}`;

            fetch(itemListUrl).then(async response => {
                if (!response.ok) {
                    const errorText = await response.text();
                    throw new Error(errorText);
                }
                return response.json();
            }).then(data => {

              
                if (data.result == 1) {

                    this.itemList = data.datalist;
                  //  loadedItemsArr = [];
                    this.pagingData.totalItems = data.pagingStatus.recordsTotal;
                    this.pagingData.totalPages = data.pagingStatus.totalPages;
                    this.setCurVisiblePages();

                }
                else {
                    alert(data.message);
                }


            }).catch(error => {
                console.log(error);
            });
        },
        async getItems(itemId, rowno, arrlength) {

            if (!itemId) {
                return;
            }

            let res = await fetch(`/focussalesmodule/api/preordersales/loaditems?compid=${this.compid}&itemId=${itemId}`);

            let data = await res.json();
            if (data.result === 1) {
                
                scrItemList.push(... data.datalist);

                loadedItemsArr.push(itemId);
                getCurrentRows();
                this.closeWin();
                //console.log(data);
               
            }
            //if (rowno == (arrlength - 1)) {

            //    getCurrentRows();
            //    this.itemList.forEach(item => {
            //        item.Status = false;
            //    });

            //}
        },
        selectAll(status) {

            for (let i = 0; i < this.itemList.length; i++) {
                this.itemList[i].Status = status;

            }
        },
        addItems() {

            scrItemList = [];
            let selectedItems = this.itemList.filter(obj => obj.Status == true);
            let arrlength = selectedItems.length;
            if (arrlength  > 0) {
                for (let i = 0; i < arrlength; i++) {

                    let itemId = selectedItems[i].Id;

                    if (loadedItemsArr.includes(itemId)) {
                        continue;
                    }

                    this.getItems(itemId, i, arrlength)
                }

            }
            else {
                alert("Select items to continue !!");
            }

        },
        timeout: null,
        setRecordDisplayCount() {

            this.loadItems();
        },
        debouncedInput() {
            clearTimeout(this.timeout);
            this.timeout = setTimeout(() => {
                this.onFinishedTyping();
            }, 500); // 500ms debounce
        },
        onFinishedTyping() {
            this.loadItems();
        },
        resetFilters() {

            this.searchVal = "";
            this.filterList = [];
            this.currentPage = 1;
            this.loadItems();
        },
        totalPages() {
            return Math.ceil(this.pagingData.totalItems / this.pagingData.pageSize);
        },
        setPagingDesc() {
            this.pagingData.pagingDesc = `Showing page ${this.currentPage} of ${this.pagingData.totalPages}`;
        },
        currentGroup(page) {

            this.currentPage = page;
            this.loadItems();
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
            this.loadItems();

        },

        prevGroup() {

            if (this.currentPage > 1) {
                this.currentPage -= 1;
                this.pagingData.startingPage = this.getCurrentGroupStart();

                this.setCurVisiblePages();
            }
            this.loadItems();

        },   
        closeWin() {
            Focus8WAPI.closePopup();
        }
    }
}

//initItemSet() {
//    const self = this;
//    let tempcompid = this.compid;
//    let $itemsetselect = $('#itemsearch').select2(
//        {
//            ajax: {
//                url: `/focussalesmodule/api/preordersales/itemsets`,
//                dataType: 'json',
//                delay: 250,
//                data: function (params) {
//                    return {
//                        _type: 'query',
//                        compid: tempcompid,
//                        term: params.term,
//                        page: params.page || 1,
//                        pagesize: 10

//                    };
//                },
//                processResults: function (dataObj, params) {

//                    params.page = params.page || 1;
//                    let tempObj = {
//                        results: dataObj.datalist.map(item => ({
//                            id: item.Id,
//                            text: item.Name
//                        })),
//                        pagination: {
//                            more: dataObj.pagingStatus.hasMore
//                        }
//                    };
//                    return tempObj;
//                }
//            },
//            width: '100%',
//            height: '100%',

//        });
//    //

//    $itemsetselect.on('change', (evt) => {

//        self.getItems($('#itemsearch').val());

//    });

//},