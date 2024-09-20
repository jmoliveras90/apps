import { Component, OnInit } from '@angular/core';
import { ChartConfiguration } from 'chart.js';
import * as moment from 'moment';
import { Expense } from 'src/app/interfaces/expense';
import { ExpenseService } from 'src/app/services/expense.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  public pieChartData!: ChartConfiguration['data'];
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
  };

  public barChartData!: ChartConfiguration['data'];
  public barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {},
      y: {
        beginAtZero: true,
      },
    },
  };

  currentMonthExpenses = 0;
  yearExpenses = 0;

  constructor(private expensesService: ExpenseService) {}

  ngOnInit(): void {
    this.loadExpensesData();
  }

  // Obtener los gastos y configurar los gráficos
  loadExpensesData(): void {
    this.expensesService.getExpenses().subscribe((expenses) => {
      this.processExpenses(expenses);
      this.processMonthlyExpenses(expenses);
    });
  }

  processExpenses(expenses: Expense[]): void {
    const currentMonth = moment().month() + 1;
    const currentYear = moment().year();
    const expensesThisMonth = expenses.filter((expense) => {
      const expenseDate = moment(expense.date);
      const expenseEndDate = expense.endDate ? moment(expense.endDate) : null;

      if (!expense.isRecurring) {
        return (
          expenseDate.month() + 1 === currentMonth &&
          expenseDate.year() === currentYear
        );
      }

      if (expense.isRecurring) {
        if (!expense.endDate) {
          return expenseDate.isBefore(
            moment({ year: currentYear, month: currentMonth - 1 })
          );
        }

        return (
          expenseDate.isBefore(
            moment({ year: currentYear, month: currentMonth - 1 })
          ) &&
          expenseEndDate?.isSameOrAfter(
            moment({ year: currentYear, month: currentMonth - 1 })
          )
        );
      }

      return false;
    });

    const categoryTotals: { [category: string]: number } = {};
    expensesThisMonth.forEach((expense) => {
      if (!categoryTotals[expense.categoryName]) {
        categoryTotals[expense.categoryName] = 0;
      }
      categoryTotals[expense.categoryName] += expense.amount;
    });

    this.pieChartData = {
      labels: Object.keys(categoryTotals), // Categorías
      datasets: [
        {
          data: Object.values(categoryTotals),
          backgroundColor: ['#ff6384', '#36a2eb', '#cc65fe', '#ffce56'],
        },
      ],
    };

    const monthlyTotals: { [month: string]: number } = {};

    this.barChartData = {
      labels: Object.keys(monthlyTotals),
      datasets: [
        {
          label: 'Gastos',
          data: Object.values(monthlyTotals),
          backgroundColor: '#36a2eb',
        },
      ],
    };

    this.currentMonthExpenses = expensesThisMonth
      .map((e) => e.amount)
      .reduce((a, b) => a + b);
  }

  processMonthlyExpenses(expenses: Expense[]): void {
    const monthlyTotals: { [month: string]: number } = {};

    // Inicializar los meses del año
    const months = [
      'Enero',
      'Febrero',
      'Marzo',
      'Abril',
      'Mayo',
      'Junio',
      'Julio',
      'Agosto',
      'Septiembre',
      'Octubre',
      'Noviembre',
      'Diciembre',
    ];

    months.forEach((month) => {
      monthlyTotals[month] = 0;
    });

    expenses.forEach((expense) => {
      const expenseDate = moment(expense.date);

      if (expense.isRecurring) {
        if (
          !expense.endDate ||
          moment(expense.endDate).year() > moment().year()
        ) {
          for (let i = expenseDate.month(); i < months.length; i++) {
            const monthName = months[i];

            monthlyTotals[monthName] += expense.amount;
          }
        } else {
          const expenseEndDate = moment(expense.endDate);

          for (let i = expenseDate.month(); i <= expenseEndDate.month(); i++) {
            const monthName = months[i];

            monthlyTotals[monthName] += expense.amount;
          }
        }
      } else {
        const monthName = months[expenseDate.month()];

        monthlyTotals[monthName] += expense.amount;
      }
    });

    // Asignar los datos al gráfico de barras
    this.barChartData = {
      labels: months, // Los nombres de los meses
      datasets: [
        {
          label: 'Gastos',
          data: Object.values(monthlyTotals), // Los valores de los gastos por mes
          backgroundColor: '#36a2eb',
        },
      ],
    };

    this.yearExpenses = Object.values(monthlyTotals).reduce((a, b) => a + b);
  }
}
