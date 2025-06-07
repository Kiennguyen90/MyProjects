import { TestBed } from '@angular/core/testing';

import { CryptouserService } from './cryptouser.service';

describe('CryptouserService', () => {
  let service: CryptouserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CryptouserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
