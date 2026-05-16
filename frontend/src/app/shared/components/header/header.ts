import { Component, inject, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';
import { WishlistService } from '../../../core/services/wishlist.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, FormsModule],
  templateUrl: './header.html',
  styleUrl: './header.css'
})
export class HeaderComponent {
  auth = inject(AuthService);
  cartSvc = inject(CartService);
  wishlistSvc = inject(WishlistService);
  private router = inject(Router);

  searchQuery = '';
  userMenuOpen = false;
  mobileMenuOpen = false;
  scrolled = false;

  @HostListener('window:scroll')
  onScroll(): void {
    this.scrolled = window.scrollY > 20;
  }

  search(): void {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/search'], { queryParams: { q: this.searchQuery.trim() } });
      this.searchQuery = '';
      this.mobileMenuOpen = false;
    }
  }

  onSearchKey(e: KeyboardEvent): void {
    if (e.key === 'Enter') this.search();
  }

  logout(): void {
    this.userMenuOpen = false;
    this.mobileMenuOpen = false;
    this.auth.logout();
  }

  get cartCount(): number { return this.cartSvc.cartCount; }
  get wishlistCount(): number { return this.wishlistSvc.count; }
  get userInitial(): string {
    return this.auth.currentUser?.fullName?.charAt(0)?.toUpperCase() ?? 'U';
  }
}
