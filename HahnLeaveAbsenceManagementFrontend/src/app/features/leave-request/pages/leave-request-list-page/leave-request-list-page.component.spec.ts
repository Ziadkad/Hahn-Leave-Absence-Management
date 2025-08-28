import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveRequestListPageComponent } from './leave-request-list-page.component';

describe('LeaveRequestListPageComponent', () => {
  let component: LeaveRequestListPageComponent;
  let fixture: ComponentFixture<LeaveRequestListPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LeaveRequestListPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LeaveRequestListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
