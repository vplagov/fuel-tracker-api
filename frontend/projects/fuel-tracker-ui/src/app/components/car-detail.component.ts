import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DataService } from '../services/data.service';
import { CarResponse, FuelEntryResponse, FuelEntryRequest } from '../shared/models/api.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-car-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="space-y-6" *ngIf="car">
      <div class="flex items-center gap-4">
        <a routerLink="/cars" class="text-blue-600 dark:text-blue-400 hover:underline"
          >&larr; Back</a
        >
        <h1 class="text-2xl font-bold">{{ car.name }} - Fuel History</h1>
      </div>

      <!-- Add Fuel Entry Form -->
      <div
        class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
      >
        <h2 class="text-lg font-semibold mb-4">
          {{ editingId ? 'Edit Entry' : 'Add New Fuel Entry' }}
        </h2>
        <form (submit)="saveEntry()" class="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div>
            <label for="date" class="block text-sm font-medium mb-1">Date</label>
            <input
              type="date"
              [(ngModel)]="newEntry.date"
              name="date"
              id="date"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 rounded-md"
            />
          </div>
          <div>
            <label for="odometer" class="block text-sm font-medium mb-1">Odometer</label>
            <input
              type="text"
              [ngModel]="formatOdometer(newEntry.odometer)"
              (ngModelChange)="onOdometerChange($event)"
              name="odometer"
              id="odometer"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 rounded-md"
            />
          </div>
          <div>
            <label for="liters" class="block text-sm font-medium mb-1">Liters</label>
            <input
              type="number"
              step="0.01"
              [(ngModel)]="newEntry.liters"
              name="liters"
              id="liters"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 rounded-md"
            />
          </div>
          <div>
            <label for="pricePerLiter" class="block text-sm font-medium mb-1">Price / Liter</label>
            <input
              type="number"
              step="0.01"
              [(ngModel)]="newEntry.pricePerLiter"
              name="pricePerLiter"
              id="pricePerLiter"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 dark:bg-gray-700 rounded-md"
            />
          </div>
          <div class="flex items-center pt-6">
            <label class="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                [(ngModel)]="isPartialFillUp"
                name="isPartialFillUp"
                class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
              />
              <span class="text-sm font-medium">Partial fill up</span>
            </label>
          </div>
          <div class="md:col-span-5 flex justify-end gap-2">
            <button
              *ngIf="editingId"
              type="button"
              (click)="cancelEdit()"
              class="bg-gray-500 text-white px-4 py-2 rounded-md hover:bg-gray-600"
            >
              Cancel
            </button>
            <button
              type="submit"
              class="bg-green-600 text-white px-6 py-2 rounded-md hover:bg-green-700"
            >
              {{ editingId ? 'Update Entry' : 'Add Entry' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Entries Table -->
      <div
        class="overflow-x-auto bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700"
      >
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Date
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Odometer
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Liters
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Price/L
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Total
              </th>
              <th
                class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Status
              </th>
              <th
                class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider"
              >
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr *ngFor="let entry of entries">
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                {{ entry.date | date: 'yyyy-MM-dd' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">{{ entry.odometer | number }} km</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">{{ entry.liters }} L</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                {{ entry.pricePerLiter | currency: 'EUR' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                {{ entry.totalCost | currency: 'EUR' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm">
                <span
                  [class]="
                    entry.isFullTank
                      ? 'text-gray-600 dark:text-gray-400'
                      : 'text-orange-600 dark:text-orange-400 font-medium'
                  "
                >
                  {{ entry.isFullTank ? 'Full' : 'Partial' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
                <button
                  (click)="startEdit(entry)"
                  class="text-blue-600 dark:text-blue-400 hover:text-blue-900"
                >
                  Edit
                </button>
                <button
                  (click)="deleteEntry(entry.id)"
                  class="text-red-600 dark:text-red-400 hover:text-red-900"
                >
                  Delete
                </button>
              </td>
            </tr>
            <tr *ngIf="entries.length === 0">
              <td colspan="7" class="px-6 py-8 text-center text-gray-500">No fuel entries yet.</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination Controls -->
      <div *ngIf="totalPages > 0" class="flex items-center justify-between">
        <span class="text-sm text-gray-500 dark:text-gray-400">
          Page {{ currentPage }} of {{ totalPages }}
        </span>
        <div class="flex gap-2">
          <button
            type="button"
            (click)="goToPage(currentPage - 1)"
            [disabled]="currentPage <= 1"
            class="px-3 py-1.5 text-sm rounded-md border border-gray-300 dark:border-gray-600 disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-100 dark:hover:bg-gray-700"
          >
            Previous
          </button>
          <button
            type="button"
            (click)="goToPage(currentPage + 1)"
            [disabled]="currentPage >= totalPages"
            class="px-3 py-1.5 text-sm rounded-md border border-gray-300 dark:border-gray-600 disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-100 dark:hover:bg-gray-700"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  `,
})
export class CarDetailComponent implements OnInit {
  car: CarResponse | null = null;
  entries: FuelEntryResponse[] = [];
  editingId: string | null = null;
  currentPage = 1;
  readonly pageSize = 10;
  totalPages = 0;

  private carId: string | null = null;
  newEntry: FuelEntryRequest = this.resetEntryForm();

  get isPartialFillUp(): boolean {
    return !this.newEntry.isFullTank;
  }

  set isPartialFillUp(value: boolean) {
    this.newEntry.isFullTank = !value;
  }

  private route = inject(ActivatedRoute);
  private dataService = inject(DataService);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.carId = id;
      this.dataService.getCar(id).subscribe((car) => (this.car = car));
      this.loadEntries(1);
    }
  }

  loadEntries(page: number) {
    if (!this.carId) return;

    this.dataService.getFuelEntries(this.carId, page, this.pageSize).subscribe((response) => {
      this.entries = response.items;
      this.currentPage = response.page;
      this.totalPages = response.totalPages;

      if (this.entries.length === 0 && this.currentPage > 1) {
        this.loadEntries(this.currentPage - 1);
      }
    });
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.loadEntries(page);
  }

  saveEntry() {
    if (!this.car) return;

    const entryToSave: FuelEntryRequest = {
      ...this.newEntry,
    };

    if (this.editingId) {
      this.dataService.updateFuelEntry(this.editingId, entryToSave).subscribe(() => {
        this.loadEntries(this.currentPage);
        this.cancelEdit();
      });
    } else {
      this.dataService.addFuelEntry(this.car.id, entryToSave).subscribe(() => {
        this.newEntry = this.resetEntryForm();
        this.loadEntries(1);
      });
    }
  }

  startEdit(entry: FuelEntryResponse) {
    this.editingId = entry.id;
    // Format date for date input (YYYY-MM-DD)
    const localDate = new Date(entry.date).toISOString().slice(0, 10);

    this.newEntry = {
      date: localDate,
      odometer: entry.odometer,
      liters: entry.liters,
      pricePerLiter: entry.pricePerLiter,
      isFullTank: entry.isFullTank,
    };
  }

  cancelEdit() {
    this.editingId = null;
    this.newEntry = this.resetEntryForm();
  }

  deleteEntry(id: string) {
    if (confirm('Are you sure you want to delete this entry?')) {
      this.dataService.deleteFuelEntry(id).subscribe(() => this.loadEntries(this.currentPage));
    }
  }

  formatOdometer(value: number | null): string {
    if (value === null || value === undefined) return '';
    return value.toLocaleString('en-US');
  }

  onOdometerChange(value: string) {
    // Remove all non-digit characters
    const cleanValue = value.replace(/\D/g, '');
    this.newEntry.odometer = cleanValue === '' ? null : parseInt(cleanValue, 10);
  }

  private resetEntryForm(): FuelEntryRequest {
    const localDate = new Date().toISOString().slice(0, 10);
    return {
      date: localDate,
      odometer: null,
      liters: null,
      pricePerLiter: null,
      isFullTank: true,
    };
  }
}
