import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    templateUrl: './error.component.html',
    styleUrls: ['./error.component.scss'],
    standalone: true,
})
export class ErrorComponent implements OnInit {
  code = '';
  message = '';
  constructor(private _ar: ActivatedRoute) {}

  ngOnInit(): void {
    this.code = this._ar.snapshot.params.code;
    this.message = this._ar.snapshot.queryParams.message;
  }
}
