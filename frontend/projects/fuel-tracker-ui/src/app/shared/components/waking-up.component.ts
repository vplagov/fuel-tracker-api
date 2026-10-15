import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-waking-up',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      class="fixed inset-0 flex items-center justify-center bg-gray-50 dark:bg-gray-900 z-50 transition-colors duration-200"
    >
      <div class="text-center">
        <div
          class="inline-block animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-600 mb-4"
        ></div>
        <h2 class="text-xl font-semibold text-gray-900 dark:text-white">{{ title }}</h2>
        <p class="text-gray-500 dark:text-gray-400 mt-2">{{ message }}</p>
      </div>
    </div>
  `,
  styles: [],
})
export class WakingUpComponent {
  @Input() title = 'Waking up the backend';
  @Input() message = 'This may take a few seconds as the server starts up...';
}
