import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { Category } from 'src/app/interfaces/category';
import { Expense } from 'src/app/interfaces/expense';
import { CategoryService } from 'src/app/services/category.service';
import { ExpenseService } from 'src/app/services/expense.service';
import { ExpensesModalComponent } from './modal/expenses-modal.component';
import * as moment from 'moment';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.scss'],
})
export class ExpensesComponent implements OnInit {
  displayedColumns: string[] = [
    'description',
    'amount',
    'date',
    'categoryId',
    'actions',
  ];
  dataSource = new MatTableDataSource<Expense>();
  filteredExpenses: Expense[] = [];
  categories: Category[] = [];
  totalAmount: number = 0;
  countUpOptions = {
    decimalPlaces: 2,
    decimal: ',',
    separator: '.',
    suffix: '€',
  };

  constructor(
    private expenseService: ExpenseService,
    private categoryService: CategoryService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadExpenses();
    this.loadCategories();
  }

  loadExpenses(): void {
    this.expenseService.getExpenses().subscribe((expenses) => {
      this.dataSource.data = expenses;
      this.filteredExpenses = expenses;
      this.calculateTotalAmount();
    });
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe((categories) => {
      this.categories = categories;
    });
  }

  addExpense(): void {
    const dialogRef = this.dialog.open(ExpensesModalComponent, {
      width: '400px',
      data: {
        gasto: null,
        categories: this.categories,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.expenseService.createExpense(result).subscribe(() => {
          this.loadExpenses();
        });
      }
    });
  }

  editExpense(expense: Expense): void {
    const dialogRef = this.dialog.open(ExpensesModalComponent, {
      width: '400px',
      data: {
        expense,
        categories: this.categories,
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.expenseService.updateExpense(result).subscribe(() => {
          this.loadExpenses();
        });
      }
    });
  }

  deleteExpense(expenseId: string): void {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.expenseService.deleteExpense(expenseId).subscribe(() => {
        this.loadExpenses();
      });
    }
  }

  onDateChanged(event: { month: number; year: number }) {
    const { month, year } = event;
    this.filterExpensesByMonthAndYear(month, year);
  }

  // Filtra los gastos por mes y año
  filterExpensesByMonthAndYear(month: number, year: number) {
    this.filteredExpenses = this.dataSource.data.filter((expense) => {
      const expenseDate = moment(expense.date);
      return expenseDate.month() + 1 === month && expenseDate.year() === year;
    });

    this.calculateTotalAmount();
  }

  calculateTotalAmount(): void {
    (this.totalAmount = 9999), 99;
  }
}
