import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { Router } from "@angular/router";
import { CartService } from "../../core/services/cart.service";
import { OrderService } from "../../core/services/order.service";
import { PaymentService } from "../../core/services/payment.service";
import { PaymentMethod } from "../../core/models/order.model";
import { NotificationService } from "../../core/services/notification.service";
import { ShippingAddressComponent } from "./components/shipping-address/shipping-address";
import { PaymentMethodComponent } from "./components/payment-method/payment-method";
import { OrderSummaryComponent } from "./components/order-summary/order-summary";
import { PromoCodeComponent } from "./components/promo-code/promo-code";

@Component({
  selector: "app-checkout",
  standalone: true,
  imports: [
    CommonModule,
    ShippingAddressComponent,
    PaymentMethodComponent,
    OrderSummaryComponent,
    PromoCodeComponent,
  ],
  templateUrl: "./checkout.html",
  styleUrl: "./checkout.css",
})
export class CheckoutComponent implements OnInit {
  cartSvc = inject(CartService);
  private orderSvc = inject(OrderService);
  private paymentSvc = inject(PaymentService);
  private notify = inject(NotificationService);
  private router = inject(Router);

  step = 1;
  steps = [
    "Shipping Address",
    "Payment Method",
    "Review Order",
    "Confirmation",
  ];

  selectedAddressId: number | null = null;
  selectedPaymentMethod: PaymentMethod | null = null;
  promoDiscount = 0;
  placingOrder = false;
  orderNumber = "";

  ngOnInit(): void {
    this.cartSvc.loadCart().subscribe();
  }

  onAddressSelected(id: number): void {
    this.selectedAddressId = id;
    this.step = 2;
  }
  onPaymentSelected(method: PaymentMethod): void {
    this.selectedPaymentMethod = method;
    this.step = 3;
  }
  onPromoApplied(discount: number): void {
    this.promoDiscount = discount;
  }

  placeOrder(): void {
    if (!this.selectedAddressId || !this.selectedPaymentMethod) return;
    this.placingOrder = true;
    this.orderSvc
      .createOrder({
        shippingAddressId: this.selectedAddressId,
        paymentMethod: this.selectedPaymentMethod,
      })
      .subscribe({
        next: (order) => {
          this.orderNumber = order.orderNumber;
          this.cartSvc.clear().subscribe();
          this.step = 4;
          this.placingOrder = false;
        },
        error: () => {
          this.placingOrder = false;
        },
      });
  }
}
