import { Role } from './common.enums';

// LOCATION

import { FormControl } from '@angular/forms';

export interface Location {
  id: number;
  name: string;
  archived: boolean;
  isEditing?: boolean;
  editControl?: FormControl<string | null>;
}

// USER

export class User {
  readonly id: number;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: string;
  readonly roleId: number;
  readonly roleName: string;

  constructor(data: User) {
    this.id = data.id;
    this.firstName = data.firstName;
    this.lastName = data.lastName;
    this.email = data.email;
    this.roleId = data.roleId;
    this.roleName = data.roleName;
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }

  isAdmin(): boolean {
    return this.roleId === Role.Admin;
  }
}

// EQUIPMENT STATUS

export interface EquipmentStatus {
  id: number;
  name: string;
}

// EQUIPMENT TYPE

export interface EquipmentType {
  id: number;
  name: string;
  archived: boolean;
  isEditing?: boolean;
  editControl?: FormControl<string | null>;
}

// EQUIPMENT
export interface Equipment {
  id?: number;
  name: string;
  serialNumber: string;
  specification: string;
  maxLoanDays: number;
  statusId: number;
  typeId: number;
  locationId: number;
  location?: Location;
  status?: EquipmentStatus;
  type?: EquipmentType;
}

// FILTER ITEM
export interface FilterItem {
  key: string;
  value: string | null;
}
