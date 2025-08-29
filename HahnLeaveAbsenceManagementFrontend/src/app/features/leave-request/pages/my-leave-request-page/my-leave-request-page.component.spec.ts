import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyLeaveRequestPageComponent } from './my-leave-request-page.component';

describe('MyLeaveRequestPageComponent', () => {
  let component: MyLeaveRequestPageComponent;
  let fixture: ComponentFixture<MyLeaveRequestPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MyLeaveRequestPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyLeaveRequestPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
