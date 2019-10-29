import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  get loggedIn() {
    return this.authService.isLoggedIn();
  }
  constructor( private authService: AuthenticationService, private router: Router) { }

  ngOnInit() {
    if (this.loggedIn) {
      console.log('already logged in..');
      return;
    }

    this.authService.startAuthentication();
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  logout() {
    this.authService.logout();
  }

}
