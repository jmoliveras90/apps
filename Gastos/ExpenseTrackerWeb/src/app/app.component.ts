import { Component, ViewChild } from '@angular/core';
import { Store } from '@ngxs/store';
import { ClearUser, SetUser, UserState } from './store/user.state';
import { NavigationEnd, Router } from '@angular/router';
import { Observable, filter } from 'rxjs';
import { AuthService } from './services/auth.service';
import { MatSidenav } from '@angular/material/sidenav';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  isLoggedIn$: Observable<boolean>;
  isMobile: boolean = false;

  sidenavExpanded = false;
  @ViewChild('sidenav') sidenav!: MatSidenav;

  constructor(
    private store: Store,
    private authService: AuthService,
    private router: Router,
    private breakpointObserver: BreakpointObserver
  ) {
    // Escuchar los eventos de navegación
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd)) // Filtrar solo NavigationEnd
      .subscribe(() => {
        if (this.sidenavExpanded) {
          this.sidenavExpanded = false;
        }
      });

    this.breakpointObserver
      .observe([Breakpoints.Handset])
      .subscribe((result) => {
        this.isMobile = result.matches; // Si la pantalla es pequeña, cambiar isMobile a true
      });

    // Recuperar el usuario del LocalStorage si existe
    const storedUser = this.authService.getUserFromLocalStorage();
    if (storedUser) {
      this.store.dispatch(new SetUser(storedUser));
    } else {
      this.store.dispatch(new ClearUser());
    }
    this.isLoggedIn$ = this.store.select(UserState.isLoggedIn);
  }

  logout(): void {
    this.authService.logout();
    this.store.dispatch(new ClearUser());
    this.router.navigate(['/login']);
  }

  // Expande el sidenav al hacer hover
  expandSidenav(): void {
    this.sidenavExpanded = true;
  }

  // Contrae el sidenav al quitar el hover
  collapseSidenav(): void {
    this.sidenavExpanded = false;
  }
}
