import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { StatisticsComponent } from '../statistics.component';
import { DataService } from '../../services/data.service';
import { AverageConsumptionResponse } from '../../shared/models/api.models';

describe('StatisticsComponent', () => {
  let component: StatisticsComponent;
  let fixture: ComponentFixture<StatisticsComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('DataService', ['getCars', 'getAverageConsumption']);

    await TestBed.configureTestingModule({
      imports: [StatisticsComponent, HttpClientTestingModule],
      providers: [{ provide: DataService, useValue: spy }],
    }).compileComponents();

    dataServiceSpy = TestBed.inject(DataService) as jasmine.SpyObj<DataService>;
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StatisticsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    dataServiceSpy.getCars.and.returnValue(of([]));
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should auto-select the car if only one is returned and load its statistics', () => {
    const mockCars = [{ id: '1', name: 'Car 1' }];
    const mockStats: AverageConsumptionResponse = {
      averageConsumption: 8.0,
      totalDistanceKm: 500,
      totalLiters: 40,
      calculatedFrom: '2023-01-01',
      calculatedTo: '2023-01-15',
      fullFillUpCount: 1,
      partialFillUpCount: 0,
      message: null,
    };

    dataServiceSpy.getCars.and.returnValue(of(mockCars));
    dataServiceSpy.getAverageConsumption.and.returnValue(of(mockStats));

    fixture.detectChanges(); // triggers ngOnInit

    expect(dataServiceSpy.getCars).toHaveBeenCalled();
    expect(component.selectedCarId).toBe('1');
    expect(dataServiceSpy.getAverageConsumption).toHaveBeenCalledWith('1');
    expect(component.statistics).toEqual(mockStats);
    expect(component.loading).toBeFalse();
  });

  it('should not auto-select if multiple cars are returned', () => {
    const mockCars = [
      { id: '1', name: 'Car 1' },
      { id: '2', name: 'Car 2' },
    ];
    dataServiceSpy.getCars.and.returnValue(of(mockCars));

    fixture.detectChanges();

    expect(component.selectedCarId).toBeNull();
    expect(dataServiceSpy.getAverageConsumption).not.toHaveBeenCalled();
  });

  it('should load statistics manually when a car is selected', () => {
    const mockCars = [
      { id: '1', name: 'Car 1' },
      { id: '2', name: 'Car 2' },
    ];
    dataServiceSpy.getCars.and.returnValue(of(mockCars));
    fixture.detectChanges();

    const mockStats: AverageConsumptionResponse = {
      averageConsumption: 6.5,
      totalDistanceKm: 600,
      totalLiters: 39,
      calculatedFrom: '2023-01-01',
      calculatedTo: '2023-01-15',
      fullFillUpCount: 1,
      partialFillUpCount: 0,
      message: null,
    };

    dataServiceSpy.getAverageConsumption.and.returnValue(of(mockStats));

    component.selectedCarId = '2';
    component.onCarSelected();

    expect(dataServiceSpy.getAverageConsumption).toHaveBeenCalledWith('2');
    expect(component.statistics).toEqual(mockStats);
  });

  it('should clear statistics if no car is selected in dropdown', () => {
    const mockCars = [
      { id: '1', name: 'Car 1' },
      { id: '2', name: 'Car 2' },
    ];
    dataServiceSpy.getCars.and.returnValue(of(mockCars));
    fixture.detectChanges();

    component.statistics = {
      averageConsumption: 5,
      totalDistanceKm: 100,
      totalLiters: 5,
      calculatedFrom: '2023-01-01',
      calculatedTo: '2023-01-15',
      fullFillUpCount: 1,
      partialFillUpCount: 0,
      message: null,
    };

    component.selectedCarId = null;
    component.onCarSelected();

    expect(component.statistics).toBeNull();
    expect(dataServiceSpy.getAverageConsumption).not.toHaveBeenCalled();
  });
});
