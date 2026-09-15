import { Component, OnInit, inject } from '@angular/core';
import { ApiService } from '../../core/api.service';

@Component({
  standalone: true,
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrl: './summary.component.scss'
})
export class SummaryComponent implements OnInit {
  private readonly api = inject(ApiService);
  kpi: any;

  ngOnInit(): void {
    this.api.get<any>('dashboard/kpis').subscribe({ next: value => this.kpi = value });
  }
}
