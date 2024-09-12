import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { CategoryService } from 'src/app/services/category.service';
import { Category } from 'src/app/interfaces/category';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { CategoriesModalComponent } from './modal/categories-modal.component';

@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss'],
})
export class CategoriesComponent implements OnInit {
  displayedColumns: string[] = ['name', 'actions'];
  dataSource = new MatTableDataSource<Category>();
  newCategory: string = '';

  constructor(
    private categoryService: CategoryService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  // Cargar las categorías y asignar los datos a la tabla
  loadCategories(): void {
    this.categoryService.getCategories().subscribe((categories) => {
      this.dataSource.data = categories;
    });
  }
  addCategory(): void {
    const dialogRef = this.dialog.open(CategoriesModalComponent, {
      width: '300px',
      data: { category: null },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.categoryService.createCategory(result).subscribe(() => {
          this.loadCategories();
        });
      }
    });
  }

  editCategory(category: Category): void {
    const dialogRef = this.dialog.open(CategoriesModalComponent, {
      width: '300px',
      data: { category: category },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.categoryService.updateCategory(result).subscribe(() => {
          this.loadCategories();
        });
      }
    });
  }

  deleteCategory(categoryId: string): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        message:
          'Se eliminará la categoría y los gastos asociados a ella, ¿está seguro?',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.categoryService.deleteCategory(categoryId).subscribe(() => {
          this.loadCategories();
        });
      }
    });
  }
}
