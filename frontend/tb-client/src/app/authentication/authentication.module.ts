import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './components/login/login.component';
import { SilentRefreshComponent } from './components/silent-refresh/silent-refresh.component';
import { ErrorComponent } from './components/error/error.component';
import { CallbackComponent } from './components/callback/callback.component';



@NgModule({
  declarations: [LoginComponent, SilentRefreshComponent, ErrorComponent, CallbackComponent],
  imports: [
    CommonModule
  ]
})
export class AuthenticationModule { }
