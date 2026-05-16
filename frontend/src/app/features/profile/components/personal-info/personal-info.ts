import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../../../core/services/user.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { User } from '../../../../core/models/user.model';

@Component({ selector: 'app-personal-info', standalone: true, imports: [CommonModule, ReactiveFormsModule], templateUrl: './personal-info.html', styleUrl: './personal-info.css' })
export class PersonalInfoComponent implements OnInit {
  private userSvc = inject(UserService);
  private notify = inject(NotificationService);
  private fb = inject(FormBuilder);
  user: User | null = null;
  loading = true;
  saving = false;
  form = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^01[0-2,5]\d{8}$/)]]
  });

  ngOnInit(): void {
    this.userSvc.getProfile().subscribe({ next: (u) => { this.user = u; this.form.patchValue({ fullName: u.fullName, phoneNumber: u.phoneNumber }); this.loading = false; }, error: () => { this.loading = false; } });
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.userSvc.updateProfile(this.form.value as any).subscribe({ next: (u) => { this.user = u; this.saving = false; this.notify.success('تم تحديث البيانات'); }, error: () => { this.saving = false; } });
  }
}
