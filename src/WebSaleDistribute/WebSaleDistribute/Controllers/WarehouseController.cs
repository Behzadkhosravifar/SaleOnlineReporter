﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSaleDistribute.Core;
using WebSaleDistribute.Models;
using AdoManager;
using Dapper;
using System.Data;

namespace WebSaleDistribute.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationUser CurrentUser
        {
            get
            {
                return UserManager.FindById(User.Identity.GetUserId());
            }
        }


        // GET: Warehouse
        public ActionResult Warehouse()
        {
            ViewBag.Title = "انبار";

            var encryptedQrCode = Request.QueryString["code"];

            if (!string.IsNullOrEmpty(encryptedQrCode))
            {
                if (encryptedQrCode.Length < 10 && User.IsInRole("Admin"))
                {
                    ViewBag.QrCode = encryptedQrCode;
                }
                else
                    ViewBag.QrCode = encryptedQrCode?.Decrypt();
            }

            return View();
        }



        // GET: EntryInWayTables
        public ActionResult EntryInWayTable(int? invoiceId)
        {
            ViewBag.QrCode = invoiceId;

            if (invoiceId == null) return null;

            // Fill Table data ---------------
            #region Table Data
            var tableData = Connections.SaleTabriz.SqlConn.ExecuteReader(
                "sp_GetInWayDetailsByOldInvoicId",
                new { OldInvoicId = invoiceId },
                commandType: CommandType.StoredProcedure);

            List<string> schema;
            var results = tableData.GetSchemaAndData(out schema);

            var model = new TableOption()
            {
                Id = "entryInWay",
                Schema = schema,
                Rows = results,
                DisplayRowsLength = 50,
                TotalFooterColumns = new string[] { }
            };

            #endregion
            //--------------------------------


            return PartialView("EntryInWayTable", model);
        }


    }
}