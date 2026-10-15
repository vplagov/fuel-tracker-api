import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CarRequest,
  CarResponse,
  FuelEntryRequest,
  FuelEntryResponse,
  AverageConsumptionResponse,
} from '../shared/models/api.models';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  // Cars
  getCars(): Observable<CarResponse[]> {
    return this.http.get<CarResponse[]>(`${this.apiUrl}/cars`);
  }

  getCar(id: string): Observable<CarResponse> {
    return this.http.get<CarResponse>(`${this.apiUrl}/cars/${id}`);
  }

  addCar(request: CarRequest): Observable<CarResponse> {
    return this.http.post<CarResponse>(`${this.apiUrl}/cars`, request);
  }

  // Statistics
  getAverageConsumption(carId: string): Observable<AverageConsumptionResponse> {
    return this.http.get<AverageConsumptionResponse>(
      `${this.apiUrl}/cars/${carId}/statistics/average-consumption`,
    );
  }

  // Fuel Entries
  getFuelEntries(carId: string): Observable<FuelEntryResponse[]> {
    return this.http.get<FuelEntryResponse[]>(`${this.apiUrl}/cars/${carId}/fuel-entries`);
  }

  addFuelEntry(carId: string, request: FuelEntryRequest): Observable<FuelEntryResponse> {
    return this.http.post<FuelEntryResponse>(`${this.apiUrl}/cars/${carId}/fuel-entries`, request);
  }

  updateFuelEntry(fuelEntryId: string, request: FuelEntryRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/fuel-entries/${fuelEntryId}`, request);
  }

  deleteFuelEntry(fuelEntryId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/fuel-entries/${fuelEntryId}`);
  }
}
