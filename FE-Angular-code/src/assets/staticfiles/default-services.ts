import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class DefaultServices {
    readonly crypto =
        {
            id: '7FF6451C-7D2E-4568-B6D2-D84E27E18319',
            name: 'Crypto',
        };
    readonly shophouse =
        {
            id: 'B11CE3B0-3074-421C-A601-B7BF9252C78C',
            name: 'Shop House',
        };
}