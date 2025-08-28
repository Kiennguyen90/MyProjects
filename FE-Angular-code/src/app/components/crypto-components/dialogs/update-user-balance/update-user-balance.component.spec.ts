import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateUserBalanceComponent } from './update-user-balance.component';

describe('UpdateUserBalanceComponent', () => {
  let component: UpdateUserBalanceComponent;
  let fixture: ComponentFixture<UpdateUserBalanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateUserBalanceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UpdateUserBalanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
