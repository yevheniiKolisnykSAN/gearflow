import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-equipment-list',
  imports: [],
  templateUrl: './equipment-list.component.html',
  styleUrl: './equipment-list.component.scss',
})
export class EquipmentListComponent {
  authService = inject(AuthService)
  logout() {
    this.authService.logout().subscribe(res => {

    });
  }
}
