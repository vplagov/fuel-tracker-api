import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WakingUpComponent } from '../waking-up.component';

describe('WakingUpComponent', () => {
  let component: WakingUpComponent;
  let fixture: ComponentFixture<WakingUpComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WakingUpComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(WakingUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render the default title and message', () => {
    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Waking up the backend');
    expect(text).toContain('This may take a few seconds as the server starts up...');
  });

  it('should render custom title and message inputs', () => {
    fixture.componentRef.setInput('title', 'Loading');
    fixture.componentRef.setInput('message', 'Please wait');
    fixture.detectChanges();
    const text = fixture.nativeElement.textContent;
    expect(text).toContain('Loading');
    expect(text).toContain('Please wait');
  });
});
