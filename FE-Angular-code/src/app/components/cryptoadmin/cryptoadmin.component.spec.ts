import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CryptoadminComponent } from './cryptoadmin.component';

describe('AdminviewComponent', () => {
  let component: CryptoadminComponent;
  let fixture: ComponentFixture<CryptoadminComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CryptoadminComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CryptoadminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
