import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
@Component({ selector: 'app-add-edit-banner', standalone: true, imports: [RouterLink], template: `<a routerLink="/admin/banners">← رجوع لقائمة البانرات</a>` })
export class AddEditBannerComponent {}
