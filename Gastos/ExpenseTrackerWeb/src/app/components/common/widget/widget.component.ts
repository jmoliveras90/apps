import { Component, Input, OnInit } from '@angular/core';
import { ChartConfiguration, ChartType } from 'chart.js';

@Component({
  selector: 'app-widget',
  templateUrl: './widget.component.html',
  styleUrls: ['./widget.component.scss'],
})
export class WidgetComponent implements OnInit {
  @Input() chartData!: ChartConfiguration['data']; // Datos para el gráfico
  @Input() chartType: ChartType = 'pie'; // Tipo de gráfico (pie, bar, line, etc.)
  @Input() chartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
  }; // Opciones del gráfico
  @Input() title: string = ''; // Título del widget

  constructor() {}

  ngOnInit(): void {}
}
