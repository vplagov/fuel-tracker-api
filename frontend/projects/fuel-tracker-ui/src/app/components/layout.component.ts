import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { ButtonComponent } from '../shared/components/button.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule, ButtonComponent],
  template: `
    <div
      class="min-h-screen bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100 transition-colors duration-200"
    >
      <nav class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex justify-between h-16">
            <div class="flex">
              <div class="flex-shrink-0 flex items-center">
                <span class="text-xl font-bold text-blue-600 dark:text-blue-400">FuelTracker</span>
              </div>
              <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
                <a
                  routerLink="/cars"
                  routerLinkActive="border-blue-500 text-gray-900 dark:text-white"
                  class="border-transparent text-gray-500 dark:text-gray-400 hover:border-gray-300 dark:hover:border-gray-600 hover:text-gray-700 dark:hover:text-gray-200 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
                >
                  Cars
                </a>
                <a
                  routerLink="/statistics"
                  routerLinkActive="border-blue-500 text-gray-900 dark:text-white"
                  class="border-transparent text-gray-500 dark:text-gray-400 hover:border-gray-300 dark:hover:border-gray-600 hover:text-gray-700 dark:hover:text-gray-200 inline-flex items-center px-1 pt-1 border-b-2 text-sm font-medium"
                >
                  Statistics
                </a>
              </div>
            </div>
            <div class="flex items-center">
              <span class="mr-4 text-sm hidden sm:block">{{
                authService.currentUser()?.username
              }}</span>
              <app-button (clicked)="authService.logout()" variant="secondary"> Logout </app-button>
            </div>
          </div>
        </div>
      </nav>

      <main class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
})
export class LayoutComponent {
  public authService = inject(AuthService);
}
