import { CallbackComponent } from './authentication/components/callback/callback.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './authentication/components/login/login.component';
import { SilentRefreshComponent } from './authentication/components/silent-refresh/silent-refresh.component';
import { ErrorComponent } from './authentication/components/error/error.component';
import { DashboardComponent } from './dashboard/components/dashboard/dashboard.component';


const routes: Routes = [
  { path: 'callback', component: CallbackComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'login', component: LoginComponent },
  { path: 'silent-refresh', component: SilentRefreshComponent },
  { path: 'error', component: ErrorComponent },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: 'dashboard', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
