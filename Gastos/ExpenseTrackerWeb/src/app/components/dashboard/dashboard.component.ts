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
    const currentMonth = moment().month() + 1; // Mes actual
    const currentYear = moment().year();

    // Filtrar los gastos del mes actual
    const expensesThisMonth = expenses.filter((expense) => {
      const expenseDate = moment(expense.date);
      return (
        expenseDate.month() + 1 === currentMonth &&
        expenseDate.year() === currentYear
      );
    });

    // Procesar los gastos por categoría para el Pie Chart
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

    // Sumar los gastos por mes
    expenses.forEach((expense) => {
      const expenseDate = moment(expense.date); // Suponemos que `expense.date` es una fecha válida
      const monthName = months[expenseDate.month()]; // Obtener el nombre del mes
      monthlyTotals[monthName] += expense.amount;
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
