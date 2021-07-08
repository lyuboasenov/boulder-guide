import { Component } from '@angular/core';

interface Point {
   x: number;
   y: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent {
  title = 'boulder-guide';
}
