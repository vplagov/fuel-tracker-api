import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DataService } from '../data.service';
import { environment } from '../../../environments/environment';
import { AverageConsumptionResponse } from '../../shared/models/api.models';

describe('DataService', () => {
  let service: DataService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [DataService],
    });
    service = TestBed.inject(DataService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch average consumption for a car', () => {
    const carId = '123-abc';
    const mockResponse: AverageConsumptionResponse = {
      averageConsumption: 7.5,
      totalDistanceKm: 1000,
      totalLiters: 75,
      calculatedFrom: '2023-01-01',
      calculatedTo: '2023-02-01',
      fullFillUpCount: 2,
      partialFillUpCount: 0,
      message: null,
    };

    service.getAverageConsumption(carId).subscribe((data) => {
      expect(data).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(
      `${environment.apiUrl}/cars/${carId}/statistics/average-consumption`,
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should fetch cars', () => {
    const mockCars = [
      { id: '1', name: 'Car 1' },
      { id: '2', name: 'Car 2' },
    ];

    service.getCars().subscribe((cars) => {
      expect(cars.length).toBe(2);
      expect(cars).toEqual(mockCars);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/cars`);
    expect(req.request.method).toBe('GET');
    req.flush(mockCars);
  });
});
