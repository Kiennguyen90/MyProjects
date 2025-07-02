import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EdituserDialogComponent } from './edituser-dialog.component';

describe('EdituserDialogComponent', () => {
  let component: EdituserDialogComponent;
  let fixture: ComponentFixture<EdituserDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EdituserDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EdituserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
