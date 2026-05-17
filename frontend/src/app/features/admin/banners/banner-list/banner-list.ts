import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../../core/services/admin.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { Banner } from '../../../../core/models/order.model';

@Component({ selector: 'app-banner-list', standalone: true, imports: [CommonModule, FormsModule], templateUrl: './banner-list.html', styleUrl: './banner-list.css' })
export class BannerListComponent implements OnInit {
  private adminSvc = inject(AdminService);
  private notify = inject(NotificationService);
  banners: Banner[] = [];
  loading = true;
  showForm = false;
  saving = false;
  f = { title:'', description:'', imageUrl:'', linkUrl:'', displayOrder:0, isActive:true, startDate:'', endDate:'' };

  ngOnInit(): void { this.load(); }
  load(): void { this.adminSvc.getBanners().subscribe({ next: b => { this.banners = b; this.loading = false; }, error: () => { this.loading = false; } }); }

  save(): void {
    this.saving = true;
    this.adminSvc.createBanner({ ...this.f, startDate: new Date(this.f.startDate), endDate: new Date(this.f.endDate) }).subscribe({ next: () => { this.showForm = false; this.saving = false; this.notify.success('تم إضافة البانر'); this.load(); }, error: () => { this.saving = false; } });
  }

  delete(id: number): void {
    if (!confirm('حذف البانر؟')) return;
    this.adminSvc.deleteBanner(id).subscribe({ next: () => { this.banners = this.banners.filter(b => b.id !== id); this.notify.success('تم الحذف'); } });
  }
}
