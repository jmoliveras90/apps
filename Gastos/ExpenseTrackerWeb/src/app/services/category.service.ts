import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BaseCategory, Category } from '../interfaces/category';
import { ApiBaseService } from './api-base.service';

@Injectable({
  providedIn: 'root',
})
export class CategoryService extends ApiBaseService {
  private suffix = 'categories';
  private fullUrl = `${this.apiUrl}/${this.suffix}`;

  constructor(private http: HttpClient) {
    super();
  }

  // Obtener todas las categorías
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.fullUrl);
  }

  // Crear una nueva categoría
  createCategory(category: BaseCategory): Observable<Category> {
    return this.http.post<Category>(this.fullUrl, category);
  }

  // Actualizar una categoría existente
  updateCategory(category: Category): Observable<Category> {
    return this.http.put<Category>(`${this.fullUrl}/${category.id}`, category);
  }

  // Eliminar una categoría
  deleteCategory(categoryId: string): Observable<void> {
    return this.http.delete<void>(`${this.fullUrl}/${categoryId}`);
  }
}
