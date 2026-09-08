import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.scss'],
    imports: [FormsModule]
})
export class SearchComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
