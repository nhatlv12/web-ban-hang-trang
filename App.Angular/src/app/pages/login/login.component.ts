import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { CheckboxModule } from 'primeng/checkbox';
import { MessageModule } from 'primeng/message';
import { FloatLabelModule } from 'primeng/floatlabel';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    ButtonModule,
    InputTextModule,
    PasswordModule,
    CheckboxModule,
    MessageModule,
    FloatLabelModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class Login {
  username = '';
  password = '';
  rememberMe = false;
  loading = signal(false);
  errorMessage = signal('');

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin(): void {
    this.errorMessage.set('');

    if (!this.username || !this.password) {
      this.errorMessage.set('Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.');
      return;
    }

    this.loading.set(true);

    // Giả lập delay gọi API
    setTimeout(() => {
      const success = this.authService.login(this.username, this.password);
      if (success) {
        this.router.navigate(['/']);
      } else {
        this.errorMessage.set('Tên đăng nhập hoặc mật khẩu không đúng.');
      }
      this.loading.set(false);
    }, 800);
  }
}
