import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'amountToTez' })
export class AmountToTezPipe implements PipeTransform {
    transform(value: number): number {
        return this.round(value / 1000000, 3);
    }

    private round(value, precision) {
        const multiplier = Math.pow(10, precision || 0);
        return Math.round(value * multiplier) / multiplier;
    }
}