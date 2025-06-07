import { TestBed } from '@angular/core/testing';

import { CryptoadminService } from './cryptoadmin.service';

describe('CryptoadminService', () => {
  let service: CryptoadminService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CryptoadminService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
