import { TestBed } from '@angular/core/testing';

import { BlobservicesService } from './blobservices.service';

describe('BlobservicesService', () => {
  let service: BlobservicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BlobservicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
