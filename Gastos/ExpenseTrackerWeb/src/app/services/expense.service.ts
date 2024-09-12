import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Expense } from '../interfaces/expense';
import { ApiBaseService } from './api-base.service';

@Injectable({
  providedIn: 'root',
})
export class ExpenseService extends ApiBaseService {
  private suffix = 'expenses';
  private fullUrl = `${this.apiUrl}/${this.suffix}`;

  constructor(private http: HttpClient) {
    super();
  }

  getExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>(this.fullUrl);
  }

  createExpense(Expense: Expense): Observable<Expense> {
    return this.http.post<Expense>(this.fullUrl, Expense);
  }

  updateExpense(Expense: Expense): Observable<Expense> {
    return this.http.put<Expense>(`${this.fullUrl}/${Expense.id}`, Expense);
  }

  deleteExpense(ExpenseId: string): Observable<void> {
    return this.http.delete<void>(`${this.fullUrl}/${ExpenseId}`);
  }
}
