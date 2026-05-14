import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';
import { Order, OrderStatus, Banner, PromoCode } from '../models/order.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AdminService {
  // Real backend:
  // GET /api/Admin/users
  // PUT /api/Admin/users/{id}/approve
  // PUT /api/Admin/users/{id}/restrict
  // GET /api/Admin/orders
  // PUT /api/Admin/orders/{id}/status   body: { status }
  // GET /api/Admin/dashboard
  // GET/POST/PUT/DELETE /api/PromoCode
  // Banners - not in real backend, use mock
  private readonly BASE = `${environment.apiUrl}/api`;
  private http = inject(HttpClient);

  // GET /api/Admin/dashboard
  getDashboard(): Observable<any> {
    return this.http.get(`${this.BASE}/Admin/dashboard`);
  }

  // GET /api/Admin/users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.BASE}/Admin/users`);
  }

  // PUT /api/Admin/users/{id}/approve
  approveUser(id: number): Observable<any> {
    return this.http.put(`${this.BASE}/Admin/users/${id}/approve`, {});
  }

  // PUT /api/Admin/users/{id}/restrict
  restrictUser(id: number): Observable<any> {
    return this.http.put(`${this.BASE}/Admin/users/${id}/restrict`, {});
  }

  // GET /api/Admin/orders
  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.BASE}/Admin/orders`);
  }

  // PUT /api/Admin/orders/{id}/status  body: UpdateOrderStatusDto { status }
  updateOrderStatus(id: number, status: OrderStatus): Observable<any> {
    return this.http.put(`${this.BASE}/Admin/orders/${id}/status`, { status });
  }

  // Banners - not in real swagger, use products endpoint or mock
  getBanners(): Observable<Banner[]> {
    // Return empty array since backend has no banner endpoint in real API
    return new Observable(obs => { obs.next([]); obs.complete(); });
  }

  createBanner(data: Partial<Banner>): Observable<Banner> {
    return new Observable(obs => { obs.next(data as Banner); obs.complete(); });
  }

  updateBanner(id: number, data: Partial<Banner>): Observable<Banner> {
    return new Observable(obs => { obs.next(data as Banner); obs.complete(); });
  }

  deleteBanner(id: number): Observable<any> {
    return new Observable(obs => { obs.next({}); obs.complete(); });
  }

  // GET /api/PromoCode
  getPromoCodes(): Observable<PromoCode[]> {
    return this.http.get<PromoCode[]>(`${this.BASE}/PromoCode`);
  }

  // POST /api/PromoCode
  createPromoCode(data: Partial<PromoCode>): Observable<PromoCode> {
    return this.http.post<PromoCode>(`${this.BASE}/PromoCode`, data);
  }

  // PUT /api/PromoCode/{id}
  updatePromoCode(id: number, data: Partial<PromoCode>): Observable<PromoCode> {
    return this.http.put<PromoCode>(`${this.BASE}/PromoCode/${id}`, data);
  }

  // DELETE /api/PromoCode/{id}
  deletePromoCode(id: number): Observable<any> {
    return this.http.delete(`${this.BASE}/PromoCode/${id}`);
  }
}
