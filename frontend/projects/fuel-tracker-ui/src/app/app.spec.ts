import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { App } from './app';
import { AuthService } from './services/auth.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('App', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['verifySession'], {
      isVerifying: signal(false),
    });
    authServiceSpy.verifySession.and.returnValue(of(true));

    await TestBed.configureTestingModule({
      imports: [App, HttpClientTestingModule],
      providers: [{ provide: AuthService, useValue: authServiceSpy }],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
    expect(app).toBeTruthy();
    expect(authServiceSpy.verifySession).toHaveBeenCalled();
  });

  it('should have the correct title', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('fuel-tracker-ui');
  });
});
