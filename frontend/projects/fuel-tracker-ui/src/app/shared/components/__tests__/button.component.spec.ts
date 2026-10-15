import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ButtonComponent } from '../button.component';

describe('ButtonComponent', () => {
  let component: ButtonComponent;
  let fixture: ComponentFixture<ButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ButtonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render the button with default variant', () => {
    const buttonElement = fixture.nativeElement.querySelector('button');
    expect(buttonElement.classList).toContain('bg-blue-600');
  });

  it('should render the button with secondary variant', () => {
    fixture.componentRef.setInput('variant', 'secondary');
    fixture.detectChanges();
    const buttonElement = fixture.nativeElement.querySelector('button');
    expect(buttonElement.classList).toContain('bg-gray-200');
  });

  it('should emit clicked event on click', () => {
    spyOn(component.clicked, 'emit');
    const buttonElement = fixture.nativeElement.querySelector('button');
    buttonElement.click();
    expect(component.clicked.emit).toHaveBeenCalled();
  });

  it('should be disabled when disabled input is true', () => {
    fixture.componentRef.setInput('disabled', true);
    fixture.detectChanges();
    const buttonElement = fixture.nativeElement.querySelector('button');
    expect(buttonElement.disabled).toBeTrue();
    expect(buttonElement.classList).toContain('opacity-50');
  });
});
