﻿using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
       
            /*last 7 days*/
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            /*Total income*/
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type != "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C2");


            //Total Expense
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C2");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C2}", Balance);

            //recent transactions
            ViewBag.RecentTransactions = await _context.Transactions
               .Include(i => i.Category)
               .OrderByDescending(j => j.Date)
               .Take(5)
               .ToListAsync();

            return View();
        }
    }
}
