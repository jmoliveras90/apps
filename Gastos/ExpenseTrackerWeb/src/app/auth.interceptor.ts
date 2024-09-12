import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    // Obtener el token desde el AuthService
    const token = this.authService.getUserFromLocalStorage()?.token || null;

    // Clonar la solicitud y agregar el token al encabezado Authorization si existe
    if (token) {
      const clonedRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });

      // Pasar la solicitud clonada al siguiente manejador
      return next.handle(clonedRequest);
    }

    // Si no hay token, pasar la solicitud original
    return next.handle(req);
  }
}
