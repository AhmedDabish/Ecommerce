import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../../core/services/admin.service';
import { CurrencyFormatPipe } from '../../../shared/pipes/currency-format.pipe';

@Component({ selector: 'app-admin-dashboard', standalone: true, imports: [CommonModule, RouterLink, CurrencyFormatPipe], templateUrl: './dashboard.html', styleUrl: './dashboard.css' })
export class AdminDashboardComponent implements OnInit {
  private adminSvc = inject(AdminService);
  stats: any = null;
  loading = true;
  ngOnInit(): void { this.adminSvc.getDashboard().subscribe({ next: (s) => { this.stats = s; this.loading = false; }, error: () => { this.loading = false; } }); }
}
