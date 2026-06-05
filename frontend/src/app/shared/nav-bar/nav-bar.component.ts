import { Component, inject, OnInit } from '@angular/core';
import { Menubar } from 'primeng/menubar';
import { Button } from 'primeng/button';
import { Menu } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-nav-bar',
  imports: [Menubar, Button, Menu],
  templateUrl: './nav-bar.component.html',
  styleUrl: './nav-bar.component.scss',
})
export class NavBarComponent implements OnInit {
  protected readonly authService = inject(AuthService);

  menubarItems: MenuItem[] = [
    {
      label: 'Equipment',
      icon: 'pi pi-warehouse',
      routerLink: '/equipment',
    },
    {
      label: 'Reservations',
      icon: 'pi pi-address-book',
      routerLink: '/reservation',
    },
  ];

  userMenuItems: MenuItem[] = [
    {
      label: 'Logout',
      icon: 'pi pi-sign-out',
      command: () => this.authService.logout().subscribe(),
    },
  ];

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (user?.isAdmin()) {
      this.menubarItems.push({
        label: 'Admin',
        icon: 'pi pi-cog',
        items: [
          {
            label: 'Locations',
            icon: 'pi pi-map-marker',
            routerLink: '/locations',
          },
          {
            label: 'Equipment Types',
            icon: 'pi pi-map-marker',
            routerLink: '/equipment-types',
          },
        ],
      });
    }
  }
}
