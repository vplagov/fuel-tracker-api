import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      [type]="type"
      [disabled]="disabled"
      (click)="clicked.emit($event)"
      [class]="
        'px-4 py-2 rounded-md transition-colors ' +
        (variant === 'primary'
          ? 'bg-blue-600 text-white hover:bg-blue-700'
          : 'bg-gray-200 text-gray-800 hover:bg-gray-300') +
        (disabled ? ' opacity-50 cursor-not-allowed' : '')
      "
    >
      <ng-content></ng-content>
    </button>
  `,
  styles: [],
})
export class ButtonComponent {
  @Input() type: 'button' | 'submit' = 'button';
  @Input() variant: 'primary' | 'secondary' = 'primary';
  @Input() disabled = false;
  @Output() clicked = new EventEmitter<MouseEvent>();
}
