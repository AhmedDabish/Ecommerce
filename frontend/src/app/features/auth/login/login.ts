import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  loading = false;
  showPassword = false;
  errorMessage = '';

  get f() { return this.form.controls; }

  submit(): void {
    this.errorMessage = '';
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;

    this.auth.login(this.form.value).subscribe({
      next: (res) => {
        this.loading = false;
        const returnUrl = this.route.snapshot.queryParams['returnUrl'];
        const role = res.user?.role?.name;
        if (returnUrl) {
          this.router.navigateByUrl(returnUrl);
        } else if (role === 'Admin') {
          this.router.navigate(['/admin']);
        } else if (role === 'Seller') {
          this.router.navigate(['/seller']);
        } else {
          this.router.navigate(['/home']);
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || 'Invalid email or password. Please try again.';
      }
    });
  }
}
