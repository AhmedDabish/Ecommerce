import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../../core/services/admin.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PromoCode } from '../../../../core/models/order.model';
import { DateFormatPipe } from '../../../../shared/pipes/date-format.pipe';
import { CurrencyFormatPipe } from '../../../../shared/pipes/currency-format.pipe';

@Component({ selector: 'app-promo-list', standalone: true, imports: [CommonModule, FormsModule, DateFormatPipe, CurrencyFormatPipe], templateUrl: './promo-list.html', styleUrl: './promo-list.css' })
export class PromoListComponent implements OnInit {
  private adminSvc = inject(AdminService);
  private notify = inject(NotificationService);
  promos: PromoCode[] = [];
  loading = true;
  showForm = false;
  saving = false;
  editItem: PromoCode | null = null;

  f = { code: '', description: '', discountType: 'Percentage' as 'Percentage'|'FixedAmount', discountValue: 0, minimumOrderAmount: 0, maxUsageCount: 0, startDate: '', endDate: '', isActive: true };

  ngOnInit(): void { this.load(); }
  load(): void { this.adminSvc.getPromoCodes().subscribe({ next: p => { this.promos = p; this.loading = false; }, error: () => { this.loading = false; } }); }

  openAdd(): void { this.editItem = null; this.f = { code:'', description:'', discountType:'Percentage', discountValue:0, minimumOrderAmount:0, maxUsageCount:0, startDate:'', endDate:'', isActive:true }; this.showForm = true; }

  save(): void {
    this.saving = true;
    const req = this.editItem ? this.adminSvc.updatePromoCode(this.editItem.id, this.f as any) : this.adminSvc.createPromoCode(this.f as any);
    req.subscribe({ next: () => { this.showForm = false; this.saving = false; this.notify.success('تم الحفظ'); this.load(); }, error: () => { this.saving = false; } });
  }

  delete(id: number): void {
    if (!confirm('حذف الكوبون؟')) return;
    this.adminSvc.deletePromoCode(id).subscribe({ next: () => { this.promos = this.promos.filter(p => p.id !== id); this.notify.success('تم الحذف'); } });
  }
}
