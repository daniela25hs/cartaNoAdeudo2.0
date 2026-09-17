import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [FormsModule]
})
export class SearchComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
