import { Injectable, inject } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable, map } from "rxjs";
import {
  Product,
  ProductFilter,
  PaginatedResult,
  Category,
  Review,
  CreateProductDto,
  CreateReviewDto,
} from "../models/product.model";
import { environment } from "../../../environments/environment";

@Injectable({ providedIn: "root" })
export class ProductService {
  private readonly API = `${environment.apiUrl}/api`;
  private http = inject(HttpClient);

  // GET /api/Products — backend returns array, we wrap into PaginatedResult
  getProducts(filter?: ProductFilter): Observable<PaginatedResult<Product>> {
    return this.http.get<any[]>(`${this.API}/Products`).pipe(
      map((data) => {
        const arr = Array.isArray(data) ? data : [];
        const products: Product[] = arr.map((p) => this.mapProduct(p));

        // Client-side filter by category
        const filtered = filter?.categoryId
          ? products.filter((p) => p.categoryId === filter.categoryId)
          : products;

        const page = filter?.page ?? 1;
        const limit = filter?.limit ?? 12;
        const totalItems = filtered.length;
        const totalPages = Math.ceil(totalItems / limit) || 1;
        const start = (page - 1) * limit;
        const items = filtered.slice(start, start + limit);

        return { items, totalItems, totalPages, currentPage: page };
      }),
    );
  }

  // GET /api/Products/{id}
  getProduct(id: number): Observable<Product> {
    return this.http
      .get<any>(`${this.API}/Products/${id}`)
      .pipe(map((p) => this.mapProduct(p)));
  }

  getByCategory(categoryId: number): Observable<Product[]> {
    return this.http
      .get<any[]>(`${this.API}/Products`)
      .pipe(
        map((arr) =>
          arr
            .filter((p) => p.categoryId === categoryId)
            .map((p) => this.mapProduct(p)),
        ),
      );
  }

  getFeatured(): Observable<Product[]> {
    return this.http
      .get<any[]>(`${this.API}/Products`)
      .pipe(map((arr) => arr.slice(0, 8).map((p) => this.mapProduct(p))));
  }

  search(
    q: string,
    filter?: ProductFilter,
  ): Observable<PaginatedResult<Product>> {
    return this.http.get<any[]>(`${this.API}/Products`).pipe(
      map((data) => {
        const arr = Array.isArray(data) ? data : [];
        const all = arr.map((p) => this.mapProduct(p));
        const filtered = all.filter(
          (p) =>
            p.name.toLowerCase().includes(q.toLowerCase()) ||
            p.description?.toLowerCase().includes(q.toLowerCase()),
        );
        const page = filter?.page ?? 1;
        const limit = filter?.limit ?? 12;
        const totalItems = filtered.length;
        const totalPages = Math.ceil(totalItems / limit) || 1;
        const start = (page - 1) * limit;
        return {
          items: filtered.slice(start, start + limit),
          totalItems,
          totalPages,
          currentPage: page,
        };
      }),
    );
  }

  // Map backend DTO → frontend Product model
  private mapProduct(p: any): Product {
    return {
      id: p.id,
      name: p.name,
      description: p.description ?? "",
      price: p.price,
      discountPrice: p.discountPrice,
      stockQuantity: p.stockQuantity ?? 0,
      sku: p.sku ?? p.sKU ?? "",
      categoryId: p.categoryId,
      sellerId: p.sellerId,
      createdAt: p.createdAt,
      updatedAt: p.updatedAt,
      isActive: p.isActive ?? true,
      viewCount: p.viewCount ?? 0,
      averageRating: p.averageRating ?? 0,
      category: p.categoryName
        ? {
            id: p.categoryId,
            name: p.categoryName,
            description: "",
            isActive: true,
          }
        : p.category,
      // Handle both: images[] of objects  OR  imageUrls[] of strings
      images: p.images?.length
        ? p.images
        : (p.imageUrls ?? []).map((url: string, i: number) => ({
            id: i,
            productId: p.id,
            imageUrl: url,
            isPrimary: i === 0,
            displayOrder: i,
          })),
    };
  }

  // POST /api/Products
  createProduct(data: FormData): Observable<Product> {
    return this.http.post<Product>(`${this.API}/Products`, data);
  }

  // PUT /api/Products/{id}
  updateProduct(id: number, data: any): Observable<Product> {
    return this.http.put<Product>(`${this.API}/Products/${id}`, data);
  }

  // DELETE /api/Products/{id}
  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.API}/Products/${id}`);
  }

  uploadImages(productId: number, images: FormData): Observable<any> {
    return this.http.post(`${this.API}/Products/${productId}/images`, images);
  }

  deleteImage(imageId: number): Observable<any> {
    return this.http.delete(`${this.API}/Products/images/${imageId}`);
  }

  // GET /api/Reviews/product/{productId}
  getReviews(productId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.API}/Reviews/product/${productId}`);
  }

  // POST /api/Reviews
  addReview(review: CreateReviewDto): Observable<Review> {
    return this.http.post<Review>(`${this.API}/Reviews`, review);
  }

  // DELETE /api/Reviews/{id}
  deleteReview(id: number): Observable<any> {
    return this.http.delete(`${this.API}/Reviews/${id}`);
  }

  // GET /api/Categories
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.API}/Categories`);
  }

  getCategory(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.API}/Categories/${id}`);
  }

  createCategory(data: Partial<Category>): Observable<Category> {
    return this.http.post<Category>(`${this.API}/Categories`, data);
  }

  updateCategory(id: number, data: Partial<Category>): Observable<Category> {
    return this.http.put<Category>(`${this.API}/Categories/${id}`, data);
  }

  deleteCategory(id: number): Observable<any> {
    return this.http.delete(`${this.API}/Categories/${id}`);
  }
}
