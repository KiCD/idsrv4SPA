import { DataService } from './../../services/data.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-protected-data',
  templateUrl: './protected-data.component.html',
  styleUrls: ['./protected-data.component.scss']
})
export class ProtectedDataComponent implements OnInit {
  protectedData: any;
  constructor(private dataService: DataService) { }

  ngOnInit() {
    this.dataService.getData().subscribe(result => {
      this.protectedData = result;
    });
  }

}
