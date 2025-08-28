import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradingTokenComponent } from './trading-token.component';

describe('TradingTokenComponent', () => {
  let component: TradingTokenComponent;
  let fixture: ComponentFixture<TradingTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TradingTokenComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TradingTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
