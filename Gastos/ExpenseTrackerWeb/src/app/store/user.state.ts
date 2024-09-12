import { State, Action, StateContext, Selector } from '@ngxs/store';
import { User } from '../interfaces/user';

export class SetUser {
  static readonly type = '[Auth] Set User';
  constructor(public payload: User) {}
}

export class ClearUser {
  static readonly type = '[Auth] Clear User';
}

@State<User | null>({
  name: 'user',
  defaults: null,
})
export class UserState {
  @Selector()
  static getUser(state: User | null) {
    return state;
  }

  @Selector()
  static isLoggedIn(state: User | null): boolean {
    return !!state;
  }

  @Action(SetUser)
  setUser(ctx: StateContext<User | null>, action: SetUser) {
    ctx.setState(action.payload);
  }

  @Action(ClearUser)
  clearUser(ctx: StateContext<User | null>) {
    ctx.setState(null);
  }
}
