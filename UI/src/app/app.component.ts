import { Component } from "@angular/core";
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from "@angular/router";
import { AuthService } from "./services/auth.service";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [RouterLink, RouterOutlet, RouterLinkActive],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  constructor(
    public auth: AuthService,
    private _router: Router,
  ) {}

  public isLoginView(): boolean {
    return this._router.url === "/login" || this._router.url === "/";
  }
}
