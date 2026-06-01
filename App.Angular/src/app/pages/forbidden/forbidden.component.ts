import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-forbidden',
  imports: [RouterLink, ButtonModule],
  templateUrl: './forbidden.component.html',
  styleUrl: './forbidden.component.scss',
})
export class Forbidden {}
