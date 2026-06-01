import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-home',
  imports: [ButtonModule, CardModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home {
  constructor(protected authService: AuthService) {}
}
