import { Component, OnInit } from '@angular/core';
import { UserManager } from 'oidc-client';
import { getClientSettings } from '../../services/oid-client-settings';

@Component({
  selector: 'app-silent-refresh',
  templateUrl: './silent-refresh.component.html',
  styleUrls: ['./silent-refresh.component.scss']
})
export class SilentRefreshComponent implements OnInit {
  private mgr: UserManager = new UserManager(getClientSettings());
  constructor() {

   }

  ngOnInit() {
    this.mgr.signinSilentCallback()
          .catch((err) => {
              console.log(err);
          });
  }

}
