import { Component, Output, EventEmitter } from '@angular/core';
import * as moment from 'moment';
import 'moment/locale/es';

@Component({
  selector: 'app-year-month-navigator',
  templateUrl: './year-month-navigator.component.html',
  styleUrls: ['./year-month-navigator.component.scss'],
})
export class YearMonthNavigatorComponent {
  // Mes y año actuales
  currentDate = moment();

  // Emite el mes y año seleccionados
  @Output() dateChanged = new EventEmitter<{ month: number; year: number }>();

  constructor() {
    // Emitimos la fecha actual al iniciar el componente
    this.emitDate();
  }

  // Método para retroceder un mes
  prevMonth() {
    this.currentDate = this.currentDate.subtract(1, 'month');
    this.emitDate();
  }

  // Método para avanzar un mes
  nextMonth() {
    this.currentDate = this.currentDate.add(1, 'month');
    this.emitDate();
  }

  // Método para emitir el mes y año actualizados
  emitDate() {
    const month = this.currentDate.month() + 1; // Los meses en moment.js empiezan desde 0
    const year = this.currentDate.year();
    this.dateChanged.emit({ month, year });
  }

  // Método para obtener el mes y año en formato "Enero 2024"
  getFormattedDate(): string {
    return this.currentDate.format('MMMM YYYY');
  }
}
