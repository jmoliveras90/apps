import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Category } from 'src/app/interfaces/category';
import { Expense } from 'src/app/interfaces/expense';

@Component({
  selector: 'app-expenses-modal',
  templateUrl: './expenses-modal.component.html',
  styleUrls: ['./expenses-modal.component.scss'],
})
export class ExpensesModalComponent {
  expenseForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ExpensesModalComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: { expense: Expense; categories: Category[] }
  ) {
    this.expenseForm = this.fb.group({
      id: [data.expense?.id || ''],
      description: [data.expense?.description || '', Validators.required],
      amount: [
        data.expense?.amount || 0,
        [Validators.required, Validators.min(0.01)],
      ],
      date: [
        data.expense?.date || new Date().toISOString().split('T')[0],
        Validators.required,
      ],
      categoryId: [data.expense?.categoryId || '', Validators.required],
      isRecurring: [false],
      endDate: [null],
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.expenseForm.valid) {
      this.dialogRef.close(this.expenseForm.value);
    }
  }
}
