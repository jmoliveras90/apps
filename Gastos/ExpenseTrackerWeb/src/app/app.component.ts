import { Component, ViewChild } from '@angular/core';
import { Store } from '@ngxs/store';
import { ClearUser, SetUser, UserState } from './store/user.state';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from './services/auth.service';
import { MatSidenav } from '@angular/material/sidenav';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  isLoggedIn$: Observable<boolean>;
  sidenavExpanded = false;
  @ViewChild('sidenav') sidenav!: MatSidenav;

  constructor(
    private store: Store,
    private authService: AuthService,
    private router: Router
  ) {
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
