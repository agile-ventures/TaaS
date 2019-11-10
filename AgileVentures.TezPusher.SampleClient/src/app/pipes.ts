import { Pipe, PipeTransform } from '@angular/core';

export class TezUtils {
    public static round(value, precision): number {
        const multiplier = Math.pow(10, precision || 0);
        return Math.round(value * multiplier) / multiplier;
    }
}

@Pipe({ name: 'amountToTez' })
export class AmountToTezPipe implements PipeTransform {
    transform(value: number): number {
        return TezUtils.round(value / 1000000, 3);
    }
}

@Pipe({ name: 'feeToTez' })
export class FeeToTezPipe implements PipeTransform {
    transform(value: number): number {
        return TezUtils.round(value / 1000000, 6);
    }
}

