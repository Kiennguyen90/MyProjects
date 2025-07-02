import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CryptouserComponent } from './cryptouser.component';

describe('CryptouserComponent', () => {
  let component: CryptouserComponent;
  let fixture: ComponentFixture<CryptouserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CryptouserComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CryptouserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
