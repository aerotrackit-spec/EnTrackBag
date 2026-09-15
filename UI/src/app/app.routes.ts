import { Routes } from "@angular/router";
import { LoginComponent } from "./login/login.component";
import { SummaryComponent } from "./dashboard/summary/summary.component";
import { SlaComponent } from "./dashboard/sla/sla.component";
import { DeviceStatusComponent } from "./device-status/device-status.component";
import { authGuard } from "./core/auth.guard";
import { TagReportComponent } from "./tag-report/tag-report.component";
import { BagJourneyComponent } from "./bag-journey/bag-journey.component";
import { AdministrationComponent } from "./administration/administration.component";

export const routes: Routes = [
  { path: "login", component: LoginComponent },
  {
    path: "dashboard/summary",
    component: SummaryComponent,
    canActivate: [authGuard],
  },
  {
    path: "dashboard/sla",
    component: SlaComponent,
    canActivate: [authGuard],
  },
  {
    path: "device-status",
    component: DeviceStatusComponent,
    canActivate: [authGuard],
  },
  {
    path: "tag-report",
    component: TagReportComponent,
    canActivate: [authGuard],
    data: { permission: "TagReport.View", accessType: "VIEW" },
  },
  {
    path: "bag-journey",
    component: BagJourneyComponent,
    canActivate: [authGuard],
    data: { permission: "BagJourney.View", accessType: "VIEW" },
  },
  {
    path: "administration",
    component: AdministrationComponent,
    canActivate: [authGuard],
    data: { permission: "Administration.View", accessType: "VIEW" },
  },
  { path: "", pathMatch: "full", redirectTo: "login" },
  { path: "**", redirectTo: "login" },
];
