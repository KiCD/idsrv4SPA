import { TestBed } from '@angular/core/testing';

import { OidcStorageService } from './oidc-storage.service';

describe('OidcStorageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OidcStorageService = TestBed.get(OidcStorageService);
    expect(service).toBeTruthy();
  });
});
