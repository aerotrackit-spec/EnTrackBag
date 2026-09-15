import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, tap } from "rxjs";
import { environment } from "../../environments/environment";

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
  userName: string;
  displayName: string;
  roles: string[];
  permissions: { code: string; accessType: string }[];
  sessionId: string;
}

@Injectable({ providedIn: "root" })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.identityApiUrl;

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.baseUrl}/auth/login`, request)
      .pipe(
        tap((response) => {
          localStorage.setItem("access_token", response.accessToken);
          localStorage.setItem(
            "permissions",
            JSON.stringify(response.permissions ?? []),
          );
          localStorage.setItem("user_name", response.userName);
          localStorage.setItem("display_name", response.displayName);
        }),
      );
  }

  logout(): void {
    localStorage.removeItem("access_token");
    localStorage.removeItem("permissions");
    localStorage.removeItem("user_name");
    localStorage.removeItem("display_name");
  }

  getAccessToken(): string | null {
    return localStorage.getItem("access_token");
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  hasPermission(permission: string, accessType: string = "VIEW"): boolean {
    return this.getPermissions().some(x => x.code === permission && x.accessType === accessType);
  }

  getPermissions(): { code: string; accessType: string }[] {
    const value = localStorage.getItem("permissions");
    if (!value) return [];
    try {
      const parsed = JSON.parse(value);
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }
}
