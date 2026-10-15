import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  template: `
    <div
      class="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 px-4 transition-colors duration-200"
    >
      <div class="max-w-md w-full space-y-8 bg-white dark:bg-gray-800 p-8 rounded-xl shadow-md">
        <div>
          <h2 class="mt-6 text-center text-3xl font-extrabold text-gray-900 dark:text-white">
            Create an account
          </h2>
        </div>
        <form class="mt-8 space-y-6" (submit)="onSubmit()">
          <div class="rounded-md shadow-sm -space-y-px">
            <div>
              <label for="username" class="sr-only">Username</label>
              <input
                id="username"
                name="username"
                type="text"
                required
                [(ngModel)]="username"
                class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-t-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm dark:bg-gray-700"
                placeholder="Username"
              />
            </div>
            <div>
              <label for="email" class="sr-only">Email</label>
              <input
                id="email"
                name="email"
                type="email"
                required
                [(ngModel)]="email"
                class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm dark:bg-gray-700"
                placeholder="Email"
              />
            </div>
            <div>
              <label for="password" class="sr-only">Password</label>
              <input
                id="password"
                name="password"
                type="password"
                required
                [(ngModel)]="password"
                class="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 placeholder-gray-500 dark:placeholder-gray-400 text-gray-900 dark:text-white rounded-b-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm dark:bg-gray-700"
                placeholder="Password"
              />
            </div>
          </div>

          <div *ngIf="error" class="text-red-500 text-sm text-center">
            {{ error }}
          </div>

          <div>
            <button
              type="submit"
              [disabled]="loading"
              class="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50"
            >
              Register
            </button>
          </div>

          <div class="text-center">
            <a
              routerLink="/login"
              class="text-sm font-medium text-blue-600 dark:text-blue-400 hover:text-blue-500"
            >
              Already have an account? Sign in
            </a>
          </div>
        </form>
      </div>
    </div>
  `,
})
export class RegisterComponent {
  username = '';
  email = '';
  password = '';
  loading = false;
  error = '';

  private authService = inject(AuthService);
  private router = inject(Router);

  onSubmit() {
    this.loading = true;
    this.error = '';
    this.authService
      .register({ username: this.username, email: this.email, password: this.password })
      .subscribe({
        next: () => this.router.navigate(['/login']),
        error: () => {
          this.error = 'Registration failed. Try a different username.';
          this.loading = false;
        },
      });
  }
}
