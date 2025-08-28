import { RouterOutlet } from '@angular/router';
import { Component, signal, inject } from '@angular/core';
import {TranslatePipe, TranslateDirective, TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  imports: [ RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent {
  title = 'uers-management-ui';
  private translate = inject(TranslateService);

  useLanguage(language: string): void {
      this.translate.use(language);
  }
}
