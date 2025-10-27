using Microsoft.AspNetCore.Mvc;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FocusSalesModule.Controllers
{
    public class StockTransferController : Controller
    {
        // In-memory storage for demonstration (replace with actual database)
        private static List<StockTransfer> _stockTransfers = new List<StockTransfer>();
        private static List<Outlet> _outlets = new List<Outlet>();
        private static List<Product> _products = new List<Product>();
        private static int _nextTransferId = 1;
        private static int _nextItemId = 1;
        private static int _nextOutletId = 1;
        private static int _nextProductId = 1;

        // Static constructor to initialize sample data
        static StockTransferController()
        {
            // Initialize sample outlets
            _outlets.Add(new Outlet
            {
                Id = _nextOutletId++,
                Code = "OUT001",
                Name = "Main Warehouse",
                Location = "Downtown",
                IsActive = true
            });
            _outlets.Add(new Outlet
            {
                Id = _nextOutletId++,
                Code = "OUT002",
                Name = "Branch Store 1",
                Location = "Uptown",
                IsActive = true
            });
            _outlets.Add(new Outlet
            {
                Id = _nextOutletId++,
                Code = "OUT003",
                Name = "Branch Store 2",
                Location = "Suburbs",
                IsActive = true
            });

            // Initialize sample products
            _products.Add(new Product
            {
                Id = _nextProductId++,
                Code = "PROD001",
                Name = "Product A",
                BaseUnit = "PCS",
                Stock = 100,
                Price = 50.00m
            });
            _products.Add(new Product
            {
                Id = _nextProductId++,
                Code = "PROD002",
                Name = "Product B",
                BaseUnit = "KG",
                Stock = 200,
                Price = 30.00m
            });
            _products.Add(new Product
            {
                Id = _nextProductId++,
                Code = "PROD003",
                Name = "Product C",
                BaseUnit = "LTR",
                Stock = 150,
                Price = 25.00m
            });
        }

        // GET: StockTransfer/StockIssue
        public IActionResult StockIssue()
        {
            return View();
        }

        // GET: StockTransfer/StockReceipt
        public IActionResult StockReceipt()
        {
            return View();
        }

        // GET: API endpoint to get all outlets
        [HttpGet]
        public JsonResult GetOutlets()
        {
            var outlets = _outlets.Where(o => o.IsActive).Select(o => new
            {
                id = o.Id,
                text = o.Name,
                code = o.Code,
                location = o.Location
            }).ToList();

            return Json(new { success = true, data = outlets });
        }

        // GET: API endpoint to get all products
        [HttpGet]
        public JsonResult GetProducts()
        {
            var products = _products.Select(p => new
            {
                id = p.Id,
                text = p.Name,
                code = p.Code,
                unit = p.BaseUnit,
                stock = p.Stock,
                price = p.Price
            }).ToList();

            return Json(new { success = true, data = products });
        }

        // GET: API endpoint to get all stock transfers
        [HttpGet]
        public JsonResult GetStockTransfers()
        {
            var transfers = _stockTransfers.Select(t => new
            {
                t.Id,
                t.StockIssueNo,
                t.FromOutletId,
                t.FromOutletName,
                t.ToOutletId,
                t.ToOutletName,
                t.Narration,
                TransferDate = t.TransferDate.ToString("yyyy-MM-dd HH:mm:ss"),
                t.Status,
                t.IsReceived,
                ReceivedDate = t.ReceivedDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                Items = t.Items.Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.ProductName,
                    i.ProductCode,
                    i.Unit,
                    i.Quantity,
                    i.RmaNo
                }).ToList()
            }).ToList();

            return Json(new { success = true, data = transfers });
        }

        // GET: API endpoint to get a specific stock transfer by stock issue number
        [HttpGet]
        public JsonResult GetStockTransferByIssueNo(string issueNo)
        {
            var transfer = _stockTransfers.FirstOrDefault(t => t.StockIssueNo == issueNo);

            if (transfer == null)
            {
                return Json(new { success = false, message = "Stock issue not found" });
            }

            var transferData = new
            {
                transfer.Id,
                transfer.StockIssueNo,
                transfer.FromOutletId,
                transfer.FromOutletName,
                transfer.ToOutletId,
                transfer.ToOutletName,
                transfer.Narration,
                TransferDate = transfer.TransferDate.ToString("yyyy-MM-dd HH:mm:ss"),
                transfer.Status,
                transfer.IsReceived,
                ReceivedDate = transfer.ReceivedDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                Items = transfer.Items.Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.ProductName,
                    i.ProductCode,
                    i.Unit,
                    i.Quantity,
                    i.RmaNo
                }).ToList()
            };

            return Json(new { success = true, data = transferData });
        }

        // POST: API endpoint to create a stock transfer
        [HttpPost]
        public JsonResult CreateStockTransfer([FromBody] StockTransfer transfer)
        {
            try
            {
                // Validation
                if (transfer.FromOutletId == 0 || transfer.ToOutletId == 0)
                {
                    return Json(new { success = false, message = "Please select both From and To outlets" });
                }

                if (transfer.FromOutletId == transfer.ToOutletId)
                {
                    return Json(new { success = false, message = "From and To outlets cannot be the same" });
                }

                if (transfer.Items == null || !transfer.Items.Any())
                {
                    return Json(new { success = false, message = "Please add at least one item" });
                }

                // Validate quantities
                foreach (var item in transfer.Items)
                {
                    if (item.Quantity <= 0)
                    {
                        return Json(new { success = false, message = "Quantity must be greater than 0" });
                    }
                }

                // Generate stock issue number
                transfer.Id = _nextTransferId++;
                transfer.StockIssueNo = $"SI{DateTime.Now:yyyyMMdd}{transfer.Id:D4}";
                transfer.TransferDate = DateTime.Now;
                transfer.Status = "Issued";
                transfer.IsReceived = false;

                // Get outlet names
                var fromOutlet = _outlets.FirstOrDefault(o => o.Id == transfer.FromOutletId);
                var toOutlet = _outlets.FirstOrDefault(o => o.Id == transfer.ToOutletId);
                transfer.FromOutletName = fromOutlet?.Name;
                transfer.ToOutletName = toOutlet?.Name;

                // Assign item IDs and populate product details
                foreach (var item in transfer.Items)
                {
                    item.Id = _nextItemId++;
                    item.StockTransferId = transfer.Id;

                    var product = _products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        item.ProductName = product.Name;
                        item.ProductCode = product.Code;
                        item.Unit = product.BaseUnit;
                    }
                }

                _stockTransfers.Add(transfer);

                return Json(new { success = true, message = "Stock transfer created successfully", stockIssueNo = transfer.StockIssueNo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: API endpoint to receive a stock transfer
        [HttpPost]
        public JsonResult ReceiveStockTransfer([FromBody] ReceiveStockTransferRequest request)
        {
            try
            {
                var transfer = _stockTransfers.FirstOrDefault(t => t.Id == request.TransferId);

                if (transfer == null)
                {
                    return Json(new { success = false, message = "Stock transfer not found" });
                }

                if (transfer.IsReceived)
                {
                    return Json(new { success = false, message = "This stock transfer has already been received" });
                }

                transfer.IsReceived = true;
                transfer.ReceivedDate = DateTime.Now;
                transfer.Status = "Received";

                return Json(new { success = true, message = "Stock transfer received successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Helper class for receive request
        public class ReceiveStockTransferRequest
        {
            public int TransferId { get; set; }
        }
    }
}
