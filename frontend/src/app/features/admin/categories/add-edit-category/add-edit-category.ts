import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
@Component({ selector: 'app-add-edit-category', standalone: true, imports: [RouterLink], template: `<p>استخدم صفحة <a routerLink="/admin/categories">قائمة الفئات</a> للإضافة والتعديل</p>` })
export class AddEditCategoryComponent {}
