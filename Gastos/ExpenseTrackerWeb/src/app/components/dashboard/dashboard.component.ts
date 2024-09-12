import { Component } from '@angular/core';
import { Store } from '@ngxs/store';
import { UserState } from 'src/app/store/user.state';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
  user$ = this.store.select(UserState.getUser);

  constructor(private store: Store) {}
}
