using Microsoft.AspNetCore.Mvc;
using FocusSalesModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FocusSalesModule.Controllers
{
    public class DiscountVoucherController : Controller
    {
        // In-memory storage for demonstration (replace with actual database)
        private static List<DiscountVoucher> _vouchers = new List<DiscountVoucher>();
        private static int _nextId = 1;

        // GET: DiscountVoucher
        public IActionResult Index()
        {
            return View();
        }

        // GET: API endpoint to get all vouchers
        [HttpGet]
        public JsonResult GetVouchers()
        {
            var vouchers = _vouchers.Select(v => new
            {
                v.Id,
                v.ItemCode,
                v.DiscountCode,
                v.MaxUsageCount,
                v.CurrentUsageCount,
                v.VoucherValue,
                StartDate = v.StartDate.ToString("yyyy-MM-dd"),
                ExpiryDate = v.ExpiryDate.ToString("yyyy-MM-dd"),
                v.IsActive,
                Status = v.GetStatusMessage(),
                IsValid = v.IsValid()
            }).ToList();

            return Json(new { success = true, data = vouchers });
        }

        // POST: API endpoint to create new voucher
        [HttpPost]
        public JsonResult CreateVoucher([FromBody] DiscountVoucher voucher)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid voucher data" });
            }

            // Check if discount code already exists
            if (_vouchers.Any(v => v.DiscountCode == voucher.DiscountCode))
            {
                return Json(new { success = false, message = "Discount code already exists" });
            }

            voucher.Id = _nextId++;
            voucher.CreatedDate = DateTime.Now;
            voucher.CurrentUsageCount = 0;
            voucher.IsActive = true;

            _vouchers.Add(voucher);

            return Json(new { success = true, message = "Voucher created successfully", data = voucher });
        }

        // PUT: API endpoint to update voucher
        [HttpPut]
        public JsonResult UpdateVoucher([FromBody] DiscountVoucher voucher)
        {
            var existing = _vouchers.FirstOrDefault(v => v.Id == voucher.Id);
            if (existing == null)
            {
                return Json(new { success = false, message = "Voucher not found" });
            }

            existing.ItemCode = voucher.ItemCode;
            existing.DiscountCode = voucher.DiscountCode;
            existing.MaxUsageCount = voucher.MaxUsageCount;
            existing.VoucherValue = voucher.VoucherValue;
            existing.StartDate = voucher.StartDate;
            existing.ExpiryDate = voucher.ExpiryDate;
            existing.IsActive = voucher.IsActive;

            return Json(new { success = true, message = "Voucher updated successfully" });
        }

        // DELETE: API endpoint to delete voucher
        [HttpDelete]
        public JsonResult DeleteVoucher(int id)
        {
            var voucher = _vouchers.FirstOrDefault(v => v.Id == id);
            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher not found" });
            }

            _vouchers.Remove(voucher);
            return Json(new { success = true, message = "Voucher deleted successfully" });
        }

        // GET: API endpoint to validate voucher for POS
        [HttpGet]
        public JsonResult ValidateVoucher(string code, string itemCode)
        {
            var voucher = _vouchers.FirstOrDefault(v => v.DiscountCode == code);

            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher not found" });
            }

            if (voucher.ItemCode != itemCode)
            {
                return Json(new { success = false, message = "Voucher is not valid for this item" });
            }

            if (!voucher.IsValid())
            {
                return Json(new { success = false, message = voucher.GetStatusMessage() });
            }

            return Json(new
            {
                success = true,
                message = "Voucher is valid",
                data = new
                {
                    voucher.DiscountCode,
                    voucher.VoucherValue,
                    voucher.ItemCode,
                    RemainingUses = voucher.MaxUsageCount - voucher.CurrentUsageCount
                }
            });
        }

        // POST: API endpoint to use voucher (increment usage count)
        [HttpPost]
        public JsonResult UseVoucher([FromBody] VoucherUsageRequest request)
        {
            var voucher = _vouchers.FirstOrDefault(v => v.DiscountCode == request.Code);

            if (voucher == null)
            {
                return Json(new { success = false, message = "Voucher not found" });
            }

            if (!voucher.IsValid())
            {
                return Json(new { success = false, message = voucher.GetStatusMessage() });
            }

            voucher.CurrentUsageCount++;

            return Json(new
            {
                success = true,
                message = "Voucher used successfully",
                data = new
                {
                    RemainingUses = voucher.MaxUsageCount - voucher.CurrentUsageCount
                }
            });
        }
    }

    public class VoucherUsageRequest
    {
        public string Code { get; set; }
        public string ItemCode { get; set; }
    }
}
