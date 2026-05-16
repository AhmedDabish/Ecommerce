import {
  Component,
  Input,
  AfterViewInit,
  ElementRef,
  QueryList,
  ViewChildren,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterLink } from "@angular/router";
import { Category } from "../../../../core/models/product.model";

@Component({
  selector: "app-categories-section",
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: "./categories-section.html",
  styleUrl: "./categories-section.css",
})
export class CategoriesSectionComponent implements AfterViewInit {
  @Input() categories: Category[] = [];
  @ViewChildren("catCard") cards!: QueryList<ElementRef>;

  ngAfterViewInit(): void {
    const obs = new IntersectionObserver(
      (entries) => {
        entries.forEach((e) => {
          if (e.isIntersecting) {
            (e.target as HTMLElement).classList.add("visible");
            obs.unobserve(e.target);
          }
        });
      },
      { threshold: 0.1 },
    );

    setTimeout(() => {
      this.cards?.forEach((c) => obs.observe(c.nativeElement));
    }, 100);
  }

  getCatIcon(name: string): string {
    const n = name.toLowerCase();
    if (
      n.includes("electron") ||
      n.includes("الكترون") ||
      n.includes("إلكترون")
    )
      return "📱";
    if (n.includes("cloth") || n.includes("fashion") || n.includes("ملابس"))
      return "👗";
    if (n.includes("home") || n.includes("منزل") || n.includes("أجهزة"))
      return "🏠";
    if (n.includes("book") || n.includes("كتب")) return "📚";
    if (n.includes("sport") || n.includes("رياضة")) return "⚽";
    if (n.includes("food") || n.includes("طعام")) return "🍔";
    if (n.includes("beauty") || n.includes("جمال")) return "💄";
    if (n.includes("toy") || n.includes("ألعاب")) return "🧸";
    return "🏷️";
  }
}
