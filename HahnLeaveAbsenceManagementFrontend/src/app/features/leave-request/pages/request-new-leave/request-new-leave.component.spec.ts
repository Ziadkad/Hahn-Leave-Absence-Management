import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestNewLeaveComponent } from './request-new-leave.component';

describe('RequestNewLeaveComponent', () => {
  let component: RequestNewLeaveComponent;
  let fixture: ComponentFixture<RequestNewLeaveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RequestNewLeaveComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestNewLeaveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
