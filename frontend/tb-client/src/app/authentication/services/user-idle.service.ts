import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

const IDLE_TIME_SECONDS = 60 * 30;

@Injectable({
  providedIn: 'root'
})
export class UserIdleService {
  userActivity;
  userInactive: Subject<any> = new Subject();
  idleStatus: boolean;

  constructor() {}

  init() {
    this.startTimeout();
    this.userInactive.subscribe(() => {
      this.idleStatus = true;
    });
  }

  startTimeout() {
    this.userActivity = setTimeout(() => this.userInactive.next(undefined), (IDLE_TIME_SECONDS * 1000));
  }

  resetTimeout() {
    this.idleStatus = false;
    clearTimeout(this.userActivity);
    this.startTimeout();
  }

  stopWatching() {
    clearTimeout();
  }

  isUserIdle() {
    return this.idleStatus;
  }
}
