import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../services/data.service';
import { CarResponse, AverageConsumptionResponse } from '../shared/models/api.models';

@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <div class="flex items-center gap-4">
        <h1 class="text-2xl font-bold">Statistics</h1>
      </div>

      <div
        class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
      >
        <div class="max-w-md">
          <label for="carSelect" class="block text-sm font-medium mb-1">Select Car</label>
          <select
            id="carSelect"
            [(ngModel)]="selectedCarId"
            (ngModelChange)="onCarSelected()"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 rounded-md"
          >
            <option [ngValue]="null">Select a car</option>
            <option *ngFor="let car of cars" [value]="car.id">{{ car.name }}</option>
          </select>
        </div>
      </div>

      <div *ngIf="loading" class="text-center py-8">
        <span class="text-gray-500">Loading statistics...</span>
      </div>

      <div *ngIf="selectedCarId && !loading && !statistics" class="text-center py-8">
        <span class="text-gray-500">Failed to load statistics for the selected car.</span>
      </div>

      <div
        *ngIf="selectedCarId && statistics && statistics.message"
        class="bg-yellow-50 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-200 p-4 rounded-lg"
      >
        <p>{{ statistics.message }}</p>
      </div>

      <div *ngIf="statistics" class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div
          class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
        >
          <h3 class="text-lg font-medium text-gray-500 dark:text-gray-400">Average Consumption</h3>
          <p class="mt-2 text-3xl font-bold">
            <ng-container *ngIf="statistics.averageConsumption !== null">
              {{ statistics.averageConsumption | number: '1.2-2' }}
              <span class="text-xl font-normal text-gray-500 dark:text-gray-400">L/100km</span>
            </ng-container>
            <ng-container *ngIf="statistics.averageConsumption === null">
              <span class="text-xl font-normal text-gray-500 dark:text-gray-400"
                >Not enough data</span
              >
            </ng-container>
          </p>
        </div>

        <div
          class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
        >
          <h3 class="text-lg font-medium text-gray-500 dark:text-gray-400">Total Distance</h3>
          <p class="mt-2 text-3xl font-bold">
            <ng-container *ngIf="statistics.totalDistanceKm !== null">
              {{ statistics.totalDistanceKm | number: '1.0-0' }}
              <span class="text-xl font-normal text-gray-500 dark:text-gray-400">km</span>
            </ng-container>
            <ng-container *ngIf="statistics.totalDistanceKm === null">
              <span class="text-xl font-normal text-gray-500 dark:text-gray-400">N/A</span>
            </ng-container>
          </p>
        </div>

        <div
          class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
        >
          <h3 class="text-lg font-medium text-gray-500 dark:text-gray-400">Total Fuel</h3>
          <p class="mt-2 text-3xl font-bold">
            <ng-container *ngIf="statistics.totalLiters !== null">
              {{ statistics.totalLiters | number: '1.1-1' }}
              <span class="text-xl font-normal text-gray-500 dark:text-gray-400">L</span>
            </ng-container>
            <ng-container *ngIf="statistics.totalLiters === null">
              <span class="text-xl font-normal text-gray-500 dark:text-gray-400">N/A</span>
            </ng-container>
          </p>
        </div>

        <div
          class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
        >
          <h3 class="text-lg font-medium text-gray-500 dark:text-gray-400">Calculation Period</h3>
          <div class="mt-2 text-sm">
            <p *ngIf="statistics.calculatedFrom">
              From: <strong>{{ statistics.calculatedFrom | date: 'mediumDate' }}</strong>
            </p>
            <p *ngIf="statistics.calculatedTo">
              To: <strong>{{ statistics.calculatedTo | date: 'mediumDate' }}</strong>
            </p>
            <p
              *ngIf="!statistics.calculatedFrom && !statistics.calculatedTo"
              class="text-gray-500 italic"
            >
              Not enough data
            </p>
          </div>
        </div>

        <div
          class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
        >
          <h3 class="text-lg font-medium text-gray-500 dark:text-gray-400">Fill-up Counts</h3>
          <div class="mt-2 flex space-x-8">
            <div>
              <p class="text-sm text-gray-500">Full</p>
              <p class="text-xl font-semibold">{{ statistics.fullFillUpCount }}</p>
            </div>
            <div>
              <p class="text-sm text-gray-500">Partial</p>
              <p class="text-xl font-semibold">{{ statistics.partialFillUpCount }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class StatisticsComponent implements OnInit {
  cars: CarResponse[] = [];
  selectedCarId: string | null = null;
  statistics: AverageConsumptionResponse | null = null;
  loading = false;

  private dataService = inject(DataService);

  ngOnInit(): void {
    this.dataService.getCars().subscribe({
      next: (cars) => {
        this.cars = cars;
        if (this.cars.length === 1) {
          this.selectedCarId = this.cars[0].id;
          this.loadStatistics(this.selectedCarId);
        }
      },
    });
  }

  onCarSelected(): void {
    if (this.selectedCarId) {
      this.loadStatistics(this.selectedCarId);
    } else {
      this.statistics = null;
    }
  }

  loadStatistics(carId: string): void {
    this.loading = true;
    this.dataService.getAverageConsumption(carId).subscribe({
      next: (stats) => {
        this.statistics = stats;
        this.loading = false;
      },
      error: () => {
        this.statistics = null;
        this.loading = false;
      },
    });
  }
}
