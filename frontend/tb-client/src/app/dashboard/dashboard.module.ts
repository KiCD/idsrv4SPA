import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProtectedDataComponent } from './components/protected-data/protected-data.component';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [DashboardComponent, ProtectedDataComponent],
  imports: [
    CommonModule,
    RouterModule,
    HttpClientModule
  ]
})
export class DashboardModule { }
