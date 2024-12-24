using RingSoft.ChurchLogix.DataAccess.Model.Financial_Management;
using RingSoft.DbLookup;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RingSoft.ChurchLogix.Library.ViewModels;
using Color = System.Drawing.Color;

namespace RingSoft.ChurchLogix
{
    /// <summary>
    /// Interaction logic for StatsnGraphsUserControl.xaml
    /// </summary>
    public partial class StatsnGraphsUserControl : IStatsView
    {
        public StatsnGraphsUserControl()
        {
            InitializeComponent();
        }

        public void RefreshChart()
        {
            WpfPlot.Plot.Clear();
            WpfPlot.Plot.Style(dataBackground: Color.Transparent, figureBackground: Color.Transparent);

            var fundPeriodTotals = GetChartData();
            var totalIncomeMonthsList = new List<double>();
            var totalExpenseMonthsList = new List<double>();
            var xAxisLabelsList = new List<string>();

            foreach (var fundPeriodTotal in fundPeriodTotals)
            {
                totalIncomeMonthsList.Add(fundPeriodTotal.TotalIncome);
                totalExpenseMonthsList.Add(fundPeriodTotal.TotalExpenses);
                var xAxisLabel = fundPeriodTotal.Date.ToString("MMMM\nyyyy");
                xAxisLabelsList.Add(xAxisLabel);
            }

            double[] xs = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
            double[] ys = totalIncomeMonthsList.ToArray();
            double[] ys1 = totalExpenseMonthsList.ToArray();
            string[] xAxisLabels = xAxisLabelsList.ToArray();

            var incomeLine = WpfPlot.Plot.AddScatter(xs, ys, Color.Blue, 3F);
            incomeLine.Label = "Income";
            var expenseLine = WpfPlot.Plot.AddScatter(xs, ys1, Color.Red, 3F);
            expenseLine.Label = "Expenses";
            WpfPlot.Plot.XAxis.TickDensity(0.01D);
            //WpfPlot.Plot.XAxis.AutomaticTickPositions(xs, xAxisLabels);
            WpfPlot.Plot.XAxis.ManualTickPositions(xs, xAxisLabels);
            WpfPlot.Plot.XLabel("Month");
            WpfPlot.Plot.YLabel("Amount");

            var legend = WpfPlot.Plot.Legend(true, Alignment.UpperRight);
            legend.FontSize = 20;
            legend.FontBold = true;
            WpfPlot.Plot.Title("Income vs. Expenses");
            WpfPlot.Refresh();

            var context = SystemGlobals.DataRepository.GetDataContext();

            if (context.GetTable<FundPeriodTotals>().Any())
            {
                WpfPlot.Visibility = Visibility.Visible;
            }
        }

        private List<FundPeriodTotals> GetChartData()
        {
            var resultTotals = new List<FundPeriodTotals>();
            var monthsLeft = 13;

            var currentDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddYears(-1);
            var context = SystemGlobals.DataRepository.GetDataContext();
            var periodsTable = context.GetTable<FundPeriodTotals>();

            while (monthsLeft > 0)
            {
                resultTotals.Add(GetTotalsForDate(currentDate, periodsTable));
                monthsLeft--;
                if (currentDate.Month == 12 && monthsLeft > 0)
                {
                    currentDate = new DateTime(currentDate.Year + 1, 1, 1);
                }
                else if (monthsLeft > 0)
                {
                    currentDate = currentDate.AddMonths(1);
                }
            }
            return resultTotals;
        }

        private FundPeriodTotals GetTotalsForDate(DateTime date, IQueryable<FundPeriodTotals> periodsTable)
        {
            var result = new FundPeriodTotals();
            date = date.AddMonths(1).AddDays(-1);
            var periods = periodsTable
                .Where(p => p.Date == date
                            && p.PeriodType == (int)PeriodTypes.MonthEnding);
            result.Date = date;
            result.TotalIncome = periods.Sum(p => p.TotalIncome);
            result.TotalExpenses = periods.Sum(p => p.TotalExpenses);
            return result;
        }
    }
}
