import { AuthenticationService } from 'src/app/authentication/services/authentication.service';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'tb-client';
  constructor(private authService: AuthenticationService, private router: Router) {

  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }
}
