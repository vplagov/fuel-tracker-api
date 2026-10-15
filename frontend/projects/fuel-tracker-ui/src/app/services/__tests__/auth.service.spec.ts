import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const routerSpy = { navigate: jasmine.createSpy('navigate') };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService, { provide: Router, useValue: routerSpy }],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should login and store user in localStorage', () => {
    const mockResponse = { id: '1', username: 'testuser', token: 'fake-token' };

    service.login({ username: 'testuser', password: 'password' }).subscribe((response) => {
      expect(response).toEqual(mockResponse);
      expect(service.currentUser()).toEqual(mockResponse);
      expect(localStorage.getItem('user')).toBeTruthy();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should logout and clear localStorage', () => {
    const userData = { id: '1', username: 'test', token: 'token' };
    localStorage.setItem('user', JSON.stringify(userData));
    // Force refresh state (normally happens in constructor, but we want to be sure)
    service.currentUser.set(userData);

    service.logout();

    expect(service.currentUser()).toBeNull();
    expect(localStorage.getItem('user')).toBeNull();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
  });
});
