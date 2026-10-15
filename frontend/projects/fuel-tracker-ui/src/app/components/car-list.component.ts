import { Component, OnInit, inject } from '@angular/core';
import { DataService } from '../services/data.service';
import { CarResponse } from '../shared/models/api.models';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-car-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="space-y-6">
      <div class="flex justify-between items-center">
        <h1 class="text-2xl font-bold">My Cars</h1>
        <button
          (click)="showAddForm = !showAddForm"
          class="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 transition"
        >
          {{ showAddForm ? 'Cancel' : 'Add Car' }}
        </button>
      </div>

      <div
        *ngIf="showAddForm"
        class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
      >
        <form (submit)="addCar()" class="flex gap-4">
          <input
            type="text"
            [(ngModel)]="newCarName"
            name="newCarName"
            required
            placeholder="Car Name"
            class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 rounded-md"
          />
          <button
            type="submit"
            [disabled]="!newCarName"
            class="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 disabled:opacity-50"
          >
            Save
          </button>
        </form>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div
          *ngFor="let car of cars"
          class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 hover:border-blue-500 transition-colors"
        >
          <h2 class="text-xl font-semibold mb-4">{{ car.name }}</h2>
          <a
            [routerLink]="['/cars', car.id]"
            class="text-blue-600 dark:text-blue-400 font-medium hover:underline"
          >
            View Fuel Entries &rarr;
          </a>
        </div>
        <div
          *ngIf="cars.length === 0 && !loading"
          class="col-span-full text-center py-12 text-gray-500"
        >
          No cars found. Add your first car to start tracking!
        </div>
      </div>
    </div>
  `,
})
export class CarListComponent implements OnInit {
  cars: CarResponse[] = [];
  showAddForm = false;
  newCarName = '';
  loading = true;

  private dataService = inject(DataService);

  ngOnInit() {
    this.loadCars();
  }

  loadCars() {
    this.dataService.getCars().subscribe({
      next: (cars) => {
        this.cars = cars;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  addCar() {
    if (!this.newCarName) return;
    this.dataService.addCar({ name: this.newCarName }).subscribe({
      next: (car) => {
        this.cars.push(car);
        this.newCarName = '';
        this.showAddForm = false;
      },
    });
  }
}
