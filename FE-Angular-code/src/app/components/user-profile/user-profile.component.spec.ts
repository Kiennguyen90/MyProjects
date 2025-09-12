import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersettingComponent } from './user-profile.component';

describe('UsersettingComponent', () => {
  let component: UsersettingComponent;
  let fixture: ComponentFixture<UsersettingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UsersettingComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UsersettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
