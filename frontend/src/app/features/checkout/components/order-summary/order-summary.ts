import { Component, Input } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterLink } from "@angular/router";
import { Cart } from "../../../../core/models/cart.model";
import { CurrencyFormatPipe } from "../../../../shared/pipes/currency-format.pipe";

@Component({
  selector: "app-order-summary",
  standalone: true,
  imports: [CommonModule, RouterLink, CurrencyFormatPipe],
  templateUrl: "./order-summary.html",
  styleUrl: "./order-summary.css",
})
export class OrderSummaryComponent {
  @Input() cart!: Cart;
  @Input() promoDiscount = 0;

  get items() {
    return this.cart?.cartItems ?? [];
  }

  get subtotal(): number {
    return this.items.reduce((s, i) => {
      const p = i.product?.discountPrice ?? i.product?.price ?? i.priceAtAdd;
      return s + p * i.quantity;
    }, 0);
  }

  get shipping(): number {
    return this.subtotal >= 500 ? 0 : 50;
  }
  get total(): number {
    return this.subtotal + this.shipping - this.promoDiscount;
  }

  getItemImg(item: any): string {
    return item.product?.images?.[0]?.imageUrl ?? "images/placeholder.png";
  }
}
