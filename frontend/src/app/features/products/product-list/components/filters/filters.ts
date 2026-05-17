import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Category, ProductFilter } from '../../../../../core/models/product.model';

@Component({
  selector: 'app-filters',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './filters.html',
  styleUrl: './filters.css'
})
export class FiltersComponent implements OnInit {
  @Input() categories: Category[] = [];
  @Input() currentFilter: ProductFilter = {};
  @Output() filterChange = new EventEmitter<Partial<ProductFilter>>();

  minPrice: number | null = null;
  maxPrice: number | null = null;
  selectedCategory: number | null = null;
  selectedRating: number | null = null;

  ngOnInit(): void {
    this.minPrice = this.currentFilter.minPrice ?? null;
    this.maxPrice = this.currentFilter.maxPrice ?? null;
    this.selectedCategory = this.currentFilter.categoryId ?? null;
    this.selectedRating = this.currentFilter.rating ?? null;
  }

  apply(): void {
    this.filterChange.emit({
      categoryId: this.selectedCategory ?? undefined,
      minPrice: this.minPrice ?? undefined,
      maxPrice: this.maxPrice ?? undefined,
      rating: this.selectedRating ?? undefined
    });
  }

  reset(): void {
    this.minPrice = null;
    this.maxPrice = null;
    this.selectedCategory = null;
    this.selectedRating = null;
    this.filterChange.emit({});
  }
}
