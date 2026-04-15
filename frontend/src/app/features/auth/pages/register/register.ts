import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
 
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
 
  private fb = inject(FormBuilder);
  private http = inject(HttpClient);
  private router = inject(Router);
 
  private api = 'http://localhost:5009/auth'; 
 
  loading = false;
  errorMessage = '';
 
  form = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(3)]]
  });
 
  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
 
    this.loading = true;
    this.errorMessage = '';
 
    this.http.post(`${this.api}/register`, this.form.getRawValue()).subscribe({
      next: () => {
        // depois de cadastrar volta pro login
        this.router.navigate(['/login']);
      },
      error: () => {
        this.errorMessage = 'Erro ao cadastrar usuário';
        this.loading = false;
      }
    });
  }
 
  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
 