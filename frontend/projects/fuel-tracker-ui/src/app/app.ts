import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { WakingUpComponent } from './shared/components/waking-up.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, WakingUpComponent],
  template: `
    <app-waking-up *ngIf="authService.isVerifying()" />
    <router-outlet />
  `,
  styles: [],
})
export class App implements OnInit {
  title = 'fuel-tracker-ui';
  authService = inject(AuthService);

  ngOnInit(): void {
    this.authService.verifySession().subscribe();
  }
}
