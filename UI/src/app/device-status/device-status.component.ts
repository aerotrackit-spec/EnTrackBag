import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ApiService } from '../core/api.service';

@Component({
  standalone: true,
  selector: 'app-device-status',
  imports: [CommonModule],
  templateUrl: './device-status.component.html',
  styleUrl: './device-status.component.scss'
})
export class DeviceStatusComponent implements OnInit {
  private readonly api = inject(ApiService);
  summary: any;
  details: any[] = [];
  selectedCategory = '';

  ngOnInit(): void { this.load(); }

  load(category = ''): void {
    this.selectedCategory = category;
    this.api.get<any>('device-status/summary').subscribe({ next: value => this.summary = value });
    const query = category ? `?category=${encodeURIComponent(category)}` : '';
    this.api.get<any[]>(`device-status/details${query}`).subscribe({ next: value => this.details = value ?? [] });
  }
}
